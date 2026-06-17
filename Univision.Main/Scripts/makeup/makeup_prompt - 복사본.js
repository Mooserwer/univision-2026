const gpt5_kr_no_career_prompt_base = `[수행내용]
이력서 내용을 **한글로 변환** 해서 주세요.
절대 추측하지 않고 이력서에 있는 **글자/단어/문장 그대로 사용**하여 내용을 추출합니다.
** 단, 눈에 보이지 않는 특수공백 및 불필요한 특수문자, 불필요한 공백은 제거. **
기간 정보는 YYYY.MM.DD 형식으로 저장 합니다. 예) 2025년1월1일 일 경우 2025.01.01
일 정보가 없는 경우 YYYY.MM 형식의 연월, 월 정보가 없는 경우 YYYY형식의 년도 정보로 저장합니다.

[추출항목]
* 이름 : 이름이 3글자 이면 글자 사이에 공백을 추가. 예) 이름 '홍길동' 일 경우 '홍 길 동' 으로 저장.

* 생년(월일) : YYYY.MM.DD 형식. 만약 년/월 정보만 있으면 YYYY.MM 형식으로 저장. 또 만약 년 정보만 있으면 YYYY 형식으로 저장. 생년 정보가 없으면 빈값('') 으로 저장.,

* 성별 : 만약 정보가 없으면 빈값('')으로 저장.,

* 거주지 주소: 만약 정보가 없으면 빈값('')으로 저장. 시/구 까지만 저장. 예1)'서울시 강남구 테헤란로 36...' 일 경우 '서울시 강남구'로 저장. 예2) '경기도 화성시 동탄'의 경우 '경기도 화성시'로 저장.,

* 핵심역량 : 이력서에 기재된 핵심역량 단어 또는 문장단위 배열 저장. 핵심역량이 기재되어있을 경우 이력서에 기재된 핵심역량 내용의 모든 글자 그대로 저장하되 최소한 양식 띄어쓰기 정도는 수정. 기재된 내용이 없을 경우 빈값('')으로 저장.,

* 학력사항 : 최종 최신 학력 부터 내림차순 정렬. 학위 사항만 추출.
[  - 학교명 : 기재된 학교이름,
  - 지역 : 기재된 학교의 위치 기재된 지역이 없으면 빈값('')으로 저장. 만약 '서울특별시', '대전광역시' 등으로 기재되어있으면 줄여서 '서울', '대전' 형식으로 저장.,
  - 전공 : 기재된 전공명,
  - 학위 : 학사/석사/박사 와 같은 학위 정보가 없으면 빈값('')으로 저장.,
  - 학점 : 만약 이력서 내 학점 정보가 없으면 빈값('')으로 저장. 정보가 있는 경우 '4.0/4.5' 와 같은 양식으로 저장,
  - 졸업여부 : 졸업/수료/재학중/중퇴/휴학 과 같은 졸업 구분.  기재된 내용이 없으면 빈값('')으로 저장.,
  - 입학년월: YYYY.MM 형식. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.,
  - 졸업년월: YYYY.MM 형식. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.],

* 자격사항(자격증) : 각종 자격증 정보. 최종 최신 자격사항 부터 내림차순 정렬. 예)컴퓨터활용능력, 유통관리사, AICPA, TOEIC.[
  - 자격명 : 기재된 자격(증)명,
  - 발급기관 : 내용이 없으면 빈값('')으로 저장.,
  - 취득년월 : YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.],

* 수상내역 : 각종 수상내역. 최종 최신 수상내역 부터 내림차순 정렬.[
  - 수상명칭 : 기재된 수상명칭,
  - 관련기관 : 내용이 없으면 빈값('')으로 저장.,
  - 수상년월 : YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.],

* 수료사항 : 비학위 과정 교육/수료 내역. 최신 사항 부터 내림차순 정렬.[
  - 교육명칭 : 기재된 교육명칭,
  - 관련기관 : 내용이 없으면 빈값('')으로 저장.,
  - 과정시작년월 : YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.,
  - 과정종료년월 : YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.],

* 활동사항 : 각종 사회활동 및 봉사활동 내역. 최신 사항 부터 내림차순으로 정렬.[
  - 활동명 : 기재된 활동명,
  - 관련기관 : 내용이 없으면 빈값('')으로 저장.,
  - 활동시작년월 : YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.,
  - 활동종료년월 : YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.],

* 해외활동 : 각종 해외 활동. 최신 활동 부터 내림차순으로 정렬. 예)어학연수, 교환학생 등.[
  - 활동명 : 기재된 활동명,
  - 관련기관 : 내용이 없으면 빈값('')으로 저장.,
  - 활동시작년월 : YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.,
  - 활동종료년월 : YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.],

* 업무스킬 : 각종 업무 기술 능력. 유사한 기술은 한줄로 나열. 예) MS Office(Word, Excel, Power Point), Adobe(Photoshop, Illustrator, Premiere, AfterEffect) 툴 사용 능력 등.  [
  - 스킬종류 : 기재된 스킬종류,
  - 스킬숙련도 : 상/중/하 또는 세부 서술한 내용 저장. 만약 정보가 없으면 빈값('')으로 저장.],

* 언어능력 : 각종 어학능력(영어, 중국어, 스페인어, 독일어, 일본어 등 기재된 내용).[
  - 언어종류 : 기재된 스킬종류,
  - 언어숙련도 : 상/중/하 또는 관련시험 성적이 기술되어있을 경우 해당 내용을 저장. 시험 성적일 경우 'TOEIC 900 (YYYY.MM)' 형식.],

* 기타사항 : 병역사항 등 이력서에 기재된 기타사항. 최신 사항 부터 내림차순 정렬. [
  - 내용 : 기재된 내용 줄단위]

* 자기소개 : 기재된 자기소개 원문 그대로 저장.
[추출금지항목]
경력사항, 자기소개, 현재연봉, 희망연봉, 연락처정보(이메일, 전화번호)`;

