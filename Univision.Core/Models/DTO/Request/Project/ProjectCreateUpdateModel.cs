using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Candidate
{
    public class ProjectCreateUpdateModel
    {
        public project data { get; set; } = new project();
        public List<pjt_director> amList { get; set; } = new List<pjt_director>();
        public List<pjt_manager> searcherList { get; set; } = new List<pjt_manager>();
        //public List<pjt_business_code> busiList { get; set; } = new List<pjt_business_code>();
        //public List<pjt_job_code> jobList { get; set; } = new List<pjt_job_code>();
        public List<pjt_place> placeList { get; set; } = new List<pjt_place>();
        public List<pjt_keyword> keywordList { get; set; } = new List<pjt_keyword>();

        public List<pjt_director> deleteAmList { get; set; } = new List<pjt_director>();
        public List<pjt_manager> deleteSearcherList { get; set; } = new List<pjt_manager>();
        //public List<pjt_business_code> deleteBusiList { get; set; } = new List<pjt_business_code>();
        //public List<pjt_job_code> deleteJobList { get; set; } = new List<pjt_job_code>();
        public List<pjt_place> deletePlaceList { get; set; } = new List<pjt_place>();
        public List<pjt_keyword> deleteKeywordList { get; set; } = new List<pjt_keyword>();

    }

    
}
