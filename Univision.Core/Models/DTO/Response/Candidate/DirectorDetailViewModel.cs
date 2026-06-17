using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
  public class DirectorDetailViewModel
  {
    public director data { get; set; }
    public List<director_school> schoolList { get; set; } = new List<director_school>();
    public List<director_career> careerList { get; set; } = new List<director_career>();
    public List<director_career> DirectorList { get; set; } = new List<director_career>();
    public List<director_activity> MemoList { get; set; } = new List<director_activity>();
    public List<director_business> businessList { get; set; } = new List<director_business>();
    public List<director_job> jobList { get; set; } = new List<director_job>();

  }

  public class DirectorActivityList : EntityListViewModel
  {
    public int d_seq { get; set; }
    public List<can_activity> activityList { get; set; }
  }
  
}
