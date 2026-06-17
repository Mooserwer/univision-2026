using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
    public class CandidateHistoryListViewModel : EntityListViewModel
    {
        public CandidateHistorySearchModel search { get; set; }
        public List<CandidateHistoryDateGroupModel> list { get; set; } = new List<CandidateHistoryDateGroupModel>();
    }

    public class CandidateHistorySearchModel
    {
        public string search_str { get; set; } = "";
        public string search_txt { get; set; } = "";
        public int history_type{ get; set; }
        public bool is_my_history { get; set; } = true;
        public string start_dt { get; set; }
        public string end_dt { get; set; }
    }

    public class CandidateHistoryDateGroupModel
    {
        public string group_date { get; set; }

        public List<CandidateHistoryGroupModel> group_list {get;set;} = new List<CandidateHistoryGroupModel>();
    }

    public class CandidateHistoryGroupModel
    {
        public string table_type { get; set; }
        public int p_seq { get; set; }
        public int c_seq { get; set; }
        public DateTime? end_dt { get; set; }
        public DateTime? start_dt { get; set; }
        public string dot_color { get; set; }
        public List<view_can_activity> activityList { get; set; } = new List<view_can_activity>();
    }
}
