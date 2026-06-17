using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
  public class CandidateRegStatusModel
  {
    public CandidateRegSearchModel search { get; set; }

    public List<uv_user> userList { get; set; }

    public List<CandidateRegModel> list { get; set; }
  }

  public class CandidateRegModel
  {    
    public string reg_name { get; set; }
    public int code1 { get; set; }
    public string code1_name { get; set; }
    public int code2 { get; set; }
    public string code2_name { get; set; }
    public int reg_count { get; set; }
    
  }


  public class CandidateRegSearchModel
  {
    public int uv_seq { get; set; }
    public string startDt { get; set; }
    public string endDt { get; set; }
  }
}
