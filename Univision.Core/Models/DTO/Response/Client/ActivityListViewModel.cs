using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
    public class ActivityListViewModel : EntityListViewModel
    {
        public ActivitySearchModel search { get; set; }        
        public List<client_activity_log> list { get; set; }
        
    }


    
    public class ActivitySearchModel
    {
        public bool is_my_history { get; set; } = true;
        public string search_txt { get; set; } = "";
        public string kor_name { get; set; }
        public string eng_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string am_name { get; set; }
        public string dateStart { get; set; }
        public string dateEnd { get; set; }
        public string createStart { get; set; }
        public string createEnd { get; set; }
        public string orderOption { get; set; } = "DESC";
        public string orderTxt { get; set; } = "log_date";

    }    
}
