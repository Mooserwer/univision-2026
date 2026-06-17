using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Client
{
    public class ClientPopupModel
    {

        public client_contact data { get; set; }
        public client_activity_log log_data { get; set; }
        public client_tax_contact tax_data { get; set; }
        public client_annual_income_rate incom_data { get; set; }
        public client_contract contract_data { get; set; }
        //public client_file file_data { get; set; }
        //public List<client_file> ClientFileList { get; set; }

        public string title { get; set; }
        public string log_comment { get; set; }
        public string log_date { get; set; }
        public string log_hour { get; set; }
        public string log_min { get; set; }


        public string name { get; set; }
        public int gender { get; set; }
        public string division { get; set; }
        public string position { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string cell_phone { get; set; }
        public string memo { get; set; }

        public string uv_name { get; set; }

        public string deposit_manager { get; set; }
        public string deposit_email { get; set; }

        public int fix_fee_rate { get; set; }

        public int is_schedule { get; set; }

    }
}