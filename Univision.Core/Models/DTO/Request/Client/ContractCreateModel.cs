using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Client
{
    public class ContractCreateModel
    {
        public string edit_mode { get; set; }
        public client_contract data { get; set; }

        public List<client_annual_income_rate> fee_step { get; set; } = new List<client_annual_income_rate>();

        public List<client_annual_income_rate> del_fee_step { get; set; } = new List<client_annual_income_rate>();
    }
}
