using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
    public class UnivisionMemoListModel
    {
        public UnivisionMemoSearchModel search { get; set; }

        public List<can_activity> list { get; set; }
    }



    public class UnivisionMemoSearchModel
    {
        public int uv_seq { get; set; }

        public string from_month_str { get; set; }
        public string to_month_str { get; set; }

        public string search_txt { get; set; }
    }

}
