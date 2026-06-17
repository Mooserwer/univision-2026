using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
    public class UnivisionCandidateListModel
    {
        public UnivisionCandidateSearchModel search { get; set; }

        public List<candidate> list { get; set; }
    }



    public class UnivisionCandidateSearchModel
    {
        public int is_direct { get; set; }
        public int uv_seq { get; set; }

        public string from_month_str { get; set; }
        public string to_month_str { get; set; }

        public string search_txt { get; set; }
    }
}
