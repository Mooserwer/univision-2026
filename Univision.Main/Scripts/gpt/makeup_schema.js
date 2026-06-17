/// <reference path="makeup_prompt.js" />

// ── 핵심역량(core)·업무상세(desc) 공용 4-depth 계층 항목 구조 ──
const gpt5_depth4_desc_items = {
  "type": "object",
  "properties": {
    "desc": { "type": "string", "description": "1st level description paragraph." },
    "depth": {
      "type": "array",
      "description": "2nd level list. 하위 항목이 없으면 빈 배열.",
      "items": {
        "type": "object",
        "properties": {
          "desc": { "type": "string", "description": "2nd level description paragraph" },
          "depth": {
            "type": "array",
            "description": "3rd level list. 하위 항목이 없으면 빈 배열.",
            "items": {
              "type": "object",
              "properties": {
                "desc": { "type": "string", "description": "3rd level description paragraph" },
                "depth": {
                  "type": "array",
                  "description": "4th level list. 하위 항목이 없으면 빈 배열.",
                  "items": {
                    "type": "object",
                    "properties": {
                      "desc": { "type": "string", "description": "4th level description paragraph" }
                    },
                    "required": ["desc"],
                    "additionalProperties": false
                  }
                }
              },
              "required": ["desc", "depth"],
              "additionalProperties": false
            }
          }
        },
        "required": ["desc", "depth"],
        "additionalProperties": false
      }
    }
  },
  "required": ["desc", "depth"],
  "additionalProperties": false
};

