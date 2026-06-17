using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Client
{
    public class ClientCreateUpdateModel
    {
        public client data { get; set; } = new client();

        public client_contact data2 { get; set; } = new client_contact();

        public client_tax_contact data3 { get; set; } = new client_tax_contact();

        public client_annual_income_rate data4 { get; set; } = new client_annual_income_rate();

        public List<client_manager> amList { get; set; } = new List<client_manager>();

        public List<client_manager> deleteAmList { get; set; } = new List<client_manager>();

    }
}
