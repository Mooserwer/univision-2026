// ═══════════════════════════════════════════════════════════════
//  makeup_prompt.js
//  GPT-4.1 전용 프롬프트 (GPT-5 프롬프트 제거됨)
//  - KR 계열 : 이력서 언어와 무관하게 **한국어** 출력 강제
//  - EN 계열 : 이력서 언어와 무관하게 **영어** 출력 강제
// ═══════════════════════════════════════════════════════════════


// ───────────────────────────────────────────────────────────────
//  KR · NC (기본정보 — 경력 제외)
// ───────────────────────────────────────────────────────────────
const gpt4_kr_no_career_prompt_base = `
[ACTION]
Convert the resume content into JSON format based on the following 'JSON structure description'.

[CRITICAL LANGUAGE RULE]
All output TEXT VALUES must be written in Korean (한국어), regardless of the language of the source resume.
Even if the resume is entirely in English, extract and translate all content into Korean before saving.
Proper nouns (school names, company names, certification names) may be kept in their original language only if a Korean name does not exist.
Do NOT output any English sentences or phrases as values — translate them into Korean.

[JSON structure description]
{
"candidate": {
"type": "string",
"description": "이름. 한국어 이름(3글자)인 경우 글자 사이에 공백을 추가. 예) '홍길동' → '홍 길 동'. 외국어 이름은 그대로 저장."
},
"yob": {
"type": "string",
"description": "생년월일. 전체 날짜가 있으면 'YYYY.MM.DD', 월까지만 있으면 'YYYY.MM', 년도만 있으면 'YYYY' 형식으로 저장. 정보가 없으면 빈값('')."
},
"gender": {
"type": "string",
"description": "성별. 정보가 있으면 '남성' 또는 '여성'으로 저장. 정보가 없으면 빈값('')."
},
"addr": {
"type": "string",
"description": "거주지 주소. 시/구 까지만 저장. 예) '서울시 강남구 테헤란로 36' → '서울시 강남구'. 정보가 없으면 빈값('')."
},
"core": {
"type": "array",
"description": "핵심역량 목록. 이력서에 기재된 내용을 그대로 사용하되 한국어로 저장. 없으면 빈 배열.",
"items": {
"type": "string"
}
},
"education": {
"type": "array",
"description": "학력사항 목록. 최신순 내림차순 정렬.",
"items": {
"type": "object",
"properties": {
"school": {
"type": "string",
"description": "학교명. 한국어 명칭이 있으면 한국어로 저장."
},
"major": {
"type": "string",
"description": "전공명. 한국어로 저장."
},
"grade": {
"type": "string",
"description": "학점 (예: '4.0/4.5'). 정보가 없으면 빈값('')."
},
"area": {
"type": "string",
"description": "학교 소재 지역. 시/도 명칭만 저장 (예: '서울', '수원'). 정보가 없으면 빈값('')."
},
"degree": {
"type": "string",
"description": "학위. '학사', '석사', '박사' 등으로 저장. 정보가 없으면 빈값('')."
},
"is_grdt": {
"type": "string",
"description": "졸업여부. '졸업', '수료', '재학중', '중퇴', '휴학' 중 해당 항목으로 저장. 정보가 없으면 빈값('')."
},
"ad_yyyymm": {
"type": "string",
"description": "입학년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
},
"gdt_yyyymm": {
"type": "string",
"description": "졸업년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
}
},
"required": [ "school", "major", "gdt_yyyymm" ]
}
},
"certifications": {
"type": "array",
"description": "자격사항(자격증) 목록. 최신순 내림차순 정렬.",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "자격(증)명."
},
"gov": {
"type": "string",
"description": "발급기관. 정보가 없으면 빈값('')."
},
"year": {
"type": "string",
"description": "취득년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
}
},
"required": [ "name", "year" ]
}
},
"awards": {
"type": "array",
"description": "수상내역 목록. 최신순 내림차순 정렬.",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "수상명칭."
},
"gov": {
"type": "string",
"description": "관련기관. 정보가 없으면 빈값('')."
},
"year": {
"type": "string",
"description": "수상년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
}
},
"required": [ "name", "year" ]
}
},
"learns": {
"type": "array",
"description": "수료사항(비학위 교육/수료) 목록. 최신순 내림차순 정렬.",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "교육명칭. 한국어로 저장."
},
"gov": {
"type": "string",
"description": "관련기관. 정보가 없으면 빈값('')."
},
"year1": {
"type": "string",
"description": "과정시작년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
},
"year2": {
"type": "string",
"description": "과정종료년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
}
},
"required": [ "name", "year1" ]
}
},
"activities": {
"type": "array",
"description": "활동사항(사회/봉사활동) 목록. 최신순 내림차순 정렬.",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "활동명. 한국어로 저장."
},
"gov": {
"type": "string",
"description": "관련기관. 정보가 없으면 빈값('')."
},
"year1": {
"type": "string",
"description": "활동시작년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
},
"year2": {
"type": "string",
"description": "활동종료년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
}
},
"required": [ "name", "year1" ]
}
},
"overseas": {
"type": "array",
"description": "해외활동(어학연수, 교환학생 등) 목록. 최신순 내림차순 정렬.",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "활동명. 한국어로 저장."
},
"gov": {
"type": "string",
"description": "관련기관. 정보가 없으면 빈값('')."
},
"year1": {
"type": "string",
"description": "활동시작년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
},
"year2": {
"type": "string",
"description": "활동종료년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
}
},
"required": [ "name", "year1" ]
}
},
"skills": {
"type": "array",
"description": "업무스킬 목록. 유사한 기술은 한 항목으로 묶어서 저장. 예) 'MS Office(Word, Excel, PowerPoint)'.",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "스킬명."
},
"desc": {
"type": "string",
"description": "숙련도 (상/중/하) 또는 세부 내용. 정보가 없으면 빈값('')."
}
},
"required": [ "name" ]
}
},
"languages": {
"type": "array",
"description": "언어능력 목록 (영어, 중국어, 일본어 등).",
"items": {
"type": "object",
"properties": {
"language": {
"type": "string",
"description": "언어종류. 한국어로 저장. 예) '영어', '중국어', '일본어'."
},
"level": {
"type": "string",
"description": "숙련도(상/중/하) 또는 시험 성적. 예) 'TOEIC 900 (2024.07)'."
}
},
"required": [ "language", "level" ]
}
},
"etcs": {
"type": "array",
"description": "기타사항 목록. 병역사항(예: '군필', '미필', '면제') 등 위 항목 어디에도 해당하지 않는 내용만 저장. 최신순 내림차순 정렬. 한국어로 저장.\n[절대 포함 금지] 경력사항, 총 경력 기간, 현재연봉, 희망연봉, 연봉 관련 내용, 입사 가능일, 연락처(이메일·전화번호), 자기소개 내용은 절대로 이 항목에 포함하지 마세요.",
"items": {
"type": "string"
}
}
}

[EXTRACTION EXCLUSION]
경력사항(work experience), 자기소개(self-introduction), 현재연봉, 희망연봉, 연락처(이메일·전화번호)는 추출하지 않습니다.
`;


