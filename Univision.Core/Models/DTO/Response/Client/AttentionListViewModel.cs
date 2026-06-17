using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
    public class AttentionListViewModel : EntityListViewModel
    {
        public AttentionSearchModel search { get; set; }
        public List<client_favorite> list { get; set; }
    }


    
    public class AttentionSearchModel
    {
        public string kor_name { get; set; } = "";
        public string eng_name { get; set; }
        public string ceo { get; set; }
        public string contact_name { get; set; }
        public string contact_email { get; set; }
        public string am_name { get; set; }
        public string biz_num { get; set; } = "";
        public string email { get; set; } = "";
        public string createStart { get; set; } = "";
        public string createEnd { get; set; } = "";
        public string create_user { get; set; } = "";

        public int is_foreign { get; set; } = 0;
        public int offlimit { get; set; } = 0;
        public int is_contract { get; set; } = 0;

        public string myClient { get; set; } = "";

        public int searchNum { get; set; } = 10;

        public string phone { get; set; }
        public int cf_seq { get; set; }

        public string orderOption { get; set; } = "DESC";
        public string orderTxt { get; set; } = "create_dt";

    }    
}
