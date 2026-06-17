using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Core.Models.DTO.Request.MyList
{
    public class VacationCreateModel
    {
        public uv_vacation_history data { get; set; }
        public List<uv_vacation_history_dtl> detail_list { get; set; } = new List<uv_vacation_history_dtl>();
        
    }


}