const gpt5_nc_schema_simple = {
  "type": "object",
  "properties": {
    "candidate": {
      "type": "string",
      "description": "후보자의 이름."
    },
    "yob": {
      "type": "string",
      "description": "생년월일."
    },
    "gender": {
      "type": "string",
      "enum": [
        "남성",
        "여성",
        "Male",
        "Female",
        ""
      ],
      "description": "성별 정보."
    },
    "addr": {
      "type": "string",
      "description": "거주지 주소."
    },
    "core": {
      "type": "array",
      "description": "핵심역량",
      "items": {
        "type": "string"
      }
    },
    "self_intro": {
      "type": "string",
      "description": "자기소개"
    },
    "education": {
      "type": "array",
      "description": "학력 사항 목록",
      "items": {
        "type": "object",
        "properties": {
          "school": {
            "type": "string",
            "description": "기재된 학교이름"
          },
          "major": {
            "type": "string",
            "description": "기재된 전공"
          },
          "grade": {
            "type": "string",
            "description": "학점정보."
          },
          "area": {
            "type": "string",
            "description": "기재된 학교의 위치"
          },
          "degree": {
            "type": "string",
            "enum": [
              "학사",
              "석사",
              "박사",
              "전문학사",
              "박사후연구원",
              ""
            ],
            "description": "학위."
          },
          "is_grdt": {
            "type": "string",
            "enum": [
              "졸업",
              "수료",
              "재학중",
              "졸업예정",
              "중퇴",
              "휴학",
              ""
            ],
            "description": "졸업 구분."
          },
          "ad_yyyymm": {
            "type": "string",
            "description": "입학년월."
          },
          "gdt_yyyymm": {
            "type": "string",
            "description": "졸업년월."
          }
        },
        "additionalProperties": false,
        "required": [
          "school",
          "major",
          "grade",
          "area",
          "degree",
          "is_grdt",
          "ad_yyyymm",
          "gdt_yyyymm"
        ]
      }
    },
    "certifications": {
      "type": "array",
      "description": "각종 자격증 정보.",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "자격사항 또는 자격증이름."
          },
          "gov": {
            "type": "string",
            "description": "발급기관."
          },
          "year": {
            "type": "string",
            "description": "취득년월 정보."
          }
        },
        "additionalProperties": false,
        "required": [
          "name",
          "gov",
          "year"
        ]
      }
    },
    "awards": {
      "type": "array",
      "description": "각종 수상내역.",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "수상 명칭."
          },
          "gov": {
            "type": "string",
            "description": "발급기관."
          },
          "year": {
            "type": "string",
            "description": "수상년월."
          }
        },
        "additionalProperties": false,
        "required": [
          "name",
          "gov",
          "year"
        ]
      }
    },
    "learns": {
      "type": "array",
      "description": "비학위 과정 교육/수료 내역.",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "과정명칭"
          },
          "gov": {
            "type": "string",
            "description": "관련기관."
          },
          "year1": {
            "type": "string",
            "description": "과정시작년월."
          },
          "year2": {
            "type": "string",
            "description": "과정종료년월."
          }
        },
        "additionalProperties": false,
        "required": [
          "name",
          "gov",
          "year1",
          "year2"
        ]
      }
    },
    "activities": {
      "type": "array",
      "description": "각종 사회활동 및 봉사활동 내역.",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "활동명"
          },
          "gov": {
            "type": "string",
            "description": "관련기관."
          },
          "year1": {
            "type": "string",
            "description": "활동 시작 년월."
          },
          "year2": {
            "type": "string",
            "description": "활동 종료 년월."
          }
        },
        "additionalProperties": false,
        "required": [
          "name",
          "gov",
          "year1",
          "year2"
        ]
      }
    },
    "overseas": {
      "type": "array",
      "description": "각종 해외 활동.",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "활동 명칭."
          },
          "gov": {
            "type": "string",
            "description": "관련기관."
          },
          "year1": {
            "type": "string",
            "description": "활동 시작 년월."
          },
          "year2": {
            "type": "string",
            "description": "활동 종료 년월."
          }
        },
        "additionalProperties": false,
        "required": [
          "name",
          "gov",
          "year1",
          "year2"
        ]
      }
    },
    "skills": {
      "type": "array",
      "description": "각종 업무 기술 능력.",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "업무 기술 능력."
          },
          "desc": {
            "type": "string",
            "description": "업무 숙련도."
          }
        },
        "additionalProperties": false,
        "required": [
          "name",
          "desc"
        ]
      }
    },
    "languages": {
      "type": "array",
      "description": "어학능력",
      "items": {
        "type": "object",
        "properties": {
          "language": {
            "type": "string",
            "description": "언어명"
          },
          "level": {
            "type": "string",
            "description": "언어 숙련도"
          }
        },
        "additionalProperties": false,
        "required": [
          "language",
          "level"
        ]
      }
    },
    "etcs": {
      "type": "array",
      "description": "병역사항·논문 등 순수 기타사항만 포함. 다음은 절대 포함 금지: 자기소개(→selfintro), 현재/희망연봉, 입사가능시기, 주야간/근무형태, 주민등록상 주소(→addr), 경력 총 기간 합계, 연락처/이메일/전화번호.",
      "items": {
        "type": "string"
      }
    },
    "selfintro": {
      "type": "array",
      "description": "자기소개(자기소개서) 전문. 이력서에 기재된 내용을 그대로 복사. 없으면 빈 배열.",
      "items": {
        "type": "string"
      }
    }
  },
  "required": [
    "candidate",
    "yob",
    "gender",
    "addr",
    "core",
    "education",
    "certifications",
    "awards",
    "learns",
    "activities",
    "overseas",
    "skills",
    "languages",
    "etcs",
    "selfintro"
  ],
  "additionalProperties": false
}

