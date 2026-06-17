using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Client
{
    public class ClientContractSaveModel
    {
        public client_contract contract { get; set; }
        public List<client_annual_income_rate> annualIncomeList { get; set; }
        public List<client_position_rate> positionRateList { get; set; }
    }
}