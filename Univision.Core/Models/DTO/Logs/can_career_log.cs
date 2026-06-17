using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{

    [Table("can_career_log")]
    //
    public partial class can_career_log
    {
        /// <summary>
        /// 
        /// <summary> 
        [Key]
        public int log_seq { get; set; }
        /// <summary>
        /// 
        /// <summary> 
        [Key]
        public int log_can_seq { get; set; }

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
        public int cc_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        [Key]
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? g_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string company_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string division_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string join_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string leave_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? annual_income { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_work { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? r_code { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? p_code { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? order_no { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string position_str { get; set; }

    }

    public partial class can_career_log
    {
        [NotMapped]
        public string r_name { get; set; }
        [NotMapped]
        public string p_name { get; set; }
    }
}