const gpt5_oc_schema_simple = {
  "type": "object",
  "properties": {
    "career": {
      "type": "array",
      "description": "경력사항",
      "items": {
        "type": "object",
        "properties": {
          "company": { "type": "string", "description": "회사명" },
          "info": {
            "type": "array",
            "description": "회사의 정보(소개). 기재되어있을 경우 문장 단위 string 배열로 추출하고 원본과 동일한 문단 구조를 유지. 이력서에 명시되어있지 않으면 빈 배열.",
            "items": { "type": "string" }
          },
          "area": { "type": "string", "description": "회사지역 (기재되어있을 경우)" },
          "dept": { "type": "string", "description": "근무부서" },
          "pos": { "type": "string", "description": "근무직급" },
          "j_yyyymm": { "type": "string", "description": "입사년월" },
          "r_yyyymm": { "type": "string", "description": "퇴사년월" },
          "r_reason": { "type": "string", "description": "퇴사사유" },
          "desc": {
            "type": "array",
            "description": "업무상세 (문장단위). 경력사항에 기재된 주요 업무 성과(성과/실적/주요성과 등)도 빠짐없이 포함.",
            "items": {
              "type": "object",
              "properties": {
                "desc": { "type": "string", "description": "1st level description paragraph." },
                "depth": {
                  "type": "array",
                  "description": "2nd level list.",
                  "items": {
                    "type": "object",
                    "properties": {
                      "desc": { "type": "string", "description": "2nd level description paragraph" },
                      "depth": {
                        "type": "array",
                        "description": "3rd level list.",
                        "items": {
                          "type": "object",
                          "properties": {
                            "desc": { "type": "string", "description": "3rd level description paragraph" },
                            "depth": {
                              "type": "array",
                              "description": "4th level list.",
                              "items": {
                                "type": "object",
                                "properties": {
                                  "desc": { "type": "string", "description": "4th level description paragraph" }
                                },
                                "required": ["desc"],
                                "additionalProperties": false
                              }
                            }
                          },
                          "required": ["desc", "depth"],
                          "additionalProperties": false
                        }
                      }
                    },
                    "required": ["desc", "depth"],
                    "additionalProperties": false
                  }
                }
              },
              "required": ["desc", "depth"],
              "additionalProperties": false
            }
          }
        },
        // Career 객체 내부의 필수 필드는 여기에 정의
        "required": ["company", "info", "area", "dept", "pos", "j_yyyymm", "r_yyyymm", "r_reason", "desc"],
        "additionalProperties": false
      }
    }
  },
  // 루트 객체(career를 담고 있는)의 필수 필드는 career 하나뿐입니다.
  "required": ["career"],
  "additionalProperties": false
}


