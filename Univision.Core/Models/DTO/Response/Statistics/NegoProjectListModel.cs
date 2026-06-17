using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
  public class NegoProjectListModel
  {
    public NegoProjectSearchModel search { get; set; }

    public List<uv_user> userList { get; set; }

    public List<NegoProjectStatistic> list { get; set; }
  }


  public class NegoProjectSearchModel
  {
    public int uv_seq { get; set; }
    public int ud_seq { get; set; }
    public string startDt { get; set; }
    public string endDt { get; set; }

    public int is_progress_only { get; set; } = 1;
    public int is_no_after_only { get; set; } = 1;
  }

  public class NegoProjectStatistic
  {
    public int p_seq { get; set; }
    public string client_name { get; set; }
    public string title { get; set; }
    public string pjt_status_str { get; set; }
    public string exp_salary { get; set; }
    public string exp_salary_won { get; set; }
    public string currency_cd { get; set; }
    public string fee_rate { get; set; }
    public string exp_sales { get; set; }
    public string am_names { get; set; }
    public string searcher_names { get; set; }
    public string recan_name { get; set; }
    public string recan_status { get; set; }
    public string recan_schedule_dt { get; set; }
    public string recan_memo { get; set; }
    public string recan_create_user_name { get; set; }
    public string recan_create_dt { get; set; }
    public string recan_status2 { get; set; }
    public string recan_schedule_dt2 { get; set; }
    public string recan_memo2 { get; set; }
    public string recan_create_dt2 { get; set; }
  }
}
