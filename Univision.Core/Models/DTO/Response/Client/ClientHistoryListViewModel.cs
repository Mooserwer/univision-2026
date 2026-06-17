using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
    public class ClientHistoryListViewModel : EntityListViewModel
    {
        public ClientHistorySearchModel search { get; set; }
        public List<ClientHistoryDateGroupModel> list { get; set; } = new List<ClientHistoryDateGroupModel>();
    }

    public class ClientHistorySearchModel
    {
        public string search_str { get; set; } = "";
        public string search_txt { get; set; } = "";
        public int history_type{ get; set; }
        public bool is_my_history { get; set; } = true;
        public string start_dt { get; set; }
        public string end_dt { get; set; }
    }

    public class ClientHistoryDateGroupModel
    {
        public string group_date { get; set; }

        public List<ClientHistoryGroupModel> group_list {get;set;} = new List<ClientHistoryGroupModel>();
    }

    public class ClientHistoryGroupModel
    {
        public int c_seq { get; set; }
        public DateTime? end_dt { get; set; }
        public DateTime? start_dt { get; set; }
        public string dot_color { get; set; }
        public List<client_activity_log> activityList { get; set; } = new List<client_activity_log>();
    }
}
