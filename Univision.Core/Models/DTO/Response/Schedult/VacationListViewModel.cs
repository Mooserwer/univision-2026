using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Schedult
{
    public class VacationListViewModel : EntityListViewModel
    {
        public String vacation_search { get; set; } = "";
        public List<VacationList> list { get; set; }
    }
        
    public class VacationList
  {
        public string name { get; set; } = String.Empty;
        public string rank_name { get; set; }
        public string email { get; set; }
        public string tel { get; set; }
        public string hp { get; set; }
        public string ud_name { get; set; }
        public string v_type_str { get; set; }
        public int? v_type { get; set; }
        
    }    
    
}
