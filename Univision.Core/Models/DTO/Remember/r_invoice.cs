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
  public class r_invoice
  {
    /// <summary>
    /// 인보이스 고유 번호
    /// </summary>
    public long invoice_id { get; set; }

    
    /// <summary>
    /// 인보이스 언어
    /// </summary>
    public string invoice_language { get; set; }

    /// <summary>
    /// 신청자 고유 번호
    /// </summary>
    public long request_user_id { get; set; }

    /// <summary>
    /// 신청자명
    /// </summary>
    public string request_user_name { get; set; }

    /// <summary>
    /// poNumber 필요 여부
    /// </summary>
    public bool is_po_number { get; set; }

    /// <summary>
    /// 후보자명 표시 여부
    /// </summary>
    public bool is_show_candidates_name { get; set; }

    /// <summary>
    /// 연봉 수수료율 표시 여부
    /// </summary>
    public bool is_show_commission_rate { get; set; }

    /// <summary>
    /// 인보이스 제목
    /// </summary>
    public string invoice_title { get; set; }

    /// <summary>
    /// 인보이스 본문
    /// </summary>
    public string invoice_contents { get; set; }

    /// <summary>
    /// 입금은행명
    /// </summary>
    public string bank_info { get; set; }

    /// <summary>
    /// 내부 요청사항
    /// </summary>
    public string memo { get; set; }

    /// <summary>
    /// 고객사 정보
    /// </summary>
    public r_account account { get; set; }

    /// <summary>
    /// 프로젝트 고유 번호
    /// </summary>
    public long key_project_id { get; set; }

    /// <summary>
    /// 프로젝트 명
    /// </summary>
    public string key_project_name { get; set; }

    /// <summary>
    /// 후보자 고유 번호
    /// </summary>
    public long? candidate_id { get; set; }

    /// <summary>
    /// 후보자 명
    /// </summary>
    public string candidate_name { get; set; }

    /// <summary>
    /// 연봉
    /// </summary>
    public decimal salary { get; set; }

    /// <summary>
    /// 연봉 단위
    /// </summary>
    public string salary_unit { get; set; }

    /// <summary>
    /// 확정 수수료율
    /// </summary>
    public decimal commission_rate { get; set; }

    /// <summary>
    /// 정액여부
    /// </summary>
    public Boolean is_fixed_amount { get; set; }


    /// <summary>
    /// 발행 금액
    /// </summary>
    public decimal apply_amount { get; set; }

    /// <summary>
    /// 발행 금액 단위
    /// </summary>
    public string amount_unit { get; set; }

    /// <summary>
    /// 후보자 최종 직급
    /// </summary>
    public string final_pos { get; set; }

    /// <summary>
    /// 후보자 최종 소스
    /// </summary>
    public string final_source { get; set; }

    /// <summary>
    /// 포지션 이름
    /// </summary>
    public string position_str { get; set; }
    /// <summary>
    /// 입사일 요청일
    /// </summary>
    public DateTime? joining_date { get; set; }
    /// <summary>
    /// 계산서 발행 요청일
    /// </summary>
    public DateTime issued_date { get; set; }

    /// <summary>
    /// 고객사 담당자
    /// </summary>
    public r_account_contact account_contacts { get; set; }

    /// <summary>
    /// 프로젝트 배분정보
    /// </summary>
    public List<r_participant> participants { get; set; } = new List<r_participant>();

  }

  /// <summary>
  /// 고객사 정보
  /// </summary>
  public class r_account
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

    /// <summary>
    /// TAXABLE(과세)
    /// ZERO_RATED(영세)
    /// EXEMPT(비과세)
    /// </summary>
    public string tax_type { get; set; }

    /// <summary>
    /// 보증기간 (일, 수)
    /// </summary>
    public int warranty_days { get; set; }
  }

  /// <summary>
  /// 고객사 담당자 묶음
  /// </summary>
  public class r_account_contact
  {
    /// <summary>
    /// 인보이스 담당자
    /// </summary>
    public r_account_contact_dtl invoice { get; set; }

    /// <summary>
    /// 계산서 담당자
    /// </summary>
    public r_account_contact_dtl tax { get; set; }
  }

  /// <summary>
  /// 담당자 정보
  /// </summary>
  public class r_account_contact_dtl
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
    /// 담당자 연락처
    /// </summary>
    public string mobile_phone_number { get; set; }

    /// <summary>
    /// 담당자 이메일
    /// </summary>
    public string email { get; set; }
  }


  /// <summary>
  /// 
  /// </summary>
  public class r_participant
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
    public long amount { get; set; }
  }
}
