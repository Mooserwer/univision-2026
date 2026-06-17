using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Project
{
  public class WeeklyReportListModel
  {
    public WeeklyReportSearchModel search { get; set; }
    public List<uv_division> deptList { get; set; }
    public List<WeeklyReportModel> list { get; set; }
    public double? sum_point { get; set; }
    public List<WeeklyReportModel> successList { get; set; }
    public List<WeeklyReportModel> completeList { get; set; }
  }

  public class WeeklyReportModel
  {
    public int p_seq { get; set; }
    public int client_seq { get; set; }
    public string client_name { get; set; }
    public string title { get; set; }
    public int ud_seq { get; set; }
    public int ua_seq { get; set; }
    public DateTime? retire_date { get; set; }
    public string name { get; set; }
    public string expertise { get; set; }
    public string am_name { get; set; }
    public int is_no_invoice { get; set; }
    public double? point { get; set; }
    public string searcher_name { get; set; }
    public int hire_candidate_seq { get; set; }
    public string hire_candidate_name { get; set; }
    public DateTime? hire_candidate_date { get; set; }
    public int interestCnt { get; set; }
    public int no_interestCnt { get; set; }
    public int recommandCnt { get; set; }
    public int afterInterviewCnt { get; set; }
    public int negoCnt { get; set; }
    public int pjt_status { get; set; }
    public int pjt_type { get; set; }
    public DateTime? create_dt { get; set; }
    public DateTime? close_dt { get; set; }
    public DateTime? modify_dt { get; set; }
    public DateTime? billing_dt { get; set; }
    public int? is_posting { get; set; }

    public int is_invoice { get; set; }

    public int? invoice_type { get; set; }
  }

  public class WeeklyReportSearchModel
  {
    public int include_ended { get; set; } = 0;
    public int ud_seq { get; set; }
    public string startDt { get; set; }
    public string endDt { get; set; }
  }
}
