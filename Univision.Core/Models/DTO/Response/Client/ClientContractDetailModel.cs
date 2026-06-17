using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
    public class ClientContractDetailModel
    {
        public client_contract data { get; set; }
        public List<client_annual_income_rate> feeList { get; set; }
        public List<client_contract_file> fileList { get; set; }
        
    }
}
