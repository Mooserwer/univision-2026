using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Dashboard
{
    public class UnBilledInvoiceListModel
    {
        public List<UnBilledInvoiceModel> list { get; set; }
    }

    public class UnBilledInvoiceModel
    {
        public int client_seq { get; set; }
        public string client_name { get; set; }
        public int p_seq { get; set; }
        public string title { get; set; }
        public int candidate_seq { get; set; }
        public string candidate_name { get; set; }
        public DateTime schedule_date { get; set; }
    }
}
