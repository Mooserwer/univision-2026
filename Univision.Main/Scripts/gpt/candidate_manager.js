// ═══════════════════════════════════════════════════════════════
//  candidate_manager.js  (이력서 → 후보자 등록, Structured Output + Direct File Upload)
//
//  처리 흐름 (FILE 모드 전용):
//    OpenAI Files API 업로드(purpose:user_data)
//      → Responses API(/v1/responses) 로 후보자 구조화 JSON 추출
//      → 유니비전 후보자 모델로 변환 → 중복확인 → 등록/업데이트 버튼 노출
//
//  선행 로드:
//    makeup_manager.js   ← 베이스(작업 카드 UI: _initJobCard/addLog/updateProgress/_handleError)
//    candidate_schema.js ← candidate_schema
//    candidate_prompt.js ← candidate_prompt
//  전역 의존:
//    API_KEY, window.url_candidate_resume_upload, window.url_candidate_ai_create,
//    window.url_candidate_ai_update, window.url_find_duplicate_candidate
// ═══════════════════════════════════════════════════════════════

class ResumeCandidateManager extends ResumeMakeupManager {

  constructor(workerId, gptModel, onComplete) {
    super("KR", workerId, gptModel, onComplete);
    this.originalFile = null;   // Dropzone File 객체
    this.openaiFileId = null;   // Files API 업로드 후 file_id
    this.resumeInfo   = null;   // 서버 저장 결과(can_resume)
    this.uploadFolder = "";     // 서버 temp_folder
  }

  // ─────────────────────────────────────────────────────────
  //  파일 정보 초기화
  // ─────────────────────────────────────────────────────────
  initDirect(file) {
    try {
      this.originalFile = file;
      const nameEl = this.cardEl?.querySelector('.mu-job-name');
      if (nameEl) { nameEl.textContent = file.name; nameEl.title = file.name; }
      // 진행률/배지는 실제 처리 시작(runOne) 시점에 갱신 → 대기열 카드는 '대기' 유지
      const statusEl = this.cardEl?.querySelector('.mu-job-status-text');
      if (statusEl) statusEl.textContent = "대기 중";
      return true;
    } catch (e) {
      this.addLog("danger", "오류: " + e.message);
      return false;
    }
  }

  // ─────────────────────────────────────────────────────────
  //  서버에 원본 파일 저장 (후보자 등록 시 첨부될 이력서 파일)
  //  텍스트 추출은 하지 않음 — 분석은 OpenAI 가 담당
  // ─────────────────────────────────────────────────────────
  async _saveToServer() {
    this.updateProgress(8, "서버에 파일 저장 중");
    this.addLog("info", "서버에 이력서 파일 저장 중...");

    const fd = new FormData();
    fd.append("file", this.originalFile, this.originalFile.name);

    const res = await fetch(window.url_candidate_resume_upload, { method: "POST", body: fd });
    if (!res.ok) throw new Error(`파일 저장 실패 (HTTP ${res.status})`);

    const data = await res.json();
    if (!data.ok) throw new Error(data.message || "파일 저장 실패");

    this.resumeInfo = data.result;
    if (this.resumeInfo && this.resumeInfo.file_dir) {
      this.resumeInfo.file_dir = "/" + this.resumeInfo.file_dir;
    }
    this.uploadFolder = data.temp_folder;
    this.addLog("success", "이력서 파일 저장 완료 ✔");
  }

  // ─────────────────────────────────────────────────────────
  //  OpenAI Files API 업로드 → file_id
  // ─────────────────────────────────────────────────────────
  async _uploadToOpenAI() {
    if (!this.originalFile) throw new Error("원본 파일 객체가 없습니다.");
    this.updateProgress(20, "OpenAI 파일 업로드 중");
    this.addLog("info", `OpenAI에 파일 업로드 중... <span style="color:#6b7280;">(${this.originalFile.name})</span>`);

    const formData = new FormData();
    formData.append("file", this.originalFile, this.originalFile.name);
    formData.append("purpose", "user_data");

    const res = await fetch("https://api.openai.com/v1/files", {
      method: "POST",
      headers: { "Authorization": `Bearer ${API_KEY}` },
      body: formData
    });
    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(`파일 업로드 실패 ${res.status}: ${err.error?.message ?? res.statusText}`);
    }
    const data = await res.json();
    if (data.error) throw new Error(data.error.message);

