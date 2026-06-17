using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
    public class ContactListViewModel : EntityListViewModel
    {        
        public ContactSearchModel search { get; set; }         
        public List<client_contact> list { get; set; }
    }


    public class ContactSearchModel
    {
        public string kor_name { get; set; }
        public string eng_name { get; set; }
        public string contact_name { get; set; }
        public string contact_email { get; set; }
        public string contact_division { get; set; }
        public string contact_position { get; set; }
        public string contact_phone { get; set; }
        public string contact_cell_phone { get; set; }
        public string am_name { get; set; }
        public string createStart { get; set; }
        public string createEnd { get; set; }
        public string orderOption { get; set; } = "DESC";
        public string orderTxt { get; set; } = "create_dt";
        public bool is_my_contact { get; set; } = true;
    }

    
}
