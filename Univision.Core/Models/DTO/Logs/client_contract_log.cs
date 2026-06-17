using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Univision.Core.Models.DTO.Logs
{
    [Table("client_contract_log")]
    public partial class client_contract_log
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
        [Key]
        public int cc_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int c_seq { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public string contract_date { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string fee_type { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public string deposit_limit { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string guarntee_type { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public string is_construct_debut { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public decimal? fix_fee_rate { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string file_directory { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public string draft_contract_origin_path { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public string draft_contract_path { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public string manual_contract_origin_path { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public string manual_contract_path { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public string final_contract_origin_path { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public string final_contract_path { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public string contract_comment { get; set; }


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
        
    }
}