const gpt5_kr_career_only_prompt_base = `
이력서 내용 중 경력사항이 서술된 부분을 **한글로 변환** 해서 주세요.
절대 추측하지 않고 이력서에 있는 **글자/단어/문장 그대로 사용**하여 내용을 추출합니다.
** 단, 눈에 보이지 않는 특수공백 및 불필요한 특수문자, 불필요한 공백은 제거. **
기간 정보는 YYYY.MM.DD 형식으로 저장 합니다. 예) 2025년1월1일 일 경우 2025.01.01
일 정보가 없는 경우 YYYY.MM 형식의 연월, 월 정보가 없는 경우 YYYY형식의 년도 정보로 저장합니다.

[추출금지항목]
자기소개, 현재연봉, 희망연봉, 연락처정보(이메일, 전화번호)

[추출항목]
* 경력 사항 : 기재된 경력 사항. 최신경력 부터 내림차순 정렬. [
 - 회사명 : 기재된 회사명.,
 - 회사정보 : 기재되어 있는 산업, 사원수, 매출 등 이력서에 기재되어 있는 경우 그대로 저장. 정보가 없는경우 빈값('')으로 저장.,
 - 회사위치 : 기재되어 있는 회사의 위치. 정보가 없는경우 빈값('')으로 저장.,
 - 부서 : 기재되어 있는 업무 부서. 정보가 없는경우 빈값('')으로 저장.,
 - 직급 : 기재되어 있는 직급. 정보가 없는경우 빈값('')으로 저장.,
 - 입사년월 : YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장.,
 - 퇴사년월 : YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장. 만약 재직 중인 경우 '현재'로 저장,
 - 퇴사사유 : 퇴사사유가 기재되어있는 경우 그대로 저장. 정보가 없는경우 빈값('')으로 저장.
 - 경력상세 : 기재되어있는 경력세부사항(기여도/주요업무/성과/역할 등 기재된 내용. **절대 요약, 변형 금지**. 서술된 **글자/단어/문장 그대로 사용**. 최대 4 depth 방식으로 줄단위 저장. [
  -- 1레벨 내용 : 서술된 내용 줄단위(머리기호(Bullet point) 제거) [
   --- 2레벨 내용 : 서술된 내용 줄단위(머리기호(Bullet point) 제거) [
    --- 3레벨 내용 : 서술된 내용 줄단위(머리기호(Bullet point) 제거) [
     --- 4레벨 내용 : 서술된 내용 줄단위(머리기호(Bullet point) 제거)
    ]
   ]
  ]     
 ]
]
`;


