using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Logs;

namespace Univision.Main.Models.Client
{
  public class ClientCreateModel
  {
    //public List<client> data { get; set; }
    //public List<project> 


    public int c_seq { get; set; }

    public int log_cnt { get; set; }
    public int ac_seq { get; set; }
    public string kor_name { get; set; }
    public string eng_name { get; set; }
    public string ceo { get; set; }
    public string addr1 { get; set; }
    public string addr2 { get; set; }
    public int is_foreign_invest { get; set; }

    public int is_inorder { get; set; }

    public int is_contract { get; set; }
    public int is_foreign { get; set; }
    public int is_portfolio { get; set; }
    public string foreign_code { get; set; }
    public string country_name { get; set; }
    public string biz_code { get; set; }
    public string biz_type { get; set; }
    public string biz_type_code { get; set; }
    public string biz_category { get; set; }
    public string biz_category_code { get; set; }
    public string biz_industry { get; set; }
    public double? biz_industry_code1 { get; set; }
    public double? biz_industry_code2 { get; set; }
    public double? busiCode1 { get; set; }
    public double? busiCode2 { get; set; }
    public string business { get; set; }
    public string business_name1 { get; set; }
    public string business_name2 { get; set; }
    public string am_name { get; set; }
    public string ud_name { get; set; }
    public int? bd_user_seq { get; set; }
    public string bd_user_name { get; set; }
    public string fix_title { get; set; }
    public string homepage { get; set; }
    public int? employee_number { get; set; }
    public int? sales_amount { get; set; }
    public int main_contract { get; set; }
    public int offlimit { get; set; }
    public string offlimit_keyword { get; set; }
    public DateTime? create_dt { get; set; }
    public int? create_user { get; set; }
    public DateTime? modify_dt { get; set; }
    public int? modify_user { get; set; }
    public string create_name { get; set; }
    public string modify_name { get; set; }

    // Contact
    public int contact_seq { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public int gender { get; set; }
    public string phone { get; set; }
    public string cell_phone { get; set; }
    public string division { get; set; }
    public string position { get; set; }

    // 세금계산서 담당
    public int tax_seq { get; set; }
    public string tax_name { get; set; }
    public string tax_division { get; set; }
    public string tax_email { get; set; }
    public string tax_phone { get; set; }
    public string tax_cell_phone { get; set; }
    public string tax_deposit_manager { get; set; }
    public string tax_deposit_email { get; set; }

    //계약관련
    public int cc_seq { get; set; }
    public string contract_date { get; set; }
    public string fee_type { get; set; }
    public string deposit_limit { get; set; }
    public string guarntee_type { get; set; }
    public string is_construct_debut { get; set; }
    public float? fix_fee_rate { get; set; }
    public string contract_comment { get; set; }
    public string draft_contract_path { get; set; }
    public string manual_contract_path { get; set; }
    public string final_contract_path { get; set; }
    public string file_directory { get; set; }
    public string fee_content { get; set; }
    public string currency_name { get; set; }
    public string incom_detail { get; set; }
    public string position_detail { get; set; }
    public string feeValue { get; set; }

    public int checkValue { get; set; } = 0;

    public List<client_manager> amList { get; set; } = new List<client_manager>();
    public List<client_contract_file> CfileList { get; set; } = new List<client_contract_file>();
    public List<client_contract_file> DfileList { get; set; } = new List<client_contract_file>();
    public List<client_contract_file> FfileList { get; set; } = new List<client_contract_file>();



    public int is_external_lock { get; set; } = 0;

    public int i_seq { get; set; } = 0;
  }
}