using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{
    [Table("pjt_recandidate_history_log")]
    public partial class pjt_recandidate_history_log
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
        public int prc_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int pic_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int p_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? state { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? schedule_date { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? annual_income { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? guarantee { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string contents { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string file_directory { get; set; }

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
        public int? is_schedule { get; set; }

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
