using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
    public class TempCandidateListViewModel : EntityListViewModel
    {
        public TempCandidateSearchModel search { get; set; }
        public List<tempsaved_candidate> list { get; set; }
    }

    public class TempCandidateSearchModel
    {
        public string total { get; set; } = "";
        public int? uv_seq { get; set; } = 0;
        public string search_option { get; set; } = "";
        public string search_txt { get; set; } = "";
        

    }
}
