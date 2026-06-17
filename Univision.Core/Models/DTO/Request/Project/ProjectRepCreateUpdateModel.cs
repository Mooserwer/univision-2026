using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Candidate
{
    public class ProjectRepCreateUpdateModel
    {
        public project data { get; set; } = new project();
        public List<pjt_director> amList { get; set; } = new List<pjt_director>();
        public List<pjt_manager> searcherList { get; set; } = new List<pjt_manager>();
       
        public List<pjt_director> deleteAmList { get; set; } = new List<pjt_director>();
        public List<pjt_manager> deleteSearcherList { get; set; } = new List<pjt_manager>();
        
    }

    
}
