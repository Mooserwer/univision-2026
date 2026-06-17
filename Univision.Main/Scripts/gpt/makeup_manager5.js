// ═══════════════════════════════════════════════════════════════
//  resume_makeup_manager5.js  (v5 — Structured Output + Direct File Upload)
//
//  처리 모드:
//    FILE 모드  : PDF / DOCX / HWP → OpenAI Files API 업로드(purpose:user_data)
//                 → Responses API (/v1/responses) 로 file_id 전달
//                 → PDF·DOCX·HWP 모두 지원
//    TEXT 모드  : HWP → 서버 텍스트 추출 후 Chat Completions 텍스트 전달 (폴백)
//
//  요구 파일 (선행 로드):
//    resume_makeup_manager.js   ← 베이스 클래스
//    gpt_5_makeup_schema.js     ← gpt5_nc_schema2 / gpt5_nc_schema / gpt5_oc_schema_simple
// ═══════════════════════════════════════════════════════════════

class ResumeMakeupManager5 extends ResumeMakeupManager {

  constructor(lang, workerId, gptModel, onComplete) {
    super(lang, workerId, gptModel, onComplete);
    this.originalFile  = null;   // Dropzone File 객체
    this.openaiFileId  = null;   // Files API 업로드 후 file_id
    this.fileDataUrl   = null;   // (미사용, 하위호환 유지)
    this.useFileUpload = false;  // FILE 모드 여부
  }

  // ─────────────────────────────────────────────────────────
  //  파일 정보 초기화
  //
  //  [FILE 모드] PDF / DOCX / HWP → initDirect(file)
  //    - OpenAI Files API 업로드 후 Responses API 로 file_id 전달
  //    - response 없음, originalFile 만 받음
  //
  //  [TEXT 모드] HWP → initFromServer(response)  ← 폴백
  //    - nGptController.GptResumeUpload 를 거쳐 텍스트 추출
  //    - response = { ok, file_content, result: { file_path, file_extension } }
  // ─────────────────────────────────────────────────────────

  /** PDF / DOCX / HWP — 직접 파일 모드 초기화 */
  initDirect(file) {
    try {
      const ext = '.' + file.name.split('.').pop().toLowerCase();
      this.resumeInfo    = { file_path: file.name, file_extension: ext };
      this.fileContent   = '';
      this.useFileUpload = true;
      this.originalFile  = file;

      const nameEl = this.cardEl?.querySelector('.mu-job-name');
      if (nameEl) { nameEl.textContent = file.name; nameEl.title = file.name; }

      this.updateProgress(3, "확인 완료 — 📂 직접 파일 (OpenAI)");
      this.addLog("info", `${ext.toUpperCase().replace('.', '')} — OpenAI 직접 파일 업로드 방식으로 처리합니다.`);
      return true;
    } catch (e) {
      this.addLog("danger", "오류: " + e.message);
      return false;
    }
  }

  /** HWP — 서버 텍스트 추출 모드 초기화 (폴백) */
  initFromServer(response) {
    try {
      if (!response?.ok)   throw response?.message || "서버 업로드 실패";
      if (!response.file_content) throw "파일 내용을 인식할 수 없습니다.";

      this.resumeInfo    = response.result;
      this.fileContent   = response.file_content;
      this.useFileUpload = false;

      const nameEl = this.cardEl?.querySelector('.mu-job-name');
      if (nameEl) { nameEl.textContent = this.resumeInfo.file_path; nameEl.title = this.resumeInfo.file_path; }

      this.updateProgress(3, "확인 완료 — 📝 텍스트 추출");
      this.addLog("info", "서버 텍스트 추출 방식으로 처리합니다.");
      this.addLog("info",
        `추출된 텍스트:<br>` +
        `<textarea class='form-control mt-1' rows='3' readonly ` +
          `style='font-size:0.72rem;resize:vertical;border-color:#e0e7ff;'>` +
          `${this.fileContent}</textarea>`
      );
      return true;
    } catch (e) {
      this.addLog("danger", "오류: " + e);
      return false;
    }
  }

  /** 하위호환 — 기존 setFileInfo 시그니처 유지 */
  setFileInfo(response, originalFile) {
    if (originalFile && !originalFile.name.toLowerCase().endsWith('.hwp')) {
      return this.initDirect(originalFile);
    }
    return this.initFromServer(response);
  }

