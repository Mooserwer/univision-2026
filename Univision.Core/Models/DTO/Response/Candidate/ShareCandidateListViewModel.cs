using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
    public class ShareCandidateListViewModel : EntityListViewModel
    {
        public ShareCandidateSearchModel search { get; set; }
        public List<board> list { get; set; }
    }

    public class ShareCandidateSearchModel
    {
        public string total { get; set; } = "";
        public int? uv_seq { get; set; } = 0;
        public string search_option { get; set; } = "";
        public string search_txt { get; set; } = "";
        

    }
}
