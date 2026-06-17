using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Candidate
{
    public class CandidateCreateUpdateModel
    {
        public candidate data { get; set; } = new candidate();
        public List<SchoolModel> schoolList { get; set; } = new List<SchoolModel>();
        public List<CareerModel> careerList { get; set; } = new List<CareerModel>();
        public List<can_place> placeList { get; set; } = new List<can_place>();
        public List<can_job> jobList { get; set; } = new List<can_job>();
        public List<can_business> busiList { get; set; } = new List<can_business>();
        public List<can_foreign_lan> foreignList { get; set; } = new List<can_foreign_lan>();

        public List<can_school> deleteSchoolList { get; set; } = new List<can_school>();
        public List<can_career> deleteCareerList { get; set; } = new List<can_career>();
        public List<can_place> deletePlaceList { get; set; } = new List<can_place>();
        public List<can_job> deleteJobList { get; set; } = new List<can_job>();
        public List<can_business> deleteBusiList { get; set; } = new List<can_business>();
        public List<can_foreign_lan> deleteForeignList { get; set; } = new List<can_foreign_lan>();
    }

    public class SchoolModel
    {
        public code_school newSchool { get; set; }
        public can_school school { get; set; }
    }

    public class CareerModel
    {
        public can_career career { get; set; } = new can_career();
        public List<can_career_job> careerJobList { get; set; } = new List<can_career_job>();
    }

    
}
