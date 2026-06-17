using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Project
{
    public class MakeupRequestListModel
    {
        public MakeupRequestSearchModel search { get; set; }

        public List<makeup_request> list { get; set; }
    }



    public class MakeupRequestSearchModel
    {
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int search_status { get; set; }
        public string search_field { get; set; }
        public string search_value { get; set; }

    }
}