    this.openaiFileId = data.id;
    this.addLog("success", `OpenAI 업로드 완료 ✔ <span style="color:#6b7280;font-size:0.75rem;">(${data.id})</span>`);
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
      method: "DELETE",
      headers: { "Authorization": `Bearer ${API_KEY}` }
    }).catch(() => { /* 정리 실패는 무시 */ });
  }

  // ─────────────────────────────────────────────────────────
  //  Responses API 구조화 출력 호출
  // ─────────────────────────────────────────────────────────
  async _callGPT(schema, schemaName, systemPrompt) {
    if (!this.openaiFileId) throw new Error("OpenAI 파일 ID가 없습니다.");

    const res = await fetch("https://api.openai.com/v1/responses", {
      method: "POST",
      headers: { "Content-Type": "application/json", "Authorization": `Bearer ${API_KEY}` },
      body: JSON.stringify({
        model: this.gptModel,
        input: [{
          role: "user",
          content: [
            { type: "input_file", file_id: this.openaiFileId },
            { type: "input_text", text: systemPrompt }
          ]
        }],
        text: { format: { type: "json_schema", name: schemaName, schema, strict: true } }
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
  }

  // ─────────────────────────────────────────────────────────
  //  Responses API 구조화 출력 호출 (텍스트 입력 — 산업/직무 제안용)
  //  파일 없이 instructions(분류표) + input(회사명/직급·부서)만 전달
  // ─────────────────────────────────────────────────────────
  async _callGPTText(schema, schemaName, systemPrompt, userText) {
    const res = await fetch("https://api.openai.com/v1/responses", {
      method: "POST",
      headers: { "Content-Type": "application/json", "Authorization": `Bearer ${API_KEY}` },
      body: JSON.stringify({
        model: this.gptModel,
        instructions: systemPrompt,
        input: userText,
        text: { format: { type: "json_schema", name: schemaName, schema, strict: true } }
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
  }

  // ─────────────────────────────────────────────────────────
  //  이력서 분석 (구조화 JSON)
  // ─────────────────────────────────────────────────────────
  async analyze() {
    this.updateProgress(40, "이력서 내용 분석 중");
    const startTime = Date.now();
    this.addLog("info", "이력서 내용 분석 시작 (직접 파일)...");

    const statusEl = this.cardEl?.querySelector('.mu-job-status-text');
    const timer = setInterval(() => {
      const sec = Math.floor((Date.now() - startTime) / 1000);
      if (sec < 180) { if (statusEl) statusEl.textContent = `이력서 분석 중... ${sec}초`; }
      else clearInterval(timer);
    }, 1000);

    try {
      const parsed = await this._callGPT(candidate_schema, "resume_candidate", candidate_prompt);
      clearInterval(timer);
      const elapsed = ((Date.now() - startTime) / 1000).toFixed(1);
      this.addLog("success", `이력서 분석 완료 ✔ (${elapsed}s)`);

      const preview = JSON.stringify(parsed, null, 2);
      this.addLog("info",
        `<span style="font-size:0.8rem;font-weight:600;color:#0d7377;">추출 결과</span>` +
        `<textarea readonly rows="5" style="display:block;width:100%;margin-top:5px;font-size:0.7rem;` +
        `font-family:monospace;resize:vertical;border:1px solid #b2d8d8;border-radius:5px;` +
        `background:#f7fefe;padding:6px;color:#1e3a5f;line-height:1.5;">${preview}</textarea>`
      );
      return parsed;
    } catch (e) {
      clearInterval(timer);
      this.addLog("danger", `이력서 분석 오류: ${e.message}`);
      return null;
    }
  }

  // ─────────────────────────────────────────────────────────
  //  산업(business) 제안 — 회사명 입력 (실패 시 null, 진행 계속)
  // ─────────────────────────────────────────────────────────
  async analyzeBusi(companyName) {
    this.updateProgress(60, "산업 분석 중");
    this.addLog("info", `[${companyName}] 산업 분석 중...`);
    try {
      const r = await this._callGPTText(candidate_busi_schema, "industry", candidate_prompt_busi, companyName);
      this.addLog("success", `산업 분석 완료 ✔ (${r.business?.length || 0}건)`);
      return r;
    } catch (e) {
      this.addLog("warning", "산업 분석 실패 — 산업 제안 없이 진행합니다. " + e.message);
      return null;
    }
  }

  // ─────────────────────────────────────────────────────────
  //  직무(job) 제안 — "회사명 / 직급·부서" 입력 (실패 시 null, 진행 계속)
  // ─────────────────────────────────────────────────────────
  async analyzeJob(companyDivText) {
    this.updateProgress(72, "직무 분석 중");
    this.addLog("info", `[${companyDivText}] 직무 분석 중...`);
    try {
      const r = await this._callGPTText(candidate_job_schema, "job", candidate_prompt_job, companyDivText);
      this.addLog("success", `직무 분석 완료 ✔ (${r.job?.length || 0}건)`);
      return r;
    } catch (e) {
      this.addLog("warning", "직무 분석 실패 — 직무 제안 없이 진행합니다. " + e.message);
      return null;
    }
  }

  // ─────────────────────────────────────────────────────────
  //  GPT JSON → 유니비전 후보자 모델로 변환
  //  (Candidate/CandidateAi 의 ConvertCandidateCreateModel 이식)
  //  busi/job 은 산업/직무 제안 결과 (없으면 null)
  // ─────────────────────────────────────────────────────────
  buildCandidateModel(data, busi, job) {
    const normMonth = (v) => {
      if (!v) return v;
      if (v.length === 4) return v + "-01";
      if (v.substring(5) === "00") return v.substring(0, 5) + "01";
      return v.replace(".", "-");
    };

    let birth = "";
    if (data?.birth_date) {
      birth = data.birth_date;
      if (birth.length === 4) birth = birth + "-01-01";
      else if (birth.substring(5) === "00-00") birth = birth.substring(0, 5) + "01-01";
    }

    const model = {
      c_seq: 0,
      kor_name:     data?.kor_name ?? '',
      eng_name:     data?.eng_name ?? '',
      birth_date:   birth,
      gender:       data?.gender ?? 0,
      country_code: data?.country_cd ?? 'KR',
      country_name: data?.country_name ?? '한국',
      addr1:        data?.addr1 ?? '',
      ex_addr:      data?.ex_addr ?? 0,
      cell_phone:   data?.cell_phone ?? '',
      phone:        data?.phone ?? '',
      email1:       data?.email1 ?? '',
      email2:       data?.email2 ?? '',
      sns_link1:    data?.sns_link1 ?? '',
      schoolList:   [],
      companyList:  [],
      foreignList:  [],
      resumeList:   [],
      gpt_jobList:  [],
      gpt_busiList: []
    };

    if (data?.schoolList?.length >= 1) {
      for (let i = 0; i < data.schoolList.length; i++) {
        const s = data.schoolList[i];
        if (s?.gubun == "1") continue; // 고등학교 제외
        const school = { gubun: s?.gubun || "3" };
        if (s?.schoolName)     school.schoolName = s.schoolName;
        if (s?.major_name)     school.major_name = s.major_name;
        if (s?.admission_year) school.admission_year = normMonth(s.admission_year);
        if (s?.graduate_year)  school.graduate_year = normMonth(s.graduate_year);
        model.schoolList.push(school);
      }
    }

    if (data?.companyList?.length >= 1) {
      for (let i = 0; i < data.companyList.length; i++) {
        const c = data.companyList[i];
        const company = {};
        if (c?.company_name)  company.company_name = c.company_name;
        if (c?.division_name) company.division_name = c.division_name;
        if (c?.join_dt)       company.join_dt = normMonth(c.join_dt);
        if (c?.leave_dt)      company.leave_dt = normMonth(c.leave_dt);
        else if (i === 0 && company.join_dt) company.is_work = 1;
        model.companyList.push(company);
      }
    }

    if (busi?.business?.length >= 1) {
      for (let i = 0; i < busi.business.length; i++) {
        const b = busi.business[i];
        if (b?.code1) {
          model.gpt_busiList.push({
            code1: parseInt(b.code1), code2: parseInt(b.code2),
            code_name1: b.code_name1, code_name2: b.code_name2, reason: b.reason
          });
        }
      }
    }

    if (job?.job?.length >= 1) {
      for (let i = 0; i < job.job.length; i++) {
        const j = job.job[i];
        if (j?.code1) {
          model.gpt_jobList.push({
            code1: parseInt(j.code1), code2: parseInt(j.code2),
            code_name1: j.code_name1, code_name2: j.code_name2, reason: j.reason
          });
        }
      }
    }

    if (this.resumeInfo) {
      if (!this.resumeInfo.file_type) this.resumeInfo.file_type = data?.resume_type || 'K';
      model.resumeList.push(this.resumeInfo);
    }

    return { model, phone: model.cell_phone, email: model.email1 };
  }

  // ─────────────────────────────────────────────────────────
  //  중복 후보자 확인 → c_seq(>0) / 0(없음) / -1(오류)
  // ─────────────────────────────────────────────────────────
  async checkDuplicate(phone, email) {
    if (!phone && !email) return 0;
    this.updateProgress(50, "중복 후보자 확인 중");
    this.addLog("info", `중복 데이터 확인 중...<br>[${phone}, ${email}]`);
    try {
      const res = await fetch(window.url_find_duplicate_candidate, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ phone, email, except_c: 0 })
      });
      const data = await res.json();
      if (!data.ok) { this.addLog("danger", "중복확인 오류: " + (data.message || "")); return -1; }

      if (data.candidate?.c_seq > 0) {
        this.dupCandidate = data.candidate;
        this.addLog("warning",
          `연락처가 중복되는 후보자가 있습니다.` +
          `<div style="margin-top:6px;font-size:0.82rem;">` +
          `<b>${data.candidate.kor_name ?? ''}</b> · ${data.candidate.cell_phone ?? ''} · ${data.candidate.email1 ?? ''}<br>` +
          `<a href="/Candidate/CandidateDetail?c_seq=${data.candidate.c_seq}" target="_blank">중복 후보자 상세보기</a></div>`
        );
        return data.candidate.c_seq;
      }
      this.addLog("info", "중복 후보자 없음.");
      return 0;
    } catch (e) {
      this.addLog("danger", "중복확인 오류: " + e.message);
      return -1;
    }
  }

  // ─────────────────────────────────────────────────────────
  //  완료 — 등록/업데이트 버튼 노출
  // ─────────────────────────────────────────────────────────
  showRegister(model, dupCseq) {
    if (!this.cardEl) return;

    // 완료 상태 표시
    const fill = this.cardEl.querySelector('.mu-job-pbar-fill');
    fill.style.width = "100%"; fill.classList.add('is-done');
    const pct = this.cardEl.querySelector('.mu-job-pct');
    pct.innerHTML = '<span style="color:#10b981;font-weight:700;">✅ 분석 완료</span>';
    const badge = this.cardEl.querySelector('.mu-job-badge');
    badge.className = "mu-job-badge done"; badge.textContent = "완료";
    const statusEl = this.cardEl.querySelector('.mu-job-status-text');
    statusEl.textContent = "등록 대기"; statusEl.style.color = "#10b981";
    this.cardEl.classList.add('is-complete');

    const area = document.createElement("div");
    area.className = "mu-download-area";

    if (dupCseq > 0) {
      area.innerHTML =
        `<div style="font-size:0.82rem;color:#b45309;margin-bottom:8px;font-weight:600;">중복 후보자가 있습니다. 선택해주세요.</div>` +
        `<button type="button" class="mu-download-btn cai-btn-update" style="margin-bottom:8px;"><i class="fas fa-user-edit"></i><span>이력서 업데이트</span></button>` +
        `<button type="button" class="mu-download-btn cai-btn-new" style="background:linear-gradient(135deg,#94a3b8,#64748b);box-shadow:none;"><i class="fas fa-user-plus"></i><span>신규로 등록</span></button>`;
      this._appendArea(area);
      area.querySelector('.cai-btn-update').addEventListener('click', () =>
        this._submit(window.url_candidate_ai_update, model, dupCseq));
      area.querySelector('.cai-btn-new').addEventListener('click', () =>
        this._submit(window.url_candidate_ai_create, model, null));
    } else {
      area.innerHTML =
        `<button type="button" class="mu-download-btn cai-btn-new"><i class="fas fa-user-plus"></i><span>이력서 등록하러 가기</span></button>`;
      this._appendArea(area);
      area.querySelector('.cai-btn-new').addEventListener('click', () =>
        this._submit(window.url_candidate_ai_create, model, null));
    }

    this.addLog("success", "이력서 등록 준비 완료 — 버튼을 눌러 등록 화면으로 이동하세요.");
    if (typeof this.onComplete === 'function') this.onComplete();

    // 로그 자동 열기
    const logArea = this.cardEl.querySelector('.mu-job-logs');
    if (logArea && !logArea.classList.contains('is-open')) {
      logArea.classList.add('is-open');
      const ico = this.cardEl.querySelector('.mu-job-log-toggle i');
      if (ico) { ico.classList.remove('fa-chevron-down'); ico.classList.add('fa-chevron-up'); }
    }
  }

  _appendArea(area) {
    const pbarWrap = this.cardEl.querySelector('.mu-job-pbar-wrap');
    pbarWrap.insertAdjacentElement('afterend', area);
  }

  _submit(url, model, cSeq) {
    const form = document.createElement('form');
    form.method = 'post';
    form.action = url;
    form.target = '_blank';
    form.style.display = 'none';
    const add = (n, v) => {
      const i = document.createElement('input');
      i.type = 'hidden'; i.name = n; i.value = v;
      form.appendChild(i);
    };
    add('candidate_model', JSON.stringify(model));
    add('uploadFolder', this.uploadFolder);
    if (cSeq != null) add('c_seq', cSeq);
    document.body.appendChild(form);
    form.submit();
    setTimeout(() => form.remove(), 1500);
  }
}
