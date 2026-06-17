using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
  public class MailResumeListModel
  {
    public MailResumeSearchModel search { get; set; }

    public List<uv_user> userList { get; set; }

    public List<MailResumeData> list { get; set; }
  }


  public class MailResumeSearchModel
  {
    public int uv_seq { get; set; }
    public int ud_seq { get; set; }
    public string startDt { get; set; }
    public string endDt { get; set; }
    public int is_progress_only { get; set; } = 1;
  }

  public class MailResumeData
  {
  
    public string dv_timestamp { get; set; }
    public string dd_rcv { get; set; }
    public string dv_snd_name { get; set; }
    public string dv_snd_addr { get; set; }
    public string dv_rcv_name { get; set; }
    public string dv_subject { get; set; }
    public string candidate_seq { get; set; }
    public string candidate_name { get; set; }
    public string birth_year { get; set; }
    public string gender { get; set; }
    public string dv_update_state { get; set; }
    public string dv_del_yn { get; set; }
    public string dv_del_reason { get; set; }
    public string gpt_result { get; set; }
    public List<mail_resume_file> files { get; set; } = new List<mail_resume_file>();
  }
}
