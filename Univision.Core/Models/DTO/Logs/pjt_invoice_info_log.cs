using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{
    [Table("pjt_invoice_info_log")]
    public partial class pjt_invoice_info_log
    {
        /// <summary>
        /// 
        /// <summary> 
        [Key]
        public int log_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? event_idx { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string event_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? event_uv_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? event_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int pii_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int p_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? prc_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? join_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? billing_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? annual_income { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? fee_rate { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? billing_money { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? billing_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? expire_guarantee { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_open_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_open_annual_income { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? invoice_lang { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? invoice_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string remarks { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? confirm_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? confirm_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? send_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? send_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? create_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? modify_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string invoice_no { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_file { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string file_dir { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string file_origin_path { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string file_path { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string file_extension { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? deposit_amt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? deposit_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? file_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? file_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string po_no { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string pjt_title { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string candidate_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_ceo { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_addr1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_biz_code { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_fee_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_contact_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_contact_email { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_contact_phone { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_contact_cell_phone { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_contact_division { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? billing_won { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string sr_file { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? billing_total { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? billing_vat { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string etax_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string etax_email { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string etax_phone { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? etax_date { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string bill_currency_cd { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string income_currency_cd { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? vat_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_po_no { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string remark_admin { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_deleted { get; set; }

    }
}