// ───────────────────────────────────────────────────────────────
//  KR · OC (경력사항 전용)
// ───────────────────────────────────────────────────────────────
const gpt4_kr_career_only_prompt_base = `
[ACTION]
Convert the resume content into JSON format based on the following 'JSON structure description'.

[CRITICAL LANGUAGE RULE]
All output TEXT VALUES must be written in Korean (한국어), regardless of the language of the source resume.
Even if the resume is entirely in English, extract and translate all content into Korean before saving.
Job titles, department names, task descriptions, achievement details — everything must be in Korean.
Company names and proper nouns may remain in their original language if no Korean equivalent exists.
Do NOT output English sentences or phrases as values.

[EXTRACTION EXCLUSION]
자기소개, 현재연봉, 희망연봉, 연락처(이메일·전화번호)는 추출하지 않습니다.

[JSON structure description]
{
"career": {
"type": "array",
"description": "경력사항 목록. 최신순 내림차순 정렬. 내용은 절대 요약하지 않고 원문 그대로 사용. 한국어로 저장.",
"items": {
"type": "object",
"properties": {
"company": {
"type": "string",
"description": "회사명."
},
"info": {
"type": "string",
"description": "회사 정보(업종, 매출, 사원수 등). 이력서에 기재된 경우 한국어로 그대로 저장. 없으면 빈값('')."
},
"area": {
"type": "string",
"description": "회사 소재지. 시 단위까지만 저장. 정보가 없으면 빈값('')."
},
"dept": {
"type": "string",
"description": "부서명. 한국어로 저장. 정보가 없으면 빈값('')."
},
"pos": {
"type": "string",
"description": "직급/직책. 한국어로 저장. 정보가 없으면 빈값('')."
},
"j_yyyymm": {
"type": "string",
"description": "입사년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식)."
},
"r_yyyymm": {
"type": "string",
"description": "퇴사년월 (YYYY.MM 형식. 월 정보가 없으면 YYYY 형식). 재직 중인 경우 '현재'로 저장."
},
"r_reason": {
"type": "string",
"description": "퇴사사유. 기재된 경우 한국어로 저장. 없으면 빈값('')."
},
"desc": {
"type": "array",
"description": "경력 상세. 기재된 업무내용/성과/역할 등을 한국어로 저장. 절대 요약하거나 변형하지 않음. 머리기호(Bullet point) 제거. 최대 4depth 구조.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "1레벨 내용."
},
"depth": {
"type": "array",
"description": "2레벨 목록.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "2레벨 내용."
},
"depth": {
"type": "array",
"description": "3레벨 목록.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "3레벨 내용."
},
"depth": {
"type": "array",
"description": "4레벨 목록.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "4레벨 내용."
}}}}}}}}}}}}}},
"required": [ "company", "dept", "pos", "desc" ]
}}}
`;


