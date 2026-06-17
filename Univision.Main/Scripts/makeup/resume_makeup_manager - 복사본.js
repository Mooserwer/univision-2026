class ResumeMakeupManager {
  constructor(lang, workerId, gptModel) {
    this.lang = lang;
    this.workerId = workerId;
    this.gptModel = gptModel;
    this.percentage = 0;
    this.isProgress = true;
    this.controller = null;
    this.fileContent = "";
    this.resumeInfo = null;
    this.aiSummaryNC = ""; // No Career
    this.aiSummaryOC = ""; // Only Career

    this.logArea = this._initLogPanel();
  }

  // 로그 패널 초기화 (UI 생성)
  _initLogPanel() {
    const logDummy = document.getElementById("log_dummy");
    const logElement = logDummy.cloneNode(true);
    logElement.id = `log_panel_${this.workerId}`;
    document.getElementById("div_info_area").prepend(logElement);

    logElement.getElementsByClassName('progress_lang')[0].innerText = `[${this.lang}]`;
    return logElement;
  }

  // 공통 로그 출력 메서드
  addLog(type = "info", contents) {
    const eventTime = `[${new Date().toISOString().split('T')[1].split('.')[0]}]`;
    const card = `
            <div class='card border border-${type} shadow mb-2'>
                <div class="card-block text-${type} py-2">
                    <span class='text-dark small'>${eventTime}</span><br/>
                    ${contents}
                </div>
            </div>`;

    const temp = document.createElement("div");
    temp.innerHTML = card;
    temp.classList.add('col-12');

    const panelBody = this.logArea.getElementsByClassName('panel-body')[0];
    panelBody.prepend(temp);

    if (type === "danger") {
      this._handleError();
    }
  }

  _handleError() {
    this.percentage = -1;
    this.logArea.classList.add("border", "border-danger");
    const progressText = this.logArea.getElementsByClassName('progress_percentage')[0];
    progressText.innerText = "작업 실패 - 오류 발생";
    progressText.classList.add("text-danger");
  }

  updateProgress(percent, statusText) {
    this.percentage = percent;
    const target = this.logArea.getElementsByClassName('progress_percentage')[0];
    target.innerText = `${percent}% (${statusText})`;
  }

  // 파일 정보 설정
  setFileInfo(response) {
    try {
      this.addLog("info", "파일 내용 확인 중...");
      this.resumeInfo = response.result;
      this.fileContent = response.file_content;

      if (!this.fileContent) throw "내용을 인식할 수 없는 파일입니다.";

      this.logArea.getElementsByClassName('progress_file_name')[0].innerText = this.resumeInfo.file_path;
      this.updateProgress(5, "파일 확인 완료");

      const preview = `<textarea class='form-control border-info' rows='3' readonly>${this.fileContent}</textarea>`;
      this.addLog("info", "추출된 텍스트 원본:" + preview);
      return true;
    } catch (e) {
      this.addLog("danger", "오류: " + e);
      return false;
    }
  }

  // OpenAI 스트림 호출 공통 로직
  async fetchAIStream(systemMessage, type = "NC") {
    const startTime = new Date();
    this.controller = new AbortController();
    const streamId = `stream-${type}-${this.workerId}`;

    this.addLog("info", `${type === "NC" ? "기본정보" : "경력사항"} 분석 시작...`);
    this.addLog("info", `<span id='sec-${streamId}'></span><textarea id='${streamId}' class='form-control border-info' rows='10' readonly></textarea>`);

    const streamTextArea = document.getElementById(streamId);
    const secLabel = document.getElementById(`sec-${streamId}`);

    const interval = setInterval(() => {
      const elapsed = Math.floor((new Date() - startTime) / 1000);
      if (elapsed < 60) {
        this.updateProgress(this.percentage, `모델 추론 중... (${elapsed}초)`);
      } else {
        clearInterval(interval);
      }
    }, 3000);

    try {
      const data = {
        model: this.gptModel,
        stream: true,
        messages: [
          { role: "system", content: systemMessage },
          { role: "user", content: this.fileContent }
        ]
      };

      // JSON Schema 적용 (GPT-5 등 지원 모델용)
      if (this.gptModel.startsWith("gpt-5")) {
        data.response_format = {
          type: "json_schema",
          json_schema: { name: "resume", strict: true, schema: (type === "NC" ? gpt5_nc_schema_simple : gpt5_oc_schema_simple) }
        };
      }

      const response = await fetch("https://api.openai.com/v1/chat/completions", {
        method: "POST",
        headers: { "Content-Type": "application/json", "Authorization": `Bearer ${API_KEY}` },
        body: JSON.stringify(data),
        signal: this.controller.signal
      });

      const reader = response.body.getReader();
      const decoder = new TextDecoder("utf-8");
      let fullContent = "";

      while (true) {
        const { value, done } = await reader.read();
        if (done) break;

        const seconds = Math.floor((new Date() - startTime) / 1000);
        secLabel.innerText = `(${seconds}초 경과)`;

        const chunk = decoder.decode(value);
        const lines = chunk.split("\n");

        for (let line of lines) {
          if (line.startsWith("data: ") && line !== "data: [DONE]") {
            try {
              const parsed = JSON.parse(line.substring(6));
              const delta = parsed.choices[0].delta.content;
              if (delta) {
                fullContent += delta;
                streamTextArea.value = fullContent;
                streamTextArea.scrollTop = streamTextArea.scrollHeight;
              }
            } catch (e) { }
          }
        }
      }

      if (type === "NC") this.aiSummaryNC = fullContent;
      else this.aiSummaryOC = fullContent;

      this.addLog("info", "분석 완료");
      return true;
    } catch (e) {
      this.addLog("danger", "AI 분석 중 오류: " + e.message);
      return false;
    }
  }

  async finalizeData() {
    this.addLog("info", "분석된 내용을 Makeup 데이터 모델로 변환 중...");

    try {
      // 1. JSON 파싱 및 데이터 클렌징
      let dataNC = this.aiSummaryNC;
      let dataOC = this.aiSummaryOC;

      if (typeof dataNC === "string") {
        dataNC = JSON.parse(dataNC.replace(/json/, "").replace(/```/gi, "").trim());
      }
      if (typeof dataOC === "string") {
        dataOC = JSON.parse(dataOC.replace(/json/, "").replace(/```/gi, "").trim());
      }

      // 2. 데이터 구조 구성 (Default값 처리)
      let dataCore = Array.isArray(dataNC?.core) ? dataNC.core : (dataNC?.core ? [dataNC.core] : ['']);

      const finalModel = {
        candidate: dataNC?.candidate ?? '',
        yob: dataNC?.yob ?? '',
        gender: dataNC?.gender ?? '',
        addr: dataNC?.addr ?? '',
        core: dataCore,
        education: (dataNC?.education?.length > 0) ? dataNC.education : [],
        career: dataOC?.career ?? [],
        learns: (dataNC?.learns?.length > 0) ? dataNC.learns : [],
        awards: (dataNC?.awards?.length > 0) ? dataNC.awards : [],
        activities: (dataNC?.activities?.length > 0) ? dataNC.activities : [],
        overseas: (dataNC?.overseas?.length > 0) ? dataNC.overseas : [],
        skills: (dataNC?.skills?.length > 0) ? dataNC.skills : [],
        certifications: (dataNC?.certifications?.length > 0) ? dataNC.certifications : [],
        languages: (dataNC?.languages?.length > 0) ? dataNC.languages : [],
        etcs: (dataNC?.etcs?.length > 0) ? dataNC.etcs : []
      };

      this.addLog("info", "데이터 모델 변환 완료");
      this.updateProgress(90, "서버 전송 준비 중");

      return finalModel;

    } catch (error) {
      this.addLog("danger", "데이터 변환 중 오류 발생: " + error);
      return null;
    }
  }

  async createResumeFile(finalModel) {
    const filePath = this.resumeInfo.file_path;
    const pureFileName = filePath.includes('.')
      ? filePath.substring(0, filePath.lastIndexOf('.'))
      : filePath;

    this.addLog("info", "서버에 파일 생성 요청 중...");

    try {

      const response = await fetch(window.url_make_makeup2, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          makeup_model: JSON.stringify(finalModel),
          file_name: pureFileName,
          file_type: this.lang === "KR" ? "K" : "E",
          // 아래 정보는 인스턴스 생성 시 전달받거나, 전역변수에서 참조
          
          
        })
      });

      const result = await response.json();

      if (result.ok && result.file_url) {
        this.updateProgress(100, "완료");
        this.addLog("success", `파일 생성 성공: <a href="${result.file_url}" target="_blank" class="btn btn-xs btn-success">다운로드 (${result.file_name})</a>`);
      } else {
        throw new Error(result.message || "서버 응답 오류");
      }
    } catch (error) {
      this.addLog("danger", "파일 생성 실패: " + error.message);
    }
  }
}