const gpt4_kr_no_career_prompt_base = `
[ACTION]
Convert the resume content into JSON format based on the following 'JSON structure description'

[추출금지항목 - 어떤 필드에도 절대 포함하지 말 것]
- 경력사항 (career 항목): 회사명, 재직기간, 직급, 담당업무 등 일체
- 연봉/급여 정보: 현재연봉, 희망연봉, 급여 등
- 연락처: 이메일, 전화번호, 주소 상세
- 입사가능일
- 자기소개/지원동기
※ 위 항목이 이력서에 있더라도 etcs를 포함한 어느 필드에도 넣지 말 것

[JSON structure description]
{
"candidate": {
"type": "string",
"description": "Name of the candidate: (If the name is in Korean, include spaces between syllables - e.g., if the name is '홍길동', save it as '홍 길 동')."
},
"yob": {
"type": "string",
"description": "Date of Birth: If the full date of birth is listed, save it in the format 'YYYY.MM.DD'. If only the month is listed, save it as 'YYYY.MM'. If only the year is listed, save it as 'YYYY'. If there is no information, leave it blank."
},
"gender": {
"type": "string",
"description": "Gender: If the gender is listed, save it as '남성' or '여성'. If there is no information, leave it blank."
},
"addr": {
"type": "string",
"description": "Residence Address: If the candidate's residence address is listed, only include up to the city/district. If the residence address is not listed, leave it blank. - e.g., if the address is '서울시 강남구 테헤란로 36...', save as '서울시 강남구'. If more details are available, they can be included)."
},
"core": {
"type": "array",
"description": "Core Competency List: If core competencies are listed, Use the original text as is and separate it with line breaks. If there are no core competencies listed, leave it blank.",
"items": {
"type": "string"
}
},
"education": {
"type": "array",
"description": "List of educational qualifications (save in descending order, starting from the most recent).",
"items": {
"type": "object",
"properties": {
"school": {
"type": "string",
"description": "Name of the school."
},
"major": {
"type": "string",
"description": "Major."
},
"grade": {
"type": "string",
"description": "grade point average : If there are no GPA, leave it blank. - e.g., '4.0/4.5'"
},
"area": {
"type": "string",
"description": "Region of the school listed on the resume : If there is no text, leave it blank. If specified, only save up to the city name, excluding words like '특별시' or '광역시' so it is saved as '서울' '수원' etc."
},
"degree": {
"type": "string",
"description": "Degree : the degree information such as '학사', '석사', '박사'. If there is no text, leave it blank.)."
},
"is_grdt": {
"type": "string",
"description": "Graduation Status: Indicate whether it is '졸업' or '수료'. If there is no text, leave it blank."
},
"ad_yyyymm": {
"type": "string",
"description": "Admission date (Save in YYYY.MM format. If there is no Month, save in YYYY format)."
},
"gdt_yyyymm": {
"type": "string",
"description": "Graduation date (Save in YYYY.MM format. If there is no Month, save in YYYY format)."
}
},
"required": [ "school", "major", "gdt_yyyymm" ]
}
},
"certifications": {
"type": "array",
"description": "List of certifications (e.g., Computer Literacy Certification, Distribution Manager, AICPA, TOEIC). (save in descending order, starting from the most recent)",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Name of the certification."
},
"gov": {
"type": "string",
"description": "Issuing institution."
},
"year": {
"type": "string",
"description": "Date of certification (Save in YYYY.MM format. If there is no Month, save in YYYY format)."
}
},
"required": [ "name", "year" ]
}
},
"awards": {
"type": "array",
"description": "List of awards. (save in descending order, starting from the most recent)",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Name of the awards."
},
"gov": {
"type": "string",
"description": "Issuing institution."
},
"year": {
"type": "string",
"description": "Date of awards(Save in YYYY.MM format. If there is no Month, save in YYYY format)."
}
},
"required": [ "name", "year" ]
}
},
"learns": {
"type": "array",
"description": "List of non-academic education/training. (save in descending order, starting from the most recent)",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Name of the course."
},
"gov": {
"type": "string",
"description": "Institution providing the training."
},
"year1": {
"type": "string",
"description": "Start date of the training (Save in YYYY.MM format. If there is no Month, save in YYYY format)."
},
"year2": {
"type": "string",
"description": "End date of the training (Save in YYYY.MM format. If there is no Month, save in YYYY format)."
}
},
"required": [ "name", "year1" ]
}
},
"activities": {
"type": "array",
"description": "List of Volunteer or Social activities. (save in descending order, starting from the most recent)",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Name of the activities."
},
"gov": {
"type": "string",
"description": "Institution providing the activities."
},
"year1": {
"type": "string",
"description": "Start date of the activities(Save in YYYY.MM format. If there is no Month, save in YYYY format)."
},
"year2": {
"type": "string",
"description": "End date of the activities(Save in YYYY.MM format. If there is no Month, save in YYYY format)."
}
},
"required": [ "name", "year1" ]
}
},
"overseas": {
"type": "array",
"description": "List of Overseas experience e.g. language training, Exchange student(save in descending order, starting from the most recent)",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Name of the experience."
},
"gov": {
"type": "string",
"description": "Institution providing the experience."
},
"year1": {
"type": "string",
"description": "Start date of the experience(Save in YYYY.MM format. If there is no Month, save in YYYY format)."
},
"year2": {
"type": "string",
"description": "End date of the experience(Save in YYYY.MM format. If there is no Month, save in YYYY format)."
}
},
"required": [ "name", "year1" ]
}
},
"skills": {
"type": "array",
"description": "List of skills (list similar skills in a string, e.g., MS Office(Word, Excel, Power Point) proficiency, Adobe(Photoshop, Illustrator, Premiere, AfterEffect) tool usage).",
"items": {
"type": "object",
"properties": {
"name": {
"type": "string",
"description": "Name of the skill."
},
"desc": {
"type": "string",
"description": "Proficiency level (상/중/하) or detailed description if available. If not, save as '')."
}
},
"required": [ "name" ]
}
},
"languages": {
"type": "array",
"description": "List of language proficiencies.(English Spanish Chinese Japanese etc.)",
"items": {
"type": "object",
"properties": {
"language": {
"type": "string",
"description": "Name of the language."
},
"level": {
"type": "string",
"description": "Proficiency level (상/중/하) or test scores if available, e.g., TOEIC 900 (2024.07)."
}
},
"required": [ "language", "level" ]
}
},
"etcs": {
"type": "array",
"description": "List of other information (e.g., military service details). Do not include information related to total experience period, annual salary, or available date of employment.",
"items": {
"type": "string"
}
}
}
`;

