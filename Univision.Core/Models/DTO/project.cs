using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //프로젝트 테이블
  [Table("project")]
  public partial class project
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int p_seq { get; set; }


    /// <summary>
    /// 프로젝트 구분
    /// </summary>
    public int pjt_type { get; set; }

    /// <summary>
    /// 회사 seq
    /// </summary>
    public int c_seq { get; set; }

    /// <summary>
    /// client_contact seq
    /// </summary>
    public int? cc_seq { get; set; } = 0;

    /// <summary>
    /// client_tax_contact seq
    /// </summary>
    public int? ctc_seq { get; set; } = 0;

    /// <summary>
    /// 모집인원
    /// <summary>
    public double? recruit_number { get; set; }

    /// <summary>
    /// 포스팅 여부
    /// <summary>
    public double? is_posting { get; set; }

    /// <summary>
    /// 제목
    /// <summary>
    public string title { get; set; }

    /// <summary>
    /// position 테이블 seq
    /// <summary>
    public int? position_seq { get; set; }

    public string position_str { get; set; }

    /// <summary>
    /// 경력여부 (신입 : 1, 경력 : 2 , 무관 : 3)
    /// <summary>
    public double? experience_type { get; set; }

    /// <summary>
    /// 경력 년수
    /// <summary>
    public double? expreience_number { get; set; }

    /// <summary>
    /// 학력 코드
    /// <summary>
    public double? edu_code { get; set; }

    /// <summary>
    /// 학력 명
    /// <summary>
    public string edu_name { get; set; }

    /// <summary>
    /// 외국어 코드
    /// <summary>
    public string foreign_lang { get; set; }

    /// <summary>
    /// 외국어 명
    /// <summary>
    public string foreign_lang_name { get; set; }

    /// <summary>
    /// 외국어 레벨(상 : 1 , 중 : 2, 하 : 3)
    /// <summary>
    public double? foreign_level { get; set; }

    /// <summary>
    /// 주소1
    /// <summary>
    public string addr1 { get; set; }

    /// <summary>
    /// 주소2
    /// <summary>
    public string addr2 { get; set; }

    /// <summary>
    /// 담당업무
    /// <summary>
    public string assign_task { get; set; }

    /// <summary>
    /// 자격요건
    /// <summary>
    public string requirement { get; set; }
    /// <summary>
    /// 내부 참고 자료
    /// <summary>
    public string internal_contents { get; set; }
    
    /// <summary>
    /// 고객사 요청사항
    /// <summary>
    public string client_require { get; set; }

    /// <summary>
    /// 내부 참고사항
    /// <summary>
    public string internal_note { get; set; }


    /// <summary>
    /// 국문이력서 여부
    /// <summary>
    public double? is_kor_resume { get; set; }

    /// <summary>
    /// 영문 이력서 여부
    /// <summary>
    public double? is_eng_resume { get; set; }

    /// <summary>
    /// 포트폴리오 여부
    /// <summary>
    public int? is_portfolio { get; set; }
    
    /// <summary>
    /// 자기소개서 여부
    /// <summary>
    public double? is_self_introduction { get; set; }

    /// <summary>
    /// 기타 여부
    /// <summary>
    public double? is_etc { get; set; }

    /// <summary>
    /// 기타 내용
    /// <summary>
    public string etc_comment { get; set; }

    /// <summary>
    /// 사전 면접 여부
    /// <summary>
    public double? is_pre_interview { get; set; }

    /// <summary>
    /// 면접 차수 여부
    /// </summary>
    public double? is_number { get; set; }

    /// <summary>
    /// 면접자수
    /// <summary>
    public double? interview_number { get; set; }

    /// <summary>
    /// 인성검사 여부
    /// <summary>
    public double? is_personality_test { get; set; }

    /// <summary>
    /// 성별(남자: 1, 여자 : 2 , 무관 : 3)
    /// <summary>
    public double? gender_type { get; set; }

    /// <summary>
    /// 시작 연령 대
    /// <summary>
    public double? start_age { get; set; }

    /// <summary>
    /// 마지막 연령 대
    /// <summary>
    public double? end_age { get; set; }

    /// <summary>
    /// 타겟학교 코드
    /// <summary>
    public string target_school_nm { get; set; }

    /// <summary>
    /// 타겟 전공 코드
    /// <summary>
    public string target_major_nm { get; set; }

    /// <summary>
    /// 타겟 회사
    /// <summary>
    public string target_company_nm { get; set; }

    /// <summary>
    /// 기밀여부
    /// </summary>
    public int? confidentiallity { get; set; }

    /// <summary>
    /// 예상연봉(사용안할 예정)
    /// <summary>
    public int? expected_salary { get; set; }
    /// <summary>
    /// 예상연봉
    /// <summary>
    public double? exp_salary { get; set; }
    /// <summary>
    /// 예상연봉(원화)
    /// <summary>
    public double? exp_salary_won { get; set; }

    public string currency_cd { get; set; }
    /// <summary>
    /// 수수료율
    /// <summary>
    public decimal? fee_rate { get; set; }

    /// <summary>
    /// 공고내용
    /// <summary>
    public string contents { get; set; }

    /// <summary>
    /// 프로젝트 상태 ( 진행 : 1, 보류 : 2, 실패 : 3, 성공 : 4)
    /// <summary>
    public int? pjt_status { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string status_comment { get; set; }


    /// <summary>
    /// 공유 프로젝트 여부
    /// </summary>
    public int? is_share_pjt { get; set; }

    /// <summary>
    /// 전담 프로젝트 여부
    /// </summary>
    public int? is_cowork { get; set; }

    public DateTime? cowork_dt { get; set; }
    /// <summary>
    /// 공유 사유
    /// </summary>
    public string share_comments { get; set; }

    public DateTime? share_dt { get; set; }

    /// <summary>
    /// 비공개 정보
    /// </summary>
    public string secret_info { get; set; }

    /// <summary>
    /// Fee Share
    /// </summary>
    public double? share_fee_rate { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_user { get; set; }

    /// <summary>
    /// 수정일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    //완료/성공/종료 시 일자
    public DateTime? close_dt { get; set; }

    //프로젝트 진행(재진행) 일자
    public DateTime? open_dt { get; set; }

    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_user { get; set; }

    /// <summary>
    /// 산업코드 1단계
    /// </summary>
    public double? business_code1 { get; set; }

    /// <summary>
    /// 산업코드 2단계
    /// </summary>
    public double? business_code2 { get; set; }

    /// <summary>
    /// 보조산업코드 1단계
    /// </summary>
    public double? sub_business_code1 { get; set; }

    /// <summary>
    /// 보조산업코드 2단계
    /// </summary>
    public double? sub_business_code2 { get; set; }

    /// <summary>
    /// 직무코드 1단계
    /// </summary>
    public int job_code1 { get; set; }

    /// <summary>
    /// 직무코드 2단계
    /// </summary>
    public int job_code2 { get; set; }

    /// <summary>
    /// 보조직무코드 1단계
    /// </summary>
    public int? sub_job_code1 { get; set; }

    /// <summary>
    /// 보조직무코드 2단계
    /// </summary>
    public int? sub_job_code2 { get; set; }

    public int? no_business { get; set; }
    public int? no_job { get; set; }

    //PE프로젝트 구분
    public int? is_pe { get; set; }
  }

  /// <summary>
  /// 리스트
  /// </summary>
  public partial class project
  {
    /// <summary>
    /// 프로젝트 후보자 최고 상태
    /// </summary>
    [NotMapped]
    public int recState { get; set; }

    /// <summary>
    /// AM 이름들
    /// </summary>
    [NotMapped]
    public string am_names { get; set; }

    [NotMapped]
    public int? exist_candidate { get; set; }

    /// <summary>
    /// Searcher 이름들
    /// </summary>
    [NotMapped]
    public string searcher_names { get; set; }

    /// <summary>
    /// 산업 명들
    /// </summary>
    [NotMapped]
    public string business_names1 { get; set; }
    [NotMapped]
    public string business_names2 { get; set; }
    /// <summary>
    /// 직무 명들
    /// </summary>
    [NotMapped]
    public string job_names1 { get; set; }
    [NotMapped]
    public string job_names2 { get; set; }

    /// <summary>
    /// 보조산업 명들
    /// </summary>
    [NotMapped]
    public string sub_business_names1 { get; set; }
    [NotMapped]
    public string sub_business_names2 { get; set; }
    /// <summary>
    /// 보조직무 명들
    /// </summary>
    [NotMapped]
    public string sub_job_names1 { get; set; }
    [NotMapped]
    public string sub_job_names2 { get; set; }
    [NotMapped]
    public string orderOption { get; set; } = "DESC";
    [NotMapped]
    public string orderTxt { get; set; } = "kor_name";
    [NotMapped]
    public int read_cnt { get; set; }
  }

  //채용
  public partial class project
  {
    /// <summary>
    /// 고객사 명
    /// </summary>
    [NotMapped]
    public string company_name { get; set; }
    [NotMapped]
    public string company_name_eng { get; set; }

    [NotMapped]
    public string exp_salary_won_str { get; set; }

    [NotMapped]
    public string fee_rate_str { get; set; }
    [NotMapped]
    public string hire_info { get; set; }
    [NotMapped]
    public string position_name { get; set; }
    [NotMapped]
    public string pjt_status_str { get; set; }

    [NotMapped]
    public string pjt_type_str { get; set; }

    [NotMapped]
    public string business_name2 { get; set; }

    [NotMapped]
    public string job_name2 { get; set; }

    [NotMapped]
    public string sub_business_name2 { get; set; }

    [NotMapped]
    public string sub_job_name2 { get; set; }
    /// <summary>
    /// 타겟학교 캠퍼스명
    /// </summary>
    [NotMapped]
    public string target_school_campus { get; set; }

    /// <summary>
    /// 타겟학교명
    /// </summary>
    [NotMapped]
    public string target_school_name { get; set; }

    /// <summary>
    /// 타겟학과명
    /// </summary>
    [NotMapped]
    public string target_major_name { get; set; }

    /// <summary>
    /// 타겟회사명
    /// </summary>
    [NotMapped]
    public string target_company_name { get; set; }
  }

  //평판조회
  public partial class project
  {
    /// <summary>
    /// 세금계산서 담당자 부서
    /// </summary>
    [NotMapped]
    public string tax_division { get; set; }
    /// <summary>
    /// 세금계산서 담당자 이름
    /// </summary>
    [NotMapped]
    public string tax_name { get; set; }

    /// <summary>
    /// 세금계산서 담당자 이메일
    /// </summary>
    [NotMapped]
    public string tax_email { get; set; }

    /// <summary>
    ///  세금계산서 담당자 전화번호
    /// </summary>
    [NotMapped]
    public string tax_phone { get; set; }

    /// <summary>
    ///  세금계산서 담당자 휴대폰
    /// </summary>
    [NotMapped]
    public string tax_cell_phone { get; set; }

    /// <summary>
    ///  세금계산서 입금메일 담당자명
    /// </summary>
    [NotMapped]
    public string tax_deposit_manager { get; set; }

    /// <summary>
    ///  세금계산서 입금메일
    /// </summary>
    [NotMapped]
    public string tax_deposit_email { get; set; }

    /// <summary>
    ///  담당자 이름
    /// </summary>
    [NotMapped]
    public string contact_name { get; set; }

    /// <summary>
    /// 담당자 성별
    /// </summary>
    [NotMapped]
    public int contact_gender { get; set; }

    /// <summary>
    /// 담당자 이메일
    /// </summary>
    [NotMapped]
    public string contact_email { get; set; }

    /// <summary>
    /// 담당자 전화번호
    /// </summary>
    [NotMapped]
    public string contact_phone { get; set; }

    /// <summary>
    /// 담당자 휴대폰
    /// </summary>
    [NotMapped]
    public string contact_cell_phone { get; set; }

    /// <summary>
    /// 담당자 부서
    /// </summary>
    [NotMapped]
    public string contact_division { get; set; }

    /// <summary>
    /// 담당자 직함
    /// </summary>
    [NotMapped]
    public string contact_position { get; set; }
  }

  public partial class project
  {
    /// <summary>
    /// 산업
    /// </summary>
    [NotMapped]
    public string biz_industry { get; set; }

    /// <summary>
    /// AM
    /// </summary>
    [NotMapped]
    public string uv_name { get; set; }

    /// <summary>
    /// searcher
    /// </summary>
    [NotMapped]
    public string director { get; set; }

    [NotMapped]
    public string pjt_value { get; set; }
  }

  public partial class project
  {
    [NotMapped]
    public int log_cnt { get; set; }

    [NotMapped]
    public int is_share_3m { get; set; }

    [NotMapped]
    public int is_my_key{ get; set; }

    [NotMapped]
    public string key_user_name { get; set; }

    [NotMapped]
    public DateTime? key_create_dt { get; set; }

    [NotMapped]
    public int interest_cnt { get; set; }
    [NotMapped]
    public int recommend_cnt { get; set; }
    [NotMapped]
    public int interview_cnt { get; set; }
    [NotMapped]
    public int nego_cnt { get; set; }
  }
}


