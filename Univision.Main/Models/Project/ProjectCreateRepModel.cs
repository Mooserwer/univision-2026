using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Project
{
  public class ProjectCreateRepModel
  {
    //프로젝트 
    public int p_seq { get; set; }
    public int c_seq { get; set; }
    public int? cc_seq { get; set; }
    public int? ctc_seq { get; set; }
    public string company_name { get; set; }
    public int pjt_type { get; set; }
    public string title { get; set; }
    public string client_require { get; set; }
    public string internal_note { get; set; }
    public int? expected_salary { get; set; }
    public double? exp_salary { get; set; }
    public double? exp_salary_won { get; set; }
    public string currency_cd { get; set; }
    public decimal? fee_rate { get; set; } = 100;
    public double? pjt_status { get; set; }
    public string status_comment { get; set; }
    public double? business_code1 { get; set; }
    public double? business_code2 { get; set; }
    public string business_name1 { get; set; }
    public string business_name2 { get; set; }
    public int job_code1 { get; set; }
    public int job_code2 { get; set; }
    public string job_name1 { get; set; }
    public string job_name2 { get; set; }
    public int? is_share_pjt { get; set; }
    public int? is_cowork { get; set; }

    //전자 세금계산서 담당자
    public string tax_division { get; set; }
    public string tax_name { get; set; }
    public string tax_email { get; set; }
    public string tax_phone { get; set; }
    public string tax_cell_phone { get; set; }
    public string tax_deposit_manager { get; set; }
    public string tax_deposit_email { get; set; }

    //담당자
    public string contact_name { get; set; }
    public int contact_gender { get; set; }
    public string contact_email { get; set; }
    public string contact_phone { get; set; }
    public string contact_cell_phone { get; set; }
    public string contact_division { get; set; }
    public string contact_position { get; set; }

    public List<pjt_director> amList { get; set; } = new List<pjt_director>();
    public List<pjt_manager> searcherList { get; set; } = new List<pjt_manager>();

    public string type { get; set; }
  }


}