using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Sms
{
    public class SmsServiceModel
    {
        public string response_code { get; set; }
        public string response_desc { get; set; }
        public string msgid { get; set; }
        public int smsType { get; set; }

        public string exception_message { get; set; }
    }
}
