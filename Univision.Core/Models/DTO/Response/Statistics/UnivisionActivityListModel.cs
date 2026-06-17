using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
  public class UnivisionActivityListModel
  {
    public UnivisionActivitySearchModel search { get; set; }

    public List<uv_user> userList { get; set; }

    public List<UnivisionActivityModel> list { get; set; }
  }

  public class UnivisionActivityModel
  {
    public string mon { get; set; }
    public int can_create_cnt { get; set; }
    public int can_create_cnt_agt { get; set; }
    public int can_update_cnt { get; set; }
    public int can_create_update_cnt { get; set; }
    public int can_memo_cnt { get; set; }
    public int pjt_memo_cnt { get; set; }
    public int can_pjt_memo_cnt { get; set; }
    public int pjt_total_cnt { get; set; }
    public int pjt_cnt { get; set; }
    public int pjt_rec_cnt { get; set; }
    public int pjt_interview_cnt { get; set; }
    public int pjt_nego_cnt { get; set; }
    public int pjt_invoice_cnt { get; set; }
    public int pjt_hire_cnt { get; set; }
  }


  public class UnivisionActivitySearchModel
  {
    public int uv_seq { get; set; }

    public string startDt { get; set; }
    public string endDt { get; set; }
  }
}