// ───────────────────────────────────────────────────────────────
//  EN · NC (기본정보 — 경력 제외, 영문 출력)
// ───────────────────────────────────────────────────────────────
const gpt4_en_no_career_prompt_base = `
[ACTION]
Convert the resume content into JSON format based on the following 'JSON structure description'.

[CRITICAL LANGUAGE RULE]
All output TEXT VALUES must be written in English, regardless of the language of the source resume.
Even if the resume is entirely in Korean, extract and translate all content into English before saving.
Proper nouns (school names, company names, certification names) may be kept in their original language only if no English equivalent exists.
Do NOT output Korean sentences or phrases as values — translate them into English.

[JSON structure description]
{
"candidate": {
"type": "string",
"description": "Full name of the candidate as written on the resume."
},
"yob": {
"type": "string",
"description": "Date of Birth. If full date: 'YYYY.MM.DD'. If month only: 'YYYY.MM'. If year only: 'YYYY'. If not available, leave blank."
},
"gender": {
"type": "string",
"description": "Gender. Save as 'Male' or 'Female'. If not available, leave blank."
},
"addr": {
"type": "string",
"description": "Residential address. Save up to city/district level only. e.g., 'Gangnam-gu, Seoul' or 'Suwon, Gyeonggi-do'. If not available, leave blank."
},
"core": {
"type": "array",
"description": "Core competency list. Translate to English and save each item. If none, empty array.",
"items": {
"type": "string"
}
},
"education": {
"type": "array",
"description": "List of educational qualifications (descending order, most recent first).",
"items": {
"type": "object",
"properties": {
"school": {
"type": "string",
"description": "Name of the school. Use English name if available."
},
"major": {
"type": "string",
"description": "Major/field of study in English."
},
"grade": {
"type": "string",
"description": "GPA (e.g., '4.0/4.5'). Leave blank if not available."
},
"area": {
"type": "string",
"description": "Location of the school (city name only, e.g., 'Seoul', 'Busan'). Leave blank if not available."
},
"degree": {
"type": "string",
"description": "Degree type: 'Bachelor', 'Master', 'Doctor', etc. Leave blank if not available."
},
"is_grdt": {
"type": "string",
"description": "Graduation status: 'Graduated', 'Completed', 'Enrolled', 'Dropped Out', 'Leave of Absence'. Leave blank if not available."
},
"ad_yyyymm": {
"type": "string",
"description": "Admission date (YYYY.MM format. If no month, YYYY format)."
},
"gdt_yyyymm": {
"type": "string",
"description": "Graduation date (YYYY.MM format. If no month, YYYY format)."
}
},
"required": [ "school", "major", "gdt_yyyymm" ]
}
},
"certifications": {
"type": "array",
"description": "List of certifications (descending order, most recent first).",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Name of the certification in English."
},
"gov": {
"type": "string",
"description": "Issuing institution. Leave blank if not available."
},
"year": {
"type": "string",
"description": "Date obtained (YYYY.MM format. If no month, YYYY format)."
}
},
"required": [ "name", "year" ]
}
},
"awards": {
"type": "array",
"description": "List of awards (descending order, most recent first).",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Name of the award in English."
},
"gov": {
"type": "string",
"description": "Awarding institution. Leave blank if not available."
},
"year": {
"type": "string",
"description": "Date received (YYYY.MM format. If no month, YYYY format)."
}
},
"required": [ "name", "year" ]
}
},
"learns": {
"type": "array",
"description": "List of non-academic training/courses completed (descending order, most recent first).",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Course/training name in English."
},
"gov": {
"type": "string",
"description": "Institution. Leave blank if not available."
},
"year1": {
"type": "string",
"description": "Start date (YYYY.MM format. If no month, YYYY format)."
},
"year2": {
"type": "string",
"description": "End date (YYYY.MM format. If no month, YYYY format)."
}
},
"required": [ "name", "year1" ]
}
},
"activities": {
"type": "array",
"description": "List of volunteer or social activities (descending order, most recent first).",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Activity name in English."
},
"gov": {
"type": "string",
"description": "Related institution. Leave blank if not available."
},
"year1": {
"type": "string",
"description": "Start date (YYYY.MM format. If no month, YYYY format)."
},
"year2": {
"type": "string",
"description": "End date (YYYY.MM format. If no month, YYYY format)."
}
},
"required": [ "name", "year1" ]
}
},
"overseas": {
"type": "array",
"description": "List of overseas experiences (e.g., language training, exchange student) (descending order, most recent first).",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Activity name in English."
},
"gov": {
"type": "string",
"description": "Related institution. Leave blank if not available."
},
"year1": {
"type": "string",
"description": "Start date (YYYY.MM format. If no month, YYYY format)."
},
"year2": {
"type": "string",
"description": "End date (YYYY.MM format. If no month, YYYY format)."
}
},
"required": [ "name", "year1" ]
}
},
"skills": {
"type": "array",
"description": "List of skills. Group similar skills together (e.g., 'MS Office (Word, Excel, PowerPoint)').",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Skill name."
},
"desc": {
"type": "string",
"description": "Proficiency level (High/Medium/Low) or description. Leave blank if not available."
}
},
"required": [ "name" ]
}
},
"languages": {
"type": "array",
"description": "List of language proficiencies.",
"items": {
"type": "object",
"properties": {
"language": {
"type": "string",
"description": "Language name in English (e.g., 'English', 'Korean', 'Chinese', 'Japanese')."
},
"level": {
"type": "string",
"description": "Proficiency level (High/Medium/Low) or test score (e.g., 'TOEIC 900 (2024.07)')."
}
},
"required": [ "language", "level" ]
}
},
"etcs": {
"type": "array",
"description": "Other miscellaneous information that does not belong to any of the above categories (e.g., military service status). Save in English, descending order.\n[STRICTLY EXCLUDE] Work experience, total career duration, current salary, desired salary, any salary-related content, available start date, contact information (email, phone number), and self-introduction must NOT be included here.",
"items": {
"type": "string"
}
}
}

[EXTRACTION EXCLUSION]
Do NOT extract: work experience (career), self-introduction, current salary, desired salary, contact information (email, phone number).
`;


