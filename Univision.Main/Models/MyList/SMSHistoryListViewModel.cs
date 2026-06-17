using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request.MyList;

namespace Univision.Main.Models.MyList
{
    public class SMSHistoryListViewModel : EntityListViewModel
    {
        public SmsHistorySearchModel search { get; set; }
        public List<sms_history> list { get; set; }
    }

}