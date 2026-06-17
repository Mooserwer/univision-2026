using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  /// <summary>
  /// 리멤버 측으로 부터 수신받는 인보이스 발행 정보
  /// </summary>
  public class r_invoice_v2
  {
    /// <summary>
    /// 인보이스 고유 번호
    /// </summary>
    public long id { get; set; }

    /// <summary>
    /// 취소/환불 대상 인보이스 고유 번호
    /// </summary>
    public long? root_invoice_id { get; set; }
    /// <summary>
    /// 인보이스 구분 (인보이스 상태 ISSUED: 발행, CANCELED: 취소, REFUNDED: 환불)
    /// </summary>
    public string status { get; set; }
    /// <summary>
    /// 인보이스 제목
    /// </summary>
    public string title { get; set; }

    /// <summary>
    /// 인보이스 본문
    /// </summary>
    public string description { get; set; }

    /// <summary>
    /// 인보이스 언어 ENUM(KR, EN)
    /// </summary>
    public string language { get; set; }

    /// <summary>
    /// 세금 포함 여부
    /// </summary>
    public bool tax_include { get; set; }

    /// <summary>
    /// 세금 유형 Enum(TAXABLE,ZERO_RATED,EXEMPT)
    /// TAXABLE(과세), ZERO_RATED(영세), EXEMPT(비과세)
    /// </summary>
    public string tax_type { get; set; }

    /// <summary>
    /// PO넘버 포함 여부
    /// </summary>
    public bool is_po_number { get; set; }

    /// <summary>
    /// 발행 금액
    /// </summary>
    public decimal apply_amount { get; set; }

    /// <summary>
    /// 공급 금액
    /// </summary>
    public decimal supply_amount { get; set; }

    /// <summary>
    /// 세금 금액
    /// </summary>
    public decimal tax_amount { get; set; }

    /// <summary>
    /// 총 금액
    /// </summary>
    public decimal total_amount { get; set; }

    /// <summary>
    /// 발행 금액 단위
    /// </summary>
    public string amount_currency { get; set; }

    /// <summary>
    /// 보증기간 (일, 수)
    /// </summary>
    public int warranty_days { get; set; } = 0;

    /// <summary>
    /// 계산서 발행 요청일
    /// </summary>
    public DateTime issued_date { get; set; }

    /// <summary>
    /// 입금기한일
    /// </summary>
    public int deposit_due_days { get; set; } = 0;

    /// <summary>
    /// 후보자명 표시 여부
    /// </summary>
    public bool is_show_candidates_name { get; set; }

    /// <summary>
    /// 연봉 수수료율 표시 여부
    /// </summary>
    public bool is_show_commission_rate { get; set; }

    /// <summary>
    /// 정액여부
    /// </summary>
    public bool is_modify_apply_amount { get; set; }

    /// <summary>
    /// 프로젝트 정보
    /// </summary>
    public r_key_project_v2 key_project { get; set; } = new r_key_project_v2();

    /// <summary>
    /// 후보자 정보
    /// </summary>
    public r_key_project_candidate_v2 key_project_candidate { get; set; } = new r_key_project_candidate_v2();

    /// <summary>
    /// 고객사 정보
    /// </summary>
    public r_account_v2 account { get; set; } = new r_account_v2();

    /// <summary>
    /// 고객사 담당자
    /// </summary>
    public r_account_contact_v2 invoice_contact { get; set; } = new r_account_contact_v2();

    /// <summary>
    /// 계산서 담당자
    /// </summary>
    public r_account_contact_v2 tax_contact { get; set; } = new r_account_contact_v2();

    /// <summary>
    /// 프로젝트 배분정보
    /// </summary>
    public List<r_participant_v2> participants { get; set; } = new List<r_participant_v2>();

    public r_user_v2 user { get; set; } = new r_user_v2();
    /// <summary>
    /// 입금은행명
    /// </summary>
    public string bank_name { get; set; }

    /// <summary>
    /// 내부 요청사항
    /// </summary>
    public string remark { get; set; }

    //===================================================


    /// <summary>
    /// 신청자 고유 번호
    /// </summary>
    public long request_user_id { get; set; }

    /// <summary>
    /// 신청자명
    /// </summary>
    public string request_user_name { get; set; }

    
    /// <summary>
    /// 후보자 최종 직급
    /// </summary>
    public string final_pos { get; set; }
    /*
    /// <summary>
    /// 후보자 최종 소스
    /// </summary>
    public string final_source { get; set; }

    /// <summary>
    /// 포지션 이름
    /// </summary>
    public string position_str { get; set; }
    */
  }

  /// <summary>
  /// 프로젝트 정보
  /// </summary>
  public class r_key_project_v2
  {
    /// <summary>
    /// 프로젝트 고유 번호
    /// </summary>
    public long id { get; set; }

    /// <summary>
    /// 프로젝트 타입 Enum(RECRUITMENT, CONSULTING)
    /// RECRUITMENT: 채용, CONSULTING: 컨설팅
    /// </summary>
    public string type { get; set; }
    /// <summary>
    /// 프로젝트 서브 타입
    /// RECRUITER: 채용대행, ADVANCED_PAYMENT: 선수금, RPO: 채용대행, REPUTATION_CHECK: 평판조회
    /// </summary>
    public string category_sub_type { get; set; }
    /// <summary>
    /// 키 프로젝트 이름
    /// </summary>
    public string title { get; set; }
  }

  /// <summary>
  /// 후보자 정보
  /// </summary>
  public class r_key_project_candidate_v2
  {
    /// <summary>
    /// 후보자 고유 번호
    /// </summary>
    public long id { get; set; }

    /// <summary>
    /// 후보자 이름
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// 연봉
    /// </summary>
    public decimal salary { get; set; }

    /// <summary>
    /// 후보자 연봉 화폐단위
    /// </summary>
    public string salary_currency { get; set; }

    /// <summary>
    /// 수수료율
    /// </summary>
    public decimal commission_rate { get; set; }
    /// <summary>
    /// 추천인 정보 출처 (링크드인, 리멤버, ETC..)
    /// </summary>
    public string reference { get; set; }

    /// <summary>
    /// 후보자 직급
    /// </summary>
    public string job_rank { get; set; }
    
    /// <summary>
    /// 입사일
    /// </summary>
    public DateTime? joining_date { get; set; }

    /// <summary>
    /// 퇴사일
    /// </summary>
    public DateTime? leaving_date { get; set; }

  }

  /// <summary>
  /// 고객사 정보
  /// </summary>
  public class r_account_v2
  {
    /// <summary>
    /// 고객사 고유 번호
    /// </summary>
    public long id { get; set; }

    /// <summary>
    /// 고객사 이름
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// 고객사 사업자 등록번호
    /// </summary>
    public string business_code { get; set; }

    /// <summary>
    /// 고객사 주소
    /// </summary>
    public string address { get; set; }

    /// <summary>
    /// 고객사 대표자 명
    /// </summary>
    public string ceo_name { get; set; }


    public decimal base_research_deduction_rate { get; set; }


  }

  /// <summary>
  /// 담당자 정보
  /// </summary>
  public class r_account_contact_v2
  {
    /// <summary>
    /// 담당자 고유 번호
    /// </summary>
    public long id { get; set; }

    /// <summary>
    /// 담당자 이름
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// 담당자 연락처 (휴대폰 번호)
    /// </summary>
    public string mobile_phone_number { get; set; }

    /// <summary>
    /// 담당자 연락처 (전화번호)
    /// </summary>
    public string phone_number { get; set; }

    /// <summary>
    /// 담당자 이메일
    /// </summary>
    public string email { get; set; }

    /// <summary>
    /// 담당자 직급
    /// </summary>
    public string job_rank { get; set; }

    /// <summary>
    /// 담당자 부서
    /// </summary>
    public string department { get; set; }

    /// <summary>
    /// 담당자 유형 Enum(HR, INVOICE)
    /// HR: HR 담당자, INVOICE: 세금계산서 담당자
    /// </summary>
    public string manager_type { get; set; }

    /// <summary>
    /// 담당자 부서
    /// </summary>
    public DateTime created_at { get; set; }

    /// <summary>
    /// 수정일시
    /// </summary>
    public DateTime updated_at { get; set; }

  }

  public class r_user_v2
  {
    /// <summary>
    /// 신청자 고유 번호
    /// </summary>
    public long id { get; set; }

    /// <summary>
    /// 신청자 명
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// 신청자 이메일
    /// </summary>
    public string email { get; set; }
  }

  /// <summary>
  /// 
  /// </summary>
  public class r_participant_v2
  {
    /// <summary>
    /// 참여자 고유 번호
    /// </summary>
    public long id { get; set; }

    /// <summary>
    /// 참여자 명
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// 참여자 이메일
    /// </summary>
    public string email { get; set; }

    /// <summary>
    /// 참여자 역할
    /// </summary>
    public string role { get; set; }

    /// <summary>
    /// 배분율
    /// </summary>
    public decimal contribution_rate { get; set; }

    /// <summary>
    /// 배분금액
    /// </summary>
    public long contribution_amount { get; set; }

    /// <summary>
    /// 환율
    /// </summary>
    public float exchange_rate { get; set; }

    /// <summary>
    /// 참여자 역할
    /// </summary>
    public string currency { get; set; }    
  }
}