// ───────────────────────────────────────────────────────────────
//  EN · OC (경력사항 전용, 영문 출력)
// ───────────────────────────────────────────────────────────────
const gpt4_en_career_only_prompt_base = `
[ACTION]
Convert the resume content into JSON format based on the following 'JSON structure description'.

[CRITICAL LANGUAGE RULE]
All output TEXT VALUES must be written in English, regardless of the language of the source resume.
Even if the resume is entirely in Korean, extract and translate all content into English before saving.
Job titles, department names, task descriptions, achievement details — everything must be in English.
Company names may remain in their original language if no English equivalent exists.
Do NOT output Korean sentences or phrases as values.

[EXTRACTION EXCLUSION]
Do NOT extract: self-introduction, current salary, desired salary, contact information (email, phone number).

[JSON structure description]
{
"career": {
"type": "array",
"description": "List of work experiences (descending order, most recent first). Translate all content to English. Do not summarize — use every detail from the resume.",
"items": {
"type": "object",
"properties": {
"company": {
"type": "string",
"description": "Company name."
},
"info": {
"type": "string",
"description": "Company information (industry, revenue, headcount etc.) in English. If available, save as is (translated). If not, leave blank."
},
"area": {
"type": "string",
"description": "Company location (city level only). Leave blank if not available."
},
"dept": {
"type": "string",
"description": "Department name in English. Leave blank if not available."
},
"pos": {
"type": "string",
"description": "Job title/position in English. Leave blank if not available."
},
"j_yyyymm": {
"type": "string",
"description": "Joining date (YYYY.MM format. If no month, YYYY format)."
},
"r_yyyymm": {
"type": "string",
"description": "Resignation date (YYYY.MM format. If no month, YYYY format). If still employed, save as 'Present'."
},
"r_reason": {
"type": "string",
"description": "Reason for leaving in English. Leave blank if not available."
},
"desc": {
"type": "array",
"description": "Work experience details. Translate to English. Do not summarize. Remove bullet point symbols. Up to 4 depth levels.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "1st level description."
},
"depth": {
"type": "array",
"description": "2nd level list.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "2nd level description."
},
"depth": {
"type": "array",
"description": "3rd level list.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "3rd level description."
},
"depth": {
"type": "array",
"description": "4th level list.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "4th level description."
}}}}}}}}}}}}}},
"required": [ "company", "dept", "pos", "desc" ]
}}}
`;