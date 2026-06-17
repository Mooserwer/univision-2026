using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request.Project;

namespace Univision.Main.Models.Invoice
{
  public class InvoiceCreateModel
  {
    public int p_seq { get; set; }

    public int pjt_type { get; set; }
    public string pjt_position_str { get; set; } //포지션 (인보이스 내용에 포함될 내용)
    public int? pii_seq { get; set; } // 인보이스 고유 코드
    public int? pre_pii { get; set; } // 관련 인보이스 코드 (취소/환불 시 취소/환불 대상)
    public DateTime? billing_dt { get; set; } // 빌링 요청일

    public DateTime? tax_req_dt { get; set; }  // 세금계산서 발행 희망일

    //후보자 관련 정보
    public int? c_seq { get; set; } //후보자 코드
    public int? prc_seq { get; set; } // 후보자 프로젝트 코드
    public string candidate_name_kor { get; set; }
    public string candidate_name_eng { get; set; }
    public DateTime? join_dt { get; set; } // 후보자 입사일자
    public int? annual_income { get; set; }
    public double? ann_income { get; set; }
    public string income_currency_cd { get; set; }
    public DateTime? expire_guarantee { get; set; } // 후보자 보증기간 

    public int guarantee_day { get; set; } = 0;
    public string construct_debut_yn { get; set; } = "N";
    public int construct_debut_per { get; set; } = 0;

    //금액 관련 정보
    public double? retainer_money { get; set; }
    public double? fee_rate { get; set; }
    public double? billing_money { get; set; }
    public double? billing_won { get; set; }
    public double? billing_vat { get; set; }
    public double? billing_total { get; set; }
    public double? billing_amt { get; set; }
    public double? retainer_amt { get; set; }
    public string bill_currency_cd { get; set; }
    public int? vat_type { get; set; } = 1; // 부가가치세 포함,미포함,면세 여부
    public int? billing_type { get; set; } //정액 여부 


    //고객사 관련 정보
    public int client_seq { get; set; }
    public int ctc_seq { get; set; } = 0; // 고객사 세금계산서 담당자 코드
    public int cc_seq { get; set; } = 0; // 고객사 인보이스 담당자 코드
    public string client_name_kor { get; set; }
    public string client_name_eng { get; set; }
    public string ceo_kor { get; set; }
    public string ceo_eng { get; set; }
    public string addr_kor { get; set; }
    public string addr_eng { get; set; }
    public string biz_code { get; set; }
    public DateTime? client_cert_upload_dt { get; set; }
    public string client_cert_file { get; set; }
    public int? client_cert_seq { get; set; }

    public DateTime? client_agr_upload_dt { get; set; }
    public string client_agr_file { get; set; }
    public int? client_agr_seq { get; set; }

    public int? is_open_name { get; set; }
    public int? invoice_lang { get; set; }
    public int? invoice_type { get; set; }
    public int? is_open_annual_income { get; set; }
    public int? is_po_no { get; set; } = 0;

    public string remarks { get; set; } = "";

    public string invoice_title { get; set; }
    public string invoice_contents { get; set; }
    public string deposit_bank_name { get; set; }
    public string deposit_bank_account { get; set; }

    public string client_name { get; set; }
    public string client_ceo { get; set; }
    public string client_addr1 { get; set; }
    public string client_biz_code { get; set; }

    public string client_contact_name { get; set; }
    public string client_contact_tel { get; set; }
    public string client_contact_email { get; set; }
    public string client_contact_pos { get; set; }
    public string client_contact_div { get; set; }
    public string client_contact_tel2 { get; set; }
    public int? client_contact_gender { get; set; }


    public string client_etax_name { get; set; }
    public string client_etax_tel { get; set; }
    public string client_etax_tel2 { get; set; }
    public string client_etax_email { get; set; }

    public int? candidate_source { get; set; }
    public string candidate_source_txt { get; set; }

    public int? candidate_position { get; set; }
    public string candidate_position_txt { get; set; }


    public int opt_is_client_update { get; set; } = 1;

    public int opt_is_candidate_name_update { get; set; } = 1;
    public int opt_is_success { get; set; } = 1;

    public int opt_is_send_only_share { get; set; } = 1;

    public int opt_pre_select_seq { get; set; } = 0;
    public List<ProjectInvoiceChargeModel> chargeList { get; set; }

    public List<pjt_recandidate_history> candidateList { get; set; }

    public List<pjt_invoice_info> pre_invoice_list { get; set; }

    public pjt_invoice_info pre_invoice { get; set; }
  }

}