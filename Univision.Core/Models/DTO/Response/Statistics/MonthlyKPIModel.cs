using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
  public class MonthlyKPIModel
  {
    public MonthlyKPISearchModel search { get; set; }

    public List<uv_user> userList { get; set; }

    public List<MonthlyKPI> list { get; set; }
  }

  public class MonthlyKPI
  {    
    public string uv_name { get; set; }
    public string ud_name { get; set; }
    public string mon { get; set; }
    public int can_reg_dr { get; set; }
    public int can_reg_ag { get; set; }
    public int can_udt { get; set; }
    public int can_reg_all { get; set; }
    public int can_memo { get; set; }
    public int rec_cnt { get; set; }
    public int rec_pjt_cnt { get; set; }
    public int int_cnt { get; set; }
    public float month_bill { get; set; }
    public float last_year_bill { get; set; }
    public int pjt_cnt_am { get; set; }
    public int pjt_cnt_sm { get; set; }

  }


  public class MonthlyKPISearchModel
  {
    public int uv_seq { get; set; }
    public int ud_seq { get; set; }
    public string startDt { get; set; }
    public string endDt { get; set; }
  }
}
