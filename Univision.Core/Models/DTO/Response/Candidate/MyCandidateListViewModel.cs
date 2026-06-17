using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
    public class MyCandidateListViewModel : EntityListViewModel
    {
        public MyCandidateSearchModel search { get; set; }
        public List<can_interest> list { get; set; }
    }

    public class MyCandidateSearchModel
    {
        public string searchOption { get; set; } = "";
        public string searchTxt { get; set; } = "";

        public string orderOption { get; set; } = "DESC";
        public string orderTxt { get; set; } = "create_dt";
    }
}