  // ─────────────────────────────────────────────────────────
  //  OpenAI Files API 업로드 → file_id 반환
  //  purpose: "user_data" → Responses API (/v1/responses) 에서 사용 가능
  //  PDF / DOCX / HWP 등 다양한 포맷 지원
  // ─────────────────────────────────────────────────────────
  async _uploadToOpenAI() {
    if (!this.originalFile) throw new Error("원본 파일 객체가 없습니다.");

    this.addLog("info", `OpenAI에 파일 업로드 중... <span style="color:#6b7280;">(${this.originalFile.name})</span>`);

    const formData = new FormData();
    formData.append("file",    this.originalFile, this.originalFile.name);
    formData.append("purpose", "user_data");

    const res = await fetch("https://api.openai.com/v1/files", {
      method:  "POST",
      headers: { "Authorization": `Bearer ${API_KEY}` },
      body:    formData
    });

    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(`파일 업로드 실패 ${res.status}: ${err.error?.message ?? res.statusText}`);
    }

    const data = await res.json();
    if (data.error) throw new Error(data.error.message);

    this.openaiFileId = data.id;
    this.addLog("success", `파일 업로드 완료 ✔ <span style="color:#6b7280;font-size:0.75rem;">(${data.id})</span>`);
    return data.id;
  }

  // ─────────────────────────────────────────────────────────
  //  OpenAI 파일 삭제 (사용 후 정리)
  // ─────────────────────────────────────────────────────────
  async _deleteFromOpenAI() {
    if (!this.openaiFileId) return;
    const id = this.openaiFileId;
    this.openaiFileId = null;
    await fetch(`https://api.openai.com/v1/files/${id}`, {
      method:  "DELETE",
      headers: { "Authorization": `Bearer ${API_KEY}` }
    }).catch(() => { /* 정리 실패는 무시 */ });
  }

  // ─────────────────────────────────────────────────────────
  //  GPT 호출 코어 (FILE 모드 / TEXT 모드 자동 분기)
  //
  //  FILE 모드 → Responses API (/v1/responses)
  //    - input_file + input_text 로 전달
  //    - 응답: output[0].content[0].text
  //
  //  TEXT 모드 → Chat Completions (/v1/chat/completions)
  //    - system + user 텍스트 전달
  //    - 응답: choices[0].message.content
  // ─────────────────────────────────────────────────────────
  async _callGPT(schema, schemaName, systemPrompt) {

    if (this.useFileUpload && this.openaiFileId) {
      // ── FILE 모드: Responses API ──────────────────────────
      const res = await fetch("https://api.openai.com/v1/responses", {
        method:  "POST",
        headers: {
          "Content-Type":  "application/json",
          "Authorization": `Bearer ${API_KEY}`
        },
        body: JSON.stringify({
          model: this.gptModel,
          input: [{
            role: "user",
            content: [
              { type: "input_file", file_id: this.openaiFileId },
              { type: "input_text", text: systemPrompt }
            ]
          }],
          text: {
            format: {
              type:        "json_schema",
              name:        schemaName,
              schema,
              strict:      true
            }
          }
        })
      });

      if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        throw new Error(`Responses API ${res.status}: ${err.error?.message ?? res.statusText}`);
      }

      const data = await res.json();
      if (data.error) throw new Error(data.error.message);

      const content = data.output?.[0]?.content?.[0]?.text;
      if (!content) throw new Error("GPT 빈 응답 (Responses API)");

      return JSON.parse(content);

    } else {
      // ── TEXT 모드: Chat Completions ──────────────────────
      const res = await fetch("https://api.openai.com/v1/chat/completions", {
        method:  "POST",
        headers: {
          "Content-Type":  "application/json",
          "Authorization": `Bearer ${API_KEY}`
        },
        body: JSON.stringify({
          model: this.gptModel,
          messages: [
            { role: "system", content: systemPrompt },
            { role: "user",   content: this.fileContent }
          ],
          response_format: {
            type: "json_schema",
            json_schema: { name: schemaName, schema, strict: true }
          }
        })
      });

      if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        throw new Error(`Chat API ${res.status}: ${err.error?.message ?? res.statusText}`);
      }

      const data = await res.json();
      if (data.error) throw new Error(data.error.message);

      const content = data.choices?.[0]?.message?.content;
      if (!content) throw new Error("GPT 빈 응답 (Chat Completions)");

      return JSON.parse(content);
    }
  }

  // ─────────────────────────────────────────────────────────
  //  분석 요청 (NC / OC 공통)
  // ─────────────────────────────────────────────────────────
  async fetchStructured(schema, schemaName, systemPrompt, label) {
    const startTime = Date.now();
    const modeTag   = this.useFileUpload ? "(직접 파일)" : "(텍스트 추출)";

    this.addLog("info", `${label} 분석 시작 ${modeTag}...`);

    const statusEl = this.cardEl?.querySelector('.mu-job-status-text');
    const timer = setInterval(() => {
      const sec = Math.floor((Date.now() - startTime) / 1000);
      if (sec < 180) {
        if (statusEl) statusEl.textContent = `${label} 분석 중... ${sec}초`;
      } else {
        clearInterval(timer);
      }
    }, 1000);

    try {
      const parsed  = await this._callGPT(schema, schemaName, systemPrompt);
      const elapsed = ((Date.now() - startTime) / 1000).toFixed(1);

      clearInterval(timer);
      this.addLog("success", `${label} 완료 ✔ (${elapsed}s)`);

      // 구조화 출력 결과 미리보기
      const preview = JSON.stringify(parsed, null, 2);
      this.addLog("info",
        `<span style="font-size:0.8rem;font-weight:600;color:#5b21b6;">${label} 추출 결과</span>` +
        `<textarea readonly rows="5" style="` +
          `display:block;width:100%;margin-top:5px;font-size:0.7rem;font-family:monospace;` +
          `resize:vertical;border:1px solid #c4b5fd;border-radius:5px;` +
          `background:#faf7ff;padding:6px;color:#2e1065;line-height:1.5;` +
        `">${preview}</textarea>`
      );

      return parsed;

    } catch (e) {
      clearInterval(timer);
      this.addLog("danger", `${label} 오류: ${e.message}`);
      return null;
    }
  }

  // ─────────────────────────────────────────────────────────
  //  서버 파일 생성 요청 (verbose 오류 로그)
  // ─────────────────────────────────────────────────────────
  async createResumeFile(finalModel) {
    const fp   = this.resumeInfo?.file_path ?? '';
    const name = fp.includes('.') ? fp.substring(0, fp.lastIndexOf('.')) : fp;

    this.addLog("info",
      `서버에 파일 생성 요청 중... ` +
      `<span style="color:#6b7280;font-size:0.8rem;">(${name})</span>`
    );

    try {
      const res = await fetch(window.url_make_makeup2, {
        method:  "POST",
        headers: { "Content-Type": "application/json" },
        body:    JSON.stringify({
          makeup_model: JSON.stringify(finalModel),
          file_name:    name,
          file_type:    this.lang === "KR" ? "K" : "E",
        })
      });

      if (!res.ok) {
        const txt = await res.text().catch(() => '');
        throw new Error(`HTTP ${res.status} — ${txt.substring(0, 300)}`);
      }

      const result = await res.json();
      if (result.ok && result.file_url) {
        this._showComplete(result.file_url, result.file_name);
        if (typeof this.onComplete === 'function') this.onComplete();
        return { ok: true, file_name: result.file_name, file_url: result.file_url };
      } else {
        throw new Error(result.message || '서버 오류 (알 수 없음)');
      }
    } catch (e) {
      this.addLog("danger", "파일 생성 실패: " + e.message);
      return { ok: false, message: e.message };
    }
  }

  // ─────────────────────────────────────────────────────────
  //  NC + OC 데이터 → 최종 Makeup 모델 병합
  // ─────────────────────────────────────────────────────────
  _buildModel(nc, oc) {
    // core는 desc/depth 계층 객체 배열 (구버전 string 배열은 서버에서 호환 처리)
    const core = Array.isArray(nc?.core) ? nc.core : [];

    return {
      candidate:      nc?.candidate      ?? '',
      yob:            nc?.yob            ?? '',
      gender:         nc?.gender         ?? '',
      addr:           nc?.addr           ?? '',
      core,
      education:      nc?.education      ?? [],
      career:         oc?.career         ?? [],
      learns:         nc?.learns         ?? [],
      awards:         nc?.awards         ?? [],
      activities:     nc?.activities     ?? [],
      overseas:       nc?.overseas       ?? [],
      skills:         nc?.skills         ?? [],
      certifications: nc?.certifications ?? [],
      languages:      nc?.languages      ?? [],
      etcs:           nc?.etcs           ?? [],
      selfintro:      nc?.selfintro      ?? []
    };
  }
}
