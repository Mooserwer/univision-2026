using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
    public class DormantListViewModel : EntityListViewModel
    {
        public DormantSearchModel search { get; set; }
        public List<client> list { get; set; }
    }

    public class DormantSearchModel
    {
        public string kor_name { get; set; } = "";
        public string ceo { get; set; }
        public string contact_name { get; set; }
        public string contact_email { get; set; }
        public string am_name { get; set; }
        public string Searcher { get; set; }
        public string biz_num { get; set; } = "";

        public string orderOption { get; set; } = "DESC";
        public string orderTxt { get; set; } = "last_project_dt";
        public bool excelDown { get; set; } = false;

        public List<code_business2> business { get; set; } = new List<code_business2>();

  }
}
