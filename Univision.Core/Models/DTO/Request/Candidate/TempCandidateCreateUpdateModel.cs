using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Candidate
{
    public class TempCandidateCreateUpdateModel
    {
        public int? temp_seq { get; set; }
        public tempsaved_candidate data { get; set; } = new tempsaved_candidate();
        public List<tempsaved_SchoolModel> schoolList { get; set; } = new List<tempsaved_SchoolModel>();
        public List<tempsaved_CareerModel> careerList { get; set; } = new List<tempsaved_CareerModel>();
        public List<tempsaved_can_place> placeList { get; set; } = new List<tempsaved_can_place>();
        public List<tempsaved_can_job> jobList { get; set; } = new List<tempsaved_can_job>();
        public List<tempsaved_can_business> busiList { get; set; } = new List<tempsaved_can_business>();
        public List<tempsaved_can_foreign_lan> foreignList { get; set; } = new List<tempsaved_can_foreign_lan>();
    }

    public class tempsaved_SchoolModel
    {
        public code_school newSchool { get; set; }
        public tempsaved_can_school school { get; set; }
    }

    public class tempsaved_CareerModel
    {
        public tempsaved_can_career career { get; set; } = new tempsaved_can_career();
        public List<tempsaved_can_career_job> careerJobList { get; set; } = new List<tempsaved_can_career_job>();
    }

    
}
