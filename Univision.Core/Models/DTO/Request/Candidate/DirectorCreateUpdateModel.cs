using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Candidate
{
    public class DirectorCreateUpdateModel
  {
        public director data { get; set; } = new director();
        public List<director_school> schoolList { get; set; } = new List<director_school>();
        public List<director_career> careerList { get; set; } = new List<director_career>();
        public List<director_job> jobList { get; set; } = new List<director_job>();
        public List<director_business> busiList { get; set; } = new List<director_business>();        
        public List<director_school> deleteSchoolList { get; set; } = new List<director_school>();
        public List<director_career> deleteCareerList { get; set; } = new List<director_career>();        
        public List<director_job> deleteJobList { get; set; } = new List<director_job>();
        public List<director_business> deleteBusiList { get; set; } = new List<director_business>();        
    }

}
