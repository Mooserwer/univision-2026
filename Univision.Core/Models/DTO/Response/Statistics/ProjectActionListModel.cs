using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
  public class ProjectActionListModel
  {
    public ProjectActionSearchModel search { get; set; }

    public List<uv_user> userList { get; set; }

    public List<ProjectActionModel> list { get; set; }
  }

  public class ProjectActionModel
  {
    public int p_seq { get; set; }
    public string create_dt { get; set; }
    public string close_dt { get; set; }
    public string status_comment { get; set; }
    public string client_kor_name { get; set; }
    public string pjt_title { get; set; }
    public int pjt_status { get; set; }
    public string pjt_status_name { get; set; }
    public string am { get; set; }
    public string sm { get; set; }
    public string c_seq { get; set; }
    public string recan_name { get; set; }
    public string recan_status { get; set; }
    public string recan_schedule_dt { get; set; }
    public string recan_memo { get; set; }
    public string recan_create_user_name { get; set; }
    public string recan_create_dt { get; set; }
  }


  public class ProjectActionSearchModel
  {
    public int uv_seq { get; set; }
    public int pjt_status { get; set; }
    public int recan_status { get; set; }
    public string startDt { get; set; }
    public string endDt { get; set; }
  }
}
