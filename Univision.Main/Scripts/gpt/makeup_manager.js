class ResumeMakeupManager {
  /**
   * @param {string}        lang       - "KR" | "EN"
   * @param {number}        workerId   - 고유 작업 번호
   * @param {string}        gptModel   - GPT 모델명
   * @param {Function|null} onComplete - 완료 콜백
   */
  constructor(lang, workerId, gptModel, onComplete) {
    this.lang = lang;
    this.workerId = workerId;
    this.gptModel = gptModel;
    this.onComplete = onComplete || null;
    this.percentage = 0;
    this.controller = null;
    this.fileContent = "";
    this.resumeInfo = null;
    this.aiSummaryNC = "";
    this.aiSummaryOC = "";
    this._logsOpen = false;

    this.cardEl = this._initJobCard();
  }

  // ─────────────────────────────────────────────────────────
  //  Job 카드 생성 (우측 패널에 삽입)
  // ─────────────────────────────────────────────────────────
  _initJobCard() {
    const list = document.getElementById("mu_jobs_list");
    if (!list) return null;

    const isKR = this.lang === "KR";
    const langCls = isKR ? "kr" : "en";
    const langEmoji = isKR ? "🇰🇷" : "🌐";

    const card = document.createElement("div");
    card.id = `mu_job_${this.workerId}`;
    card.className = "mu-job-card";
    card.innerHTML = `
      <!-- 헤더 -->
      <div class="mu-job-head">
        <div class="mu-job-lang ${langCls}">${langEmoji}</div>
        <div class="mu-job-info">
          <div class="mu-job-name">파일 분석 준비 중...</div>
          <div class="mu-job-status-text">대기 중</div>
        </div>
        <span class="mu-job-badge">대기</span>
      </div>

      <!-- 프로그레스 바 -->
      <div class="mu-job-pbar-wrap">
        <div class="mu-job-pbar-track">
          <div class="mu-job-pbar-fill" style="width: 2%;"></div>
        </div>
        <div class="mu-job-pct">0%</div>
      </div>

      <!-- 로그 토글 버튼 -->
      <button class="mu-job-log-toggle" onclick="
        var logs = this.closest('.mu-job-card').querySelector('.mu-job-logs');
        logs.classList.toggle('is-open');
        var ico = this.querySelector('i');
        ico.classList.toggle('fa-chevron-down');
        ico.classList.toggle('fa-chevron-up');
      ">
        <i class="fas fa-chevron-down" style="font-size:0.65rem;"></i>
        로그 보기
      </button>

      <!-- 로그 영역 -->
      <div class="mu-job-logs"></div>
    `;

    // 맨 위에 추가 (새 작업이 상단에 쌓임)
    list.prepend(card);
    return card;
  }

  // ─────────────────────────────────────────────────────────
  //  로그 추가
  // ─────────────────────────────────────────────────────────
  addLog(type = "info", contents) {
    if (!this.cardEl) return;

    const iconMap = {
      info: { icon: '●', color: '#6366f1' },
      success: { icon: '✔', color: '#10b981' },
      danger: { icon: '✖', color: '#ef4444' },
      warning: { icon: '▲', color: '#f59e0b' },
    };
    const { icon, color } = iconMap[type] || { icon: '●', color: '#94a3b8' };
    const time = new Date().toISOString().split('T')[1].split('.')[0];

    const item = document.createElement("div");
    item.className = "mu-log-item";
    item.innerHTML = `
      <span class="mu-log-icon" style="color:${color};">${icon}</span>
      <div class="mu-log-body">
        <span class="mu-log-time">${time}</span>
        <div class="mu-log-text">${contents}</div>
      </div>
    `;

    const logsEl = this.cardEl.querySelector('.mu-job-logs');
    logsEl.prepend(item);

    if (type === "danger") this._handleError();
  }

  // ─────────────────────────────────────────────────────────
  //  오류 처리
  // ─────────────────────────────────────────────────────────
  _handleError() {
    if (!this.cardEl) return;
    this.cardEl.classList.add("is-error");

    const badge = this.cardEl.querySelector('.mu-job-badge');
    badge.className = "mu-job-badge error";
    badge.textContent = "오류";

    const fill = this.cardEl.querySelector('.mu-job-pbar-fill');
    fill.classList.add('is-error');
    fill.style.width = "100%";

    const statusText = this.cardEl.querySelector('.mu-job-status-text');
    statusText.textContent = "작업 실패";
    statusText.style.color = "#ef4444";
  }

  // ─────────────────────────────────────────────────────────
  //  진행률 업데이트
  // ─────────────────────────────────────────────────────────
  updateProgress(percent, statusText) {
    if (!this.cardEl) return;
    this.percentage = percent;

    const fill = this.cardEl.querySelector('.mu-job-pbar-fill');
    const pct = this.cardEl.querySelector('.mu-job-pct');
    const status = this.cardEl.querySelector('.mu-job-status-text');
    const badge = this.cardEl.querySelector('.mu-job-badge');

    fill.style.width = `${percent}%`;
    pct.textContent = `${percent}%`;
    status.textContent = statusText;

    if (percent > 0 && percent < 100) {
      badge.className = "mu-job-badge processing";
      badge.textContent = "처리 중";
    }
  }

  // ─────────────────────────────────────────────────────────
  //  파일 정보 설정
  // ─────────────────────────────────────────────────────────
  setFileInfo(response) {
    try {
      this.addLog("info", "파일 내용 확인 중...");
      this.resumeInfo = response.result;
      this.fileContent = response.file_content;

      if (!this.fileContent) throw "내용을 인식할 수 없는 파일입니다.";

      // 파일명 표시
      const nameEl = this.cardEl.querySelector('.mu-job-name');
      nameEl.textContent = this.resumeInfo.file_path;
      nameEl.title = this.resumeInfo.file_path;

      this.updateProgress(5, "파일 확인 완료");

      const preview = `<textarea class='form-control mt-1' rows='3' readonly style='font-size:0.72rem; resize:vertical; border-color:#e0e7ff;'>${this.fileContent}</textarea>`;
      this.addLog("info", "추출된 텍스트 원본:" + preview);
      return true;
    } catch (e) {
      this.addLog("danger", "오류: " + e);
      return false;
    }
  }

  // ─────────────────────────────────────────────────────────
  //  OpenAI 스트림 호출
  // ─────────────────────────────────────────────────────────
  async fetchAIStream(systemMessage, type = "NC") {
    const startTime = new Date();
    this.controller = new AbortController();
    const streamId = `stream_${type}_${this.workerId}`;
    const label = type === "NC" ? "기본정보" : "경력사항";

    this.addLog("info", `${label} 분석 시작...`);
    this.addLog("info", `
      <span id="sec_${streamId}" style="font-size:0.68rem; color:#94a3b8;"></span>
      <textarea id="${streamId}" class="form-control mt-1" rows="7" readonly
        style="font-size:0.72rem; font-family:monospace; resize:vertical; border-color:#e0e7ff;"></textarea>
    `);

    const streamTA = document.getElementById(streamId);
    const secEl = document.getElementById(`sec_${streamId}`);

    // 경과 시간 타이머
    const timer = setInterval(() => {
      const sec = Math.floor((new Date() - startTime) / 1000);
      if (sec < 180) {
        if (secEl) secEl.textContent = `(${sec}초 경과)`;
        const statusEl = this.cardEl ? this.cardEl.querySelector('.mu-job-status-text') : null;
        if (statusEl) statusEl.textContent = `${label} 분석 중... ${sec}초`;
      } else clearInterval(timer);
    }, 1000);

    try {
      const data = {
        model: this.gptModel,
        stream: true,
        messages: [
          { role: "system", content: systemMessage },
          { role: "user", content: this.fileContent }
        ]
      };

      const res = await fetch("https://api.openai.com/v1/chat/completions", {
        method: "POST",
        headers: { "Content-Type": "application/json", "Authorization": `Bearer ${API_KEY}` },
        body: JSON.stringify(data),
        signal: this.controller.signal
      });

      const reader = res.body.getReader();
      const decoder = new TextDecoder("utf-8");
      let full = "";
      let buffer = ""; // 청크 경계에서 잘린 불완전한 줄 누적용

      while (true) {
        const { value, done } = await reader.read();
        if (done) break;

        const sec = Math.floor((new Date() - startTime) / 1000);
        if (secEl) secEl.textContent = `(${sec}초 경과)`;

        // stream:true 옵션 → 멀티바이트 문자가 청크 경계에서 잘려도 안전하게 디코딩
        buffer += decoder.decode(value, { stream: true });

        // 완전한 줄(\n 단위)만 처리, 마지막 불완전한 줄은 buffer에 남겨 다음 청크와 합산
        const lines = buffer.split("\n");
        buffer = lines.pop();

        for (const line of lines) {
          const trimmed = line.trimEnd();
          if (trimmed.startsWith("data: ") && trimmed !== "data: [DONE]") {
            try {
              const parsed = JSON.parse(trimmed.substring(6));
              const delta = parsed.choices[0]?.delta?.content;
              if (delta) {
                full += delta;
                if (streamTA) {
                  streamTA.value = full;
                  streamTA.scrollTop = streamTA.scrollHeight;
                }
              }
            } catch (_) { /* 불완전한 청크 무시 */ }
          }
        }
      }

      // 스트림 종료 후 decoder flush + 잔여 버퍼 처리
      buffer += decoder.decode();
      if (buffer.trim().startsWith("data: ") && buffer.trim() !== "data: [DONE]") {
        try {
          const parsed = JSON.parse(buffer.trim().substring(6));
          const delta = parsed.choices[0]?.delta?.content;
          if (delta) { full += delta; if (streamTA) streamTA.value = full; }
        } catch (_) { }
      }

      clearInterval(timer);
      if (type === "NC") this.aiSummaryNC = full;
      else this.aiSummaryOC = full;

      this.addLog("info", `${label} 분석 완료 ✔`);
      return true;

    } catch (e) {
      clearInterval(timer);
      this.addLog("danger", "AI 분석 오류: " + e.message);
      return false;
    }
  }

  // ─────────────────────────────────────────────────────────
  //  JSON 문자열 → 객체 변환 (자동 복구 포함)
  //  @param {string} raw   - 파싱할 원본 문자열
  //  @param {string} label - 오류 메시지에 표시할 구분 이름 (예: "NC(기본정보)")
  // ─────────────────────────────────────────────────────────
  _parseJSON(raw, label) {
    // 1) 마크다운 코드블록 제거
    const str = (typeof raw === "string" ? raw : JSON.stringify(raw))
      .replace(/^```json\s*/i, '')
      .replace(/^```\s*/i, '')
      .replace(/```\s*$/i, '')
      .trim();

    // 2) 먼저 정상 파싱 시도
    try {
      return JSON.parse(str);
    } catch (e1) {
      // 3) jsonrepair 로 자동 복구 후 재시도
      if (typeof jsonrepair === 'function') {
        try {
          const repaired = jsonrepair(str);
          const result = JSON.parse(repaired);
          this.addLog("warning", `⚠ [${label}] JSON 자동 복구 후 파싱 성공 — GPT 응답에 문법 오류가 있었습니다. 내용을 꼭 검수해주세요.`);
          return result;
        } catch (e2) {
          throw new Error(`[${label}] JSON 복구 실패: ${e2.message}`);
        }
      }
      throw new Error(`[${label}] JSON 파싱 실패: ${e1.message}`);
    }
  }

  // ─────────────────────────────────────────────────────────
  //  데이터 정제
  // ─────────────────────────────────────────────────────────
  async finalizeData() {
    this.addLog("info", "Makeup 데이터 모델로 변환 중...");

    // NC / OC 각각 독립적으로 파싱 — 한쪽 실패가 다른쪽에 영향 없음
    let dataNC = null;
    let dataOC = null;
    let hasError = false;

    try {
      dataNC = this._parseJSON(this.aiSummaryNC, "NC(기본정보)");
    } catch (e) {
      this.addLog("danger", `데이터 변환 오류 — ${e.message}`);
      hasError = true;
    }

    try {
      dataOC = this._parseJSON(this.aiSummaryOC, "OC(경력사항)");
    } catch (e) {
      this.addLog("danger", `데이터 변환 오류 — ${e.message}`);
      hasError = true;
    }

    // 둘 다 실패한 경우만 중단, 한쪽만 실패하면 빈값으로 계속 진행
    if (!dataNC && !dataOC) {
      this.addLog("danger", "NC / OC 모두 파싱 실패 — 작업을 중단합니다.");
      return null;
    }
    if (hasError) {
      this.addLog("warning", "⚠ 일부 데이터 파싱 실패 — 해당 항목은 빈값으로 처리됩니다. 생성 후 반드시 검수해주세요.");
    }

    const dataCore = Array.isArray(dataNC?.core)
      ? dataNC.core
      : (dataNC?.core ? [dataNC.core] : ['']);

    const model = {
      candidate: dataNC?.candidate ?? '',
      yob: dataNC?.yob ?? '',
      gender: dataNC?.gender ?? '',
      addr: dataNC?.addr ?? '',
      core: dataCore,
      education: dataNC?.education?.length > 0 ? dataNC.education : [],
      career: dataOC?.career ?? [],
      learns: dataNC?.learns?.length > 0 ? dataNC.learns : [],
      awards: dataNC?.awards?.length > 0 ? dataNC.awards : [],
      activities: dataNC?.activities?.length > 0 ? dataNC.activities : [],
      overseas: dataNC?.overseas?.length > 0 ? dataNC.overseas : [],
      skills: dataNC?.skills?.length > 0 ? dataNC.skills : [],
      certifications: dataNC?.certifications?.length > 0 ? dataNC.certifications : [],
      languages: dataNC?.languages?.length > 0 ? dataNC.languages : [],
      etcs: dataNC?.etcs?.length > 0 ? dataNC.etcs : []
    };

    this.addLog("info", "데이터 변환 완료");
    this.updateProgress(90, "서버 전송 준비 중");
    return model;
  }

  // ─────────────────────────────────────────────────────────
  //  서버 파일 생성 요청
  // ─────────────────────────────────────────────────────────
  async createResumeFile(finalModel) {
    const fp = this.resumeInfo.file_path;
    const name = fp.includes('.') ? fp.substring(0, fp.lastIndexOf('.')) : fp;

    this.addLog("info", "서버에 파일 생성 요청 중...");
    try {
      const res = await fetch(window.url_make_makeup2, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          makeup_model: JSON.stringify(finalModel),
          file_name: name,
          file_type: this.lang === "KR" ? "K" : "E",
        })
      });
      const result = await res.json();
      if (result.ok && result.file_url) {
        this._showComplete(result.file_url, result.file_name);
        if (typeof this.onComplete === 'function') this.onComplete();
      } else {
        throw new Error(result.message || "서버 오류");
      }
    } catch (e) {
      this.addLog("danger", "파일 생성 실패: " + e.message);
    }
  }

  // ─────────────────────────────────────────────────────────
  //  완료 이펙트
  // ─────────────────────────────────────────────────────────
  _showComplete(fileUrl, fileName) {
    if (!this.cardEl) return;

    // 프로그레스 100%
    const fill = this.cardEl.querySelector('.mu-job-pbar-fill');
    fill.style.width = "100%";
    fill.classList.add('is-done');

    const pct = this.cardEl.querySelector('.mu-job-pct');
    pct.innerHTML = '<span style="color:#10b981; font-weight:700;">✅ 완료!</span>';

    // 배지
    const badge = this.cardEl.querySelector('.mu-job-badge');
    badge.className = "mu-job-badge done";
    badge.textContent = "완료";

    // 상태
    const statusEl = this.cardEl.querySelector('.mu-job-status-text');
    statusEl.textContent = "파일 생성 완료";
    statusEl.style.color = "#10b981";

    // 카드 완료 스타일
    this.cardEl.classList.add('is-complete');

    // 다운로드 영역 삽입 (프로그레스 바 아래)
    const pbarWrap = this.cardEl.querySelector('.mu-job-pbar-wrap');

    const dlArea = document.createElement("div");
    dlArea.className = "mu-download-area";
    dlArea.innerHTML = `
      <a href="${fileUrl}" target="_blank" class="mu-download-btn">
        <i class="fas fa-file-download" style="font-size:1rem;"></i>
        <span>
          다운로드
          <span class="mu-download-filename">${fileName}</span>
        </span>
      </a>
      <div class="mu-sparkles-container" id="sparkles_${this.workerId}"></div>
    `;

    pbarWrap.insertAdjacentElement('afterend', dlArea);

    // 다운로드 클릭 시 버튼 색 변경 (다운로드 여부 구분)
    const dlBtn = dlArea.querySelector('.mu-download-btn');
    dlBtn.addEventListener('click', function () {
      this.classList.add('is-downloaded');
      const icon = this.querySelector('i');
      if (icon) icon.className = 'fas fa-check';
    });

    // 스파클
    this._runSparkles(document.getElementById(`sparkles_${this.workerId}`));

    // 로그에도 추가
    this.addLog("success", `완료! <a href="${fileUrl}" target="_blank" style="color:#10b981; font-weight:700;">${fileName}</a>`);

    // 로그 자동 열기
    const logArea = this.cardEl.querySelector('.mu-job-logs');
    if (!logArea.classList.contains('is-open')) {
      logArea.classList.add('is-open');
      const toggleBtn = this.cardEl.querySelector('.mu-job-log-toggle i');
      if (toggleBtn) {
        toggleBtn.classList.remove('fa-chevron-down');
        toggleBtn.classList.add('fa-chevron-up');
      }
    }
  }

  // ─────────────────────────────────────────────────────────
  //  스파클 파티클
  // ─────────────────────────────────────────────────────────
  _runSparkles(container) {
    if (!container) return;
    const colors = ['#10b981', '#34d399', '#6ee7b7', '#f59e0b', '#fbbf24', '#818cf8', '#a78bfa', '#fb7185'];

    const burst = (count) => {
      for (let i = 0; i < count; i++) {
        setTimeout(() => {
          const dot = document.createElement('div');
          const sz = Math.random() * 8 + 3;
          dot.style.cssText = `
            position: absolute;
            width: ${sz}px; height: ${sz}px;
            border-radius: 50%;
            background: ${colors[Math.floor(Math.random() * colors.length)]};
            left: ${5 + Math.random() * 90}%;
            top:  ${5 + Math.random() * 90}%;
            animation: sparkle-anim 0.9s ease-out forwards;
            pointer-events: none; z-index: 20;
          `;
          container.appendChild(dot);
          setTimeout(() => dot.remove(), 950);
        }, i * 55);
      }
    };

    burst(16);
    setTimeout(() => burst(12), 950);
    setTimeout(() => burst(8), 1900);
  }
}