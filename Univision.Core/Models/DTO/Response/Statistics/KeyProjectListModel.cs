using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
  public class KeyProjectListModel
  {
    public KeyProjectSearchModel search { get; set; }

    public List<uv_user> userList { get; set; }

    public List<KeyProjectStatistic> list { get; set; }
  }


  public class KeyProjectSearchModel
  {
    public int uv_seq { get; set; }
    public int ud_seq { get; set; }
    public string startDt { get; set; }
    public string endDt { get; set; }
    public int is_progress_only { get; set; } = 1;
  }

  public class KeyProjectStatistic
  {
    public int p_seq { get; set; }
    public int uv_seq { get; set; }
    public string key_user_name { get; set; }
    public string key_create_dt { get; set; }
    public string client_name { get; set; }
    public string title { get; set; }
    public string pjt_status_str { get; set; }
    public string am_names { get; set; }
    public string searcher_names { get; set; }
    public string open_dt { get; set; }
    public string close_dt { get; set; }
    public int interest_cnt { get; set; }
    public int recommend_cnt { get; set; }
    public int interview_cnt { get; set; }
    public int nego_cnt { get; set; }
  }
}