const gpt4_kr_career_only_prompt_base = `
[ACTION]
Convert the resume content into JSON format based on the following 'JSON structure description'

[JSON structure description]
{
"career": {
"type": "array",
"description": "List of work experiences (save in descending order, starting from the most recent. no summarize. use every single word about work experience ).",
"items": {
"type": "object",
"properties": {
"company": {
"type": "string",
"description": "Name of the company."
},
"info": {
"type": "string",
"description": "Company information (industry, revenue, number of employees). If available in the resume, save as it is. If not, save as '')."
},
"area": {
"type": "string",
"description": "Region of the company listed on the resume : If not specified, leave it blank. If specified, only save up to the city name"
},
"dept": {
"type": "string",
"description": "Department."
},
"pos": {
"type": "string",
"description": "Position at the time of employment."
},
"j_yyyymm": {
"type": "string",
"description": "Joining date (Save in YYYY.MM format. If there is no Month, save in YYYY format)."
},
"r_yyyymm": {
"type": "string",
"description": "Resignation date (Save in YYYY.MM format. If still employed, save as '현재'. If there is no Month, save in YYYY format)."
},
"r_reason": {
"type": "string",
"description": "Reason for resignation: If available, save it. If not, save as '')."
},
"desc": {
"type": "array",
"description": "List of responsibilities/major tasks/achievements/role/details - Use all text about work experience in its original form. Do not summarize. use every single word about details. just extract text. if there is bullet point, remove bullet point",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "1st level description paragraph."
},
"depth": {
"type": "array",
"description": "2nd level list.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "2nd level description paragraph (sub-description of the 1st level)."
},
"depth": {
"type": "array",
"description": "3rd level list.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "3rd level description paragraph (sub-description of the 2nd level)."
},
"depth": {
"type": "array",
"description": "4th level list.",
"items": {
"type": "object",
"properties": {
"desc": {
"type": "string",
"description": "4th level description paragraph (sub-description of the 3rd level)."
}}}}}}}}}}}}}},
"required": [ "company", "dept", "pos", "desc" ]
}}}`;