const gpt5_nc_schema = {
  type: "object",
  properties: {
    candidate: {
      type: "string",
      description: "Name of the candidate: (If the name is in Korean, include spaces between syllables - e.g., if the name is '홍길동', save it as '홍 길 동')."
    },
    yob: {
      type: "string",
      description: "Date of Birth: If the full date of birth is listed, save it in the format 'YYYY.MM.DD'. If only the month is listed, save it as 'YYYY.MM'. If only the year is listed, save it as 'YYYY'. If there is no information, leave it blank."
    },
    gender: {
      type: "string",
      enum: ["남성", "여성", "Male", "Female", ""],
      description: "Gender: If the gender is listed, save it as 'Male' or 'Female'. If there is no information, leave it blank ('')."
    },
    addr: {
      type: "string",
      description: "Residence Address: If the candidate's residence address is listed, only include up to the city/district. If the residence address is not listed, leave it blank. - e.g., if the address is '서울시 강남구 테헤란로 36...', save as '서울시 강남구'. If more details are available, they can be included)."
    },
    core: {
      type: "array",
      description: "Core Competency List: If core competencies are listed, use the original text as is. Even without a 'core competency' heading, if the profile/summary area at the top of the resume contains a short bullet list summarizing strengths/capabilities, extract it as core (do NOT omit it). Short bullet-style strength list → core; narrative paragraph prose → selfintro. If the content has a hierarchical (indented/sub-item) structure, store it as a hierarchical structure up to 4 depth levels — same structure as the career desc field. Leave empty array if not present.",
      items: gpt5_depth4_desc_items
    },
    education: {
      type: "array",
      description: "List of educational qualifications (save in descending order, starting from the most recent).",
      items: {
        type: "object",
        properties: {
          school: {
            type: "string",
            description: "Name of the school."
          },
          major: {
            type: "string",
            description: "Major."
          },
          grade: {
            type: "string",
            description: "grade point average : If there are no GPA, leave it blank. - e.g., '4.0/4.5'"
          },
          area: {
            type: "string",
            description: "Region of the school listed on the resume. If there is no text, leave it blank (''). If specified, only save up to the city name — shorten '서울특별시' → '서울', '대전광역시' → '대전', etc."
          },
          degree: {
            type: "string",
            enum: ["학사", "석사", "박사", "전문학사", "박사후연구원", ""],
            description: "Degree: e.g., '학사', '석사', '박사', '전문학사', '박사후연구원'. If there is no information, leave it blank ('')."
          },
          is_grdt: {
            type: "string",
            enum: ["졸업", "수료", "재학중", "졸업예정", "중퇴", "휴학", ""],
            description: "Graduation status: '졸업', '수료', '재학중', '졸업예정', '중퇴', '휴학'. If there is no information, leave it blank ('')."
          },
          ad_yyyymm: {
            type: "string",
            "description": "Admission date (Save in YYYY.MM format. If there is no Month, save in YYYY format)."
          },
          gdt_yyyymm: {
            type: "string",
            description: "Graduation date (Save in YYYY.MM format. If there is no Month, save in YYYY format)."
          }
        },
        additionalProperties: false,
        required: [
          "school",
          "major",
          "grade",
          "area",
          "degree",
          "is_grdt",
          "ad_yyyymm",
          "gdt_yyyymm"
        ]
      }
    },
    certifications: {
      type: "array",
      description: "List of certifications (e.g., Computer Literacy Certification, Distribution Manager, AICPA, TOEIC). (save in descending order, starting from the most recent)",
      items: {
        type: "object",
        properties: {
          name: {
            type: "string",
            description: "Name of the certification."
          },
          gov: {
            type: "string",
            description: "Issuing institution. (If there is no text, leave it blank.)"
          },
          year: {
            type: "string",
            description: "Date of certification (Save in YYYY.MM format. If there is no Month, save in YYYY format)."
          }
        },
        additionalProperties: false,
        required: ["name", "gov", "year"]
      }
    },
    awards: {
      type: "array",
      description: "List of awards. (save in descending order, starting from the most recent)",
      items: {
        type: "object",
        properties: {
          name: {
            type: "string",
            description: "Name of the awards."
          },
          gov: {
            type: "string",
            description: "Issuing institution."
          },
          year: {
            type: "string",
            description: "Date of awards(Save in YYYY.MM format. If there is no Month, save in YYYY format)."
          }
        },
        additionalProperties: false,
        required: ["name", "gov", "year"]
      }
    },
    learns: {
      type: "array",
      description: "List of non-academic education/training. (save in descending order, starting from the most recent)",
      items: {
        type: "object",
        properties: {
          name: {
            type: "string",
            description: "Name of the course."
          },
          gov: {
            type: "string",
            description: "Institution providing the training."
          },
          year1: {
            type: "string",
            description: "Start date of the training (Save in YYYY.MM format. If there is no Month, save in YYYY format). If only a single year or year-month is listed (not a period), save it here in year1. If no date is written on the resume, leave it blank ('') — NEVER fabricate or guess a date."
          },
          year2: {
            type: "string",
            description: "End date of the training (Save in YYYY.MM format. If there is no Month, save in YYYY format). If only a single year or year-month is listed (not a period), leave it blank ('')."
          }
        },
        additionalProperties: false,
        required: ["name", "gov", "year1", "year2"]
      }
    },
    activities: {
      type: "array",
      description: "List of Volunteer or Social activities. (save in descending order, starting from the most recent)",
      items: {
        type: "object",
        properties: {
          name: {
            type: "string",
            description: "Name of the activities."
          },
          gov: {
            type: "string",
            description: "Institution providing the activities."
          },
          year1: {
            type: "string",
            description: "Start date of the activity (Save in YYYY.MM format. If there is no month, save in YYYY format). If no information, leave it blank (''). If only a single year or year-month is listed (not a period), save it here in year1. If no date is written on the resume, leave it blank ('') — NEVER fabricate or guess a date."
          },
          year2: {
            type: "string",
            description: "End date of the activity (Save in YYYY.MM format. If there is no month, save in YYYY format). If no information, leave it blank (''). If only a single year or year-month is listed (not a period), leave it blank ('')."
          }
        },
        additionalProperties: false,
        required: ["name", "gov", "year1", "year2"]
      }
    },
    overseas: {
      type: "array",
      description: "List of Overseas experience e.g. language training, Exchange student(save in descending order, starting from the most recent)",
      items: {
        type: "object",
        properties: {
          name: {
            type: "string",
            description: "Name of the experience."
          },
          gov: {
            type: "string",
            description: "Institution providing the experience."
          },
          year1: {
            type: "string",
            description: "Start date of the experience (Save in YYYY.MM format. If there is no Month, save in YYYY format). If only a single year or year-month is listed (not a period), save it here in year1. If no date is written on the resume, leave it blank ('') — NEVER fabricate or guess a date."
          },
          year2: {
            type: "string",
            description: "End date of the experience (Save in YYYY.MM format. If there is no Month, save in YYYY format). If only a single year or year-month is listed (not a period), leave it blank ('')."
          }
        },
        additionalProperties: false,
        required: ["name", "gov", "year1", "year2"]
      }
    },
    skills: {
      type: "array",
      description: "List of skills. Extract each line exactly as written on the resume — even if one line lists multiple tools/programs, do NOT split them into separate items. e.g., 'MS Office (PowerPoint, Word, Excel) 능숙' → one single item; 'SAP사용가능, ECOUNT 사용가능, 더존 사용가능' → one single item. NEVER include language proficiencies (English, Japanese, etc.) here — those go in the languages field.",
      items: {
        type: "object",
        properties: {
          name: {
            type: "string",
            description: "Name of the skill."
          },
          desc: {
            type: "string",
            description: "Proficiency level (상/중/하) or detailed description if available. If not, save as '')."
          }
        },
        additionalProperties: false,
        required: ["name", "desc"]
      }
    },
    languages: {
      type: "array",
      description: "List of language proficiencies.(English Spanish Chinese Japanese etc.)",
      items: {
        type: "object",
        properties: {
          language: {
            type: "string",
            description: "Name of the language."
          },
          level: {
            type: "string",
            description: "Proficiency level (상/중/하) or test scores if available, e.g., TOEIC 900 (2024.07)."
          }
        },
        additionalProperties: false,
        required: ["language", "level"]
      }
    },
    etcs: {
      type: "array",
      description: "Only pure miscellaneous info (e.g., military service, thesis/papers). NEVER include: self-introduction/cover letter (→selfintro field), current/desired salary, available start date, day/night shift preference (주간/야간 등), resident registration address (→addr field), total career duration summary (e.g., '경력 총 N년 N개월'), phone/email/contact info.",
      items: {
        type: "string"
      }
    },
    selfintro: {
      type: "array",
      description: "Self-introduction / cover letter text. Preserve headings and blank lines exactly as in the original: store each heading (e.g., [성장과정], '지원동기') verbatim as its own array element, and represent blank lines between paragraphs/headings as empty string ('') elements. Store body text by paragraph — only remove awkward mid-sentence line breaks caused by the document layout and join them naturally. Do not alter the wording itself. Leave empty array if not present.",
      items: {
        type: "string"
      }
    }
  },
  required: ["candidate", "yob", "gender", "addr", "core", "education", "certifications", "awards", "learns", "activities", "overseas", "skills", "languages", "etcs", "selfintro"],
  additionalProperties: false
};


