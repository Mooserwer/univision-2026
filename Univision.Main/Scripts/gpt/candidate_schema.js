// ═══════════════════════════════════════════════════════════════
//  candidate_schema.js
//  이력서 → 후보자 기본정보 구조화 출력 스키마 (Responses API json_schema, strict)
//
//  strict 모드 규칙:
//    - 모든 property 는 required 에 포함
//    - 모든 object 는 additionalProperties:false
//  값이 없는 경우 빈 문자열('') / 빈 배열([]) 로 출력하도록 프롬프트에서 지시.
// ═══════════════════════════════════════════════════════════════

const candidate_schema = {
  type: "object",
  additionalProperties: false,
  properties: {
    resume_type:   { type: "string", description: "한글 이력서면 K, 영문이면 E" },
    kor_name:      { type: "string" },
    eng_name:      { type: "string" },
    birth_date:    { type: "string", description: "yyyy-MM-dd" },
    gender:        { type: "string", description: "남자 1 / 여자 2 / 미상 0" },
    cell_phone:    { type: "string" },
    phone:         { type: "string" },
    email1:        { type: "string" },
    email2:        { type: "string" },
    sns_link1:     { type: "string" },
    country_cd:    { type: "string" },
    country_name:  { type: "string" },
    hope_salary:   { type: "string" },
    hope_salary2:  { type: "string" },
    addr1:         { type: "string" },
    ex_addr:       { type: "string", description: "국내 0 / 해외 1" },
    schoolList: {
      type: "array",
      description: "학력사항(최신순 내림차순)",
      items: {
        type: "object",
        additionalProperties: false,
        properties: {
          gubun:          { type: "string", description: "고1/전문대2/학사3/석사4/박사5" },
          schoolName:     { type: "string" },
          major_name:     { type: "string" },
          admission_year: { type: "string", description: "yyyy-MM 또는 yyyy" },
          graduate_year:  { type: "string", description: "yyyy-MM 또는 yyyy" }
        },
        required: ["gubun", "schoolName", "major_name", "admission_year", "graduate_year"]
      }
    },
    companyList: {
      type: "array",
      description: "경력사항(최신순 내림차순)",
      items: {
        type: "object",
        additionalProperties: false,
        properties: {
          company_name:  { type: "string" },
          division_name: { type: "string" },
          join_dt:       { type: "string", description: "yyyy-MM 또는 yyyy" },
          leave_dt:      { type: "string", description: "yyyy-MM 또는 yyyy" }
        },
        required: ["company_name", "division_name", "join_dt", "leave_dt"]
      }
    }
  },
  required: [
    "resume_type", "kor_name", "eng_name", "birth_date", "gender",
    "cell_phone", "phone", "email1", "email2", "sns_link1",
    "country_cd", "country_name", "hope_salary", "hope_salary2",
    "addr1", "ex_addr", "schoolList", "companyList"
  ]
};
