using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request.MyList;

namespace Univision.Main.Models.MyList
{
    public class AlarmHistoryListViewModel : EntityListViewModel
    {
        public AlarmHistorySearchModel search { get; set; }
        public List<alarm_message> list { get; set; }
    }

}