const gpt5_nc_schema2 = {
  "type": "object",
  "properties": {
    "candidate": {
      "type": "string",
      "description": "후보자의 이름 이름이 3글자 이면 글자 사이에 공백을 추가. 예) 이름 '홍길동' 일 경우 '홍 길 동' 으로 저장."
    },
    "yob": {
      "type": "string",
      "description": "생년월일 YYYY.MM.DD 형식. 만약 년/월 정보만 있으면 YYYY.MM 형식으로 저장. 또 만약 년 정보만 있으면 YYYY 형식으로 저장. 생년 정보가 없으면 빈값('') 으로 저장."
    },
    "gender": {
      "type": "string",
      "enum": ["남성", "여성", ""],
      "description": "성별 정보. 만약 정보가 없으면 빈값('')으로 저장."
    },
    "addr": {
      "type": "string",
      "description": "거주지 주소. 만약 정보가 없으면 빈값('')으로 저장. 시/구 까지만 저장. 예)'서울시 강남구 테헤란로 36...' 일 경우 '서울시 강남구'로 저장"
    },
    "core": {
      "type": "array",
      "description": "이력서에 기재된 핵심역량. '핵심역량' 제목이 없더라도 프로필/요약 영역에 강점·역량을 요약한 짧은 개조식 불릿 목록이 있으면 핵심역량으로 저장(누락 금지). 개조식 강점 나열 → core, 서술형 문단 → selfintro. 기재되어있을 경우 모든 글자 그대로 저장. 기재 내용에 들여쓰기/하위 항목 등 계층 구조가 있으면 경력사항 업무상세(desc)와 동일하게 최대 4depth 계층 구조로 저장. 기재된 내용이 없으면 빈 배열.",
      "items": gpt5_depth4_desc_items
    },
    "education": {
      "type": "array",
      "description": "학력 사항 목록 (최종 최신 학력 부터 내림차순 정렬. 학위 사항만 추출).",
      "items": {
        "type": "object",
        "properties": {
          "school": {
            "type": "string",
            "description": "기재된 학교이름"
          },
          "major": {
            "type": "string",
            "description": "기재된 전공명"
          },
          "grade": {
            "type": "string",
            "description": "학점. 만약 이력서 내 학점 정보가 없으면 빈값('')으로 저장. 정보가 있는 경우 '4.0/4.5' 와 같은 양식으로 저장"
          },
          "area": {
            "type": "string",
            "description": "기재된 학교의 위치 기재된 지역이 없으면 빈값('')으로 저장. 만약 '서울특별시', '대전광역시' 등으로 기재되어있으면 줄여서 '서울', '대전' 형식으로 저장."
          },
          "degree": {
            "type": "string",
            "enum": ["학사", "석사", "박사", "전문학사", "박사후연구원", ""],
            "description": "학위. 학위 정보가 없으면 빈값('')으로 저장."
          },
          "is_grdt": {
            "type": "string",
            "enum": ["졸업", "수료", "재학중", "졸업예정", "중퇴", "휴학", ""],
            "description": "졸업 구분. 기재된 내용이 없으면 빈값('')으로 저장."
          },
          "ad_yyyymm": {
            "type": "string",
            "description": "입학년월. YYYY.MM 형식. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장."
          },
          "gdt_yyyymm": {
            "type": "string",
            "description": "졸업년월. YYYY.MM 형식. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장."
          }
        },
        "additionalProperties": false,
        "required": [
          "school",
          "major",
          "grade",
          "area",
          "degree",
          "is_grdt",
          "ad_yyyymm",
          "gdt_yyyymm"
        ]
      }
    },
    "certifications": {
      "type": "array",
      "description": "각종 자격증 정보. 최종 최신 자격사항 부터 내림차순 정렬. 예)컴퓨터활용능력, 유통관리사, AICPA, TOEIC.",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "자격사항 또는 자격증이름."
          },
          "gov": {
            "type": "string",
            "description": "발급기관. 만약 발급기관 내용이 없으면 빈값('')으로 저장."
          },
          "year": {
            "type": "string",
            "description": "취득년월 정보. YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장."
          }
        },
        "additionalProperties": false,
        "required": ["name", "gov", "year"]
      }
    },
    "awards": {
      "type": "array",
      "description": "각종 수상내역. 최종 최신 수상내역 부터 내림차순 정렬.",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "수상 명칭."
          },
          "gov": {
            "type": "string",
            "description": "발급기관. 만약 발급기관 내용이 없으면 빈값('')으로 저장."
          },
          "year": {
            "type": "string",
            "description": "수상년월. YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장."
          }
        },
        "additionalProperties": false,
        "required": ["name", "gov", "year"]
      }
    },
    "learns": {
      "type": "array",
      "description": "비학위 과정 교육/수료 내역. 최신 사항 부터 내림차순 정렬.",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "과정명칭"
          },
          "gov": {
            "type": "string",
            "description": "관련기관. 만약 관련기관 내용이 없으면 빈값('')으로 저장."
          },
          "year1": {
            "type": "string",
            "description": "과정시작년월. YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장. 기간이 아닌 단일 년도/년월만 기재된 경우 그 값을 year1에 저장. 날짜가 기재되어 있지 않으면 빈값('') — 절대 임의로 추정/생성 금지."
          },
          "year2": {
            "type": "string",
            "description": "과정종료년월. YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장. 기간이 아닌 단일 년도/년월만 기재된 경우 빈값('')으로 저장."
          }
        },
        "additionalProperties": false,
        "required": ["name", "gov", "year1", "year2"]
      }
    },
    "activities": {
      "type": "array",
      "description": "각종 사회활동 및 봉사활동 내역. 최신 사항 부터 내림차순으로 정렬",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "활동명"
          },
          "gov": {
            "type": "string",
            "description": "관련기관. 만약 관련기관 내용이 없으면 빈값('')으로 저장."
          },
          "year1": {
            "type": "string",
            "description": "활동 시작 년월. 내용이 없으면 빈값('')으로 저장. YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장. 기간이 아닌 단일 년도/년월만 기재된 경우 그 값을 year1에 저장. 날짜가 기재되어 있지 않으면 빈값('') — 절대 임의로 추정/생성 금지."
          },
          "year2": {
            "type": "string",
            "description": "활동 종료 년월. 내용이 없으면 빈값('')으로 저장. YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장. 기간이 아닌 단일 년도/년월만 기재된 경우 빈값('')으로 저장."
          }
        },
        "additionalProperties": false,
        "required": ["name", "gov", "year1", "year2"]
      }
    },
    "overseas": {
      "type": "array",
      "description": "각종 해외 활동. 최신 활동 부터 내림차순으로 정렬. 예)어학연수, 교환학생 등",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "활동 명칭."
          },
          "gov": {
            "type": "string",
            "description": "관련기관. 만약 관련기관 내용이 없으면 빈값('')으로 저장."
          },
          "year1": {
            "type": "string",
            "description": "활동 시작 년월. YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장. 기간이 아닌 단일 년도/년월만 기재된 경우 그 값을 year1에 저장. 날짜가 기재되어 있지 않으면 빈값('') — 절대 임의로 추정/생성 금지."
          },
          "year2": {
            "type": "string",
            "description": "활동 종료 년월. YYYY.MM 형식으로 저장. 만약 년도 정보만 있을 경우 YYYY 형식으로 저장. 기간이 아닌 단일 년도/년월만 기재된 경우 빈값('')으로 저장."
          }
        },
        "additionalProperties": false,
        "required": ["name", "gov", "year1", "year2"]
      }
    },
    "skills": {
      "type": "array",
      "description": "각종 업무 기술 능력. 이력서에 기재된 행(줄) 단위 그대로 추출 — 한 줄에 여러 도구/프로그램이 나열되어 있어도 항목별로 분리하지 말고 그대로 한 항목으로 저장. 예) 'MS Office (PowerPoint, Word, Excel) 능숙' → 한 항목, 'SAP사용가능, ECOUNT 사용가능, 더존 사용가능' → 한 항목. 어학능력(영어·중국어·일본어 등 언어 능력)은 절대 포함 금지(→languages).",
      "items": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "description": "업무 기술 능력."
          },
          "desc": {
            "type": "string",
            "description": "업무 숙련도. 상/중/하 또는 세부 서술한 내용 저장. 만약 정보가 없으면 빈값('')으로 저장."
          }
        },
        "additionalProperties": false,
        "required": ["name", "desc"]
      }
    },
    "languages": {
      "type": "array",
      "description": "각종 어학능력(영어, 중국어, 스페인어, 독일어, 일본어 등 기재된 내용)",
      "items": {
        "type": "object",
        "properties": {
          "language": {
            "type": "string",
            "description": "언어명"
          },
          "level": {
            "type": "string",
            "description": "언어 숙련도 상/중/하 또는 관련시험 성적이 기술되어있을 경우 해당 내용을 저장. 시험 성적일 경우 'TOEIC 900 (YYYY.MM)' 형식."
          }
        },
        "additionalProperties": false,
        "required": ["language", "level"]
      }
    },
    "etcs": {
      "type": "array",
      "description": "병역사항·논문 등 순수 기타사항만 포함. 다음은 절대 포함 금지: 자기소개(→selfintro), 현재/희망연봉, 입사가능시기, 주야간/근무형태, 주민등록상 주소(→addr), 경력 총 기간 합계, 연락처/이메일/전화번호.",
      "items": {
        "type": "string"
      }
    },
    "selfintro": {
      "type": "array",
      "description": "자기소개서 / 커버레터 텍스트. 소제목·빈 줄까지 원본 그대로 보존: 소제목은 글자 그대로 별도 요소로, 문단/제목 사이 빈 줄은 빈 문자열('') 요소로 저장. 본문은 문단 단위로 저장하되 레이아웃 때문에 문장 중간에 생긴 어색한 줄바꿈만 제거하고 이어붙일 것. 내용 자체는 왜곡/추정 없이 글자 그대로 사용. 존재하지 않는 경우 빈 배열.",
      "items": {
        "type": "string"
      }
    }
  },
  "required": ["candidate", "yob", "gender", "addr", "core", "education", "certifications", "awards", "learns", "activities", "overseas", "skills", "languages", "etcs", "selfintro"],
  "additionalProperties": false
}
