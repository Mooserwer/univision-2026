using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request.Project;

namespace Univision.Main.Models.Project
{
  public class ProjectInvoiceCreateModel
  {
    public int p_seq { get; set; }
    public int? c_seq { get; set; }
    public int? pii_seq { get; set; }
    public int? prc_seq { get; set; }
    public int ctc_seq { get; set; } = 0;
    public int cc_seq { get; set; } = 0;
    public string candidate_name { get; set; }
    public string candidate_eng_name { get; set; }
    public DateTime? join_dt { get; set; }
    public DateTime? billing_dt { get; set; }
    public int? annual_income { get; set; }
    public double? ann_income { get; set; }
    public string income_currency_cd { get; set; }
    public double? fee_rate { get; set; }
    public double? billing_money { get; set; }
    public double? billing_won { get; set; }
    public double? billing_vat { get; set; }
    public double? billing_total { get; set; }
    public double? billing_amt { get; set; }
    public double? retainer_amt { get; set; }
    public string bill_currency_cd { get; set; }
    public int? vat_type { get; set; } = 1;
    public int? is_po_no { get; set; } = 0;
    public int? billing_type { get; set; }
    public DateTime? expire_guarantee { get; set; }
    public int client_seq { get; set; }
    public string client_name { get; set; }
    public string client_eng_name { get; set; }
    public string ceo { get; set; }
    public string addr { get; set; }
    public string biz_code { get; set; }
    public int? is_open_name { get; set; }
    public int? invoice_lang { get; set; }
    public int? invoice_type { get; set; }
    public int? is_open_annual_income { get; set; }
    public string remarks { get; set; }
    public bool isEngCandiSave { get; set; }
    public bool isEngCompanySave { get; set; }

    public string client_ceo { get; set; }
    public string client_addr1 { get; set; }
    public string client_biz_code { get; set; }

    public string client_contact_name { get; set; }
    public string client_contact_tel { get; set; }
    public string client_contact_email { get; set; }

    public string client_etax_name { get; set; }
    public string client_etax_tel { get; set; }
    public string client_etax_email { get; set; }

    public int? candidate_source { get; set; }

    public string candidate_source_txt { get; set; }

    public int? candidate_position { get; set; }

    public string candidate_position_txt { get; set; }

    public List<ProjectInvoiceChargeModel> chargeList { get; set; }

    public project project_info { get; set; }
  }
}