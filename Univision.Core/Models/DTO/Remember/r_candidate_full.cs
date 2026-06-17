using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public class r_candidate_full : r_candidate
  {
    public List<r_can_school> can_school { get; set; } = new List<r_can_school>();
    public List<r_can_career> can_career { get; set; } = new List<r_can_career>();
    public List<r_can_business> can_business { get; set; } = new List<r_can_business>();
    public List<r_can_job> can_job { get; set; } = new List<r_can_job>();
    public List<r_can_foreign_lan> can_foreign_lan { get; set; } = new List<r_can_foreign_lan>();
    public List<r_can_activity> can_activity { get; set; } = new List<r_can_activity>();
    public r_can_resume can_resume { get; set; }

    public List<r_can_resume_file_info> can_resume_list { get; set; } = new List<r_can_resume_file_info>();

  }
}
