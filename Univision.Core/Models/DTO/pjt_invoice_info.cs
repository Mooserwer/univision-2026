using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //프로젝트 빌링 인포
  [Table("pjt_invoice_info")]
  public partial class pjt_invoice_info
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int pii_seq { get; set; }

    /// <summary>
    /// project pk
    /// <summary>
    public int p_seq { get; set; }

    /// <summary>
    /// candidate pk
    /// <summary>
    public int? c_seq { get; set; }

    /// <summary>
    /// project_reindicate pk
    /// <summary>
    public int? prc_seq { get; set; }

    /// <summary>
    /// pre_pii (
    /// <summary>
    public int? pre_pii { get; set; }

    /// <summary>
    /// 입사일
    /// <summary>
    public DateTime? join_dt { get; set; }

    /// <summary>
    /// 빌링 날짜
    /// <summary>
    public DateTime? billing_dt { get; set; }

    /// <summary>
    /// 세금계산서 발행 희망일
    /// <summary>
    public DateTime? tax_req_dt { get; set; }

    /// <summary>
    /// 매출기준일 
    /// <summary>
    public DateTime? base_dt { get; set; }


    /// <summary>
    /// 연봉(사용안할 예정)
    /// <summary>
    public int? annual_income { get; set; }
    /// <summary>
    /// 실 연봉
    /// <summary>
    public double? ann_income { get; set; }

    public string income_currency_cd { get; set; }

    /// <summary>
    /// 수수료율
    /// <summary>
    public double? fee_rate { get; set; }

    /// <summary>
    /// 빌링금액(생성전용)
    /// <summary>
    public double? billing_money { get; set; }
    /// <summary>
    /// 원화 환산액
    /// <summary>
    public double? billing_won { get; set; }
    
    

    /// <summary>
    /// 총 금액
    /// <summary>
    public double? billing_total { get; set; }
    public double? billing_total_won { get; set; }
    /// <summary>
    /// 공급가
    /// <summary>
    public double? billing_amt { get; set; }

    /// <summary>
    /// 부가세
    /// <summary>
    public double? billing_vat { get; set; }
    public double? billing_vat_won { get; set; }

    public string bill_currency_cd { get; set; }

    public int? vat_type { get; set; } = 0;
    public int? is_po_no { get; set; } = 0;

    /// <summary>
    /// 빌링 유형(정액 : 1, 비율 : 2)
    /// <summary>
    public int? billing_type { get; set; }

    /// <summary>
    /// 개런티 만료일
    /// <summary>
    public DateTime? expire_guarantee { get; set; }

    /// <summary>
    /// 인보이스 후보자명 표시 (0 : No, 1: Yes)
    /// <summary>
    public int? is_open_name { get; set; } = 1;

    /// <summary>
    /// 
    /// <summary>
    public int? is_open_annual_income { get; set; } = 1;

    /// <summary>
    /// 인보이스 언어 (0 : 국문, 1 : 영문)
    /// <summary>
    public int? invoice_lang { get; set; }

    /// <summary>
    /// 인보이스 종류(0: Contingency, 1: Retainer, 2: Retainer Balance, 3:Other)
    /// <summary>
    public int? invoice_type { get; set; }

    /// <summary>
    /// 비고
    /// <summary>
    public string remarks { get; set; }

    public string remark_admin { get; set; }

    /// <summary>
    /// 승인일자
    /// <summary>
    public DateTime? confirm_dt { get; set; }

    /// <summary>
    /// 승인자
    /// <summary>
    public int? confirm_user { get; set; }

    /// <summary>
    /// 발송일자
    /// <summary>
    public DateTime? send_dt { get; set; }

    /// <summary>
    /// 발송자
    /// <summary>
    public int? send_user { get; set; }

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

    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_user { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string invoice_no { get; set; }

    /// <summary>
    /// 인보이스 파일 생성여부(0: 미생성, 1:생성)
    /// <summary>
    public int? is_file { get; set; }

    /// <summary>
    /// 인보이스 파일 폴더 경로
    /// <summary>
    public string file_dir { get; set; }

    /// <summary>
    /// 인보이스 파일 원본 이름
    /// <summary>
    public string file_origin_path { get; set; }

    /// <summary>
    /// 인보이스 파일 이름
    /// <summary>
    public string file_path { get; set; }

    /// <summary>
    /// 인보이스 파일 확장자
    /// <summary>
    public string file_extension { get; set; }

    /// <summary>
    /// 인보이스 실입금 금액
    /// <summary>
    public double? deposit_amt { get; set; }

    /// <summary>
    /// 인보이스 실입금 일자
    /// <summary>
    public DateTime? deposit_dt { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? file_dt { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? file_user { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string po_no { get; set; }

    /// <summary>
    /// 프로젝트 타이틀
    /// <summary>
    public string pjt_title { get; set; }

    /// <summary>
    /// 후보자 명
    /// <summary>
    public string candidate_name { get; set; }

    /// <summary>
    /// 고객사 명
    /// <summary>
    public string client_name { get; set; }

    /// <summary>
    /// 고객사 대표자명
    /// <summary>
    public string client_ceo { get; set; }

    /// <summary>
    /// 고객사 주소
    /// <summary>
    public string client_addr1 { get; set; }

    /// <summary>
    /// 고객사 사업자번호
    /// <summary>
    public string client_biz_code { get; set; }

    /// <summary>
    /// 고객사 수수료율 반영구분(연봉:a, 직급:b, 고정:c)
    /// <summary>
    public string client_fee_type { get; set; }

    /// <summary>
    /// 고객사 세금계산서 담당자 이름
    /// <summary>
    public string client_contact_name { get; set; }

    /// <summary>
    /// 고객사 세금계산서 담당자 이메일
    /// <summary>
    public string client_contact_email { get; set; }

    /// <summary>
    /// 고객사 세금계산서 담당자 전화번호
    /// <summary>
    public string client_contact_phone { get; set; }

    /// <summary>
    /// 고객사 세금계산서 담당자 휴대폰 번호
    /// <summary>
    public string client_contact_cell_phone { get; set; }

    /// <summary>
    /// 고객사 세금계산서 담당자 부서
    /// <summary>
    public string client_contact_division { get; set; }

    public string etax_name { get; set; }
    public string etax_email { get; set; }
    public string etax_phone { get; set; }

    /// <summary>
    /// 후보자 소스 ( 1: 유니비전 후보, 1: 링크드인, 2:리멤버, 3:직접지원, 9: 기타)
    /// </summary>
    public int? candidate_source { get; set; }
    public string candidate_source_txt { get; set; }

    public int? candidate_position { get; set; }
    public string candidate_position_txt { get; set; }

    /// <summary>
    /// 인보이스 삭제여부( 0: x, 1: 삭제)
    /// </summary>
    public int is_deleted { get; set; } = 0;

    public string invoice_title { get; set; }
    public string invoice_contents { get; set; }
    public string deposit_bank_name { get; set; }
    public string deposit_bank_account { get; set; }

    public double? ex_rate { get; set; }
  }

  public partial class pjt_invoice_info
  {
    [NotMapped]
    public int cc_seq { get; set; } = 0;

    [NotMapped]
    public int ctc_seq { get; set; } = 0;

    [NotMapped]
    public int candidate_seq { get; set; }

    [NotMapped]
    public string candidate_eng_name { get; set; }

    [NotMapped]
    public int client_seq { get; set; }

    [NotMapped]
    public string client_eng_name { get; set; }

    [NotMapped]
    public string ceo { get; set; }

    [NotMapped]
    public string addr { get; set; }

    [NotMapped]
    public string biz_code { get; set; }

    [NotMapped]
    public string create_user_name { get; set; }

    [NotMapped]
    public string confirm_user_name { get; set; }

    [NotMapped]
    public int pjt_type { get; set; }
  }
}


