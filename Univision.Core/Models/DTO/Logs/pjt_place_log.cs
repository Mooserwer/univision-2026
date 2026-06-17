using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Univision.Core.Models.DTO.Logs
{
    [Table("pjt_place_log")]
    public partial class pjt_place_log
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
        public int p_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int pp_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string code1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string area1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string code2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string area2 { get; set; }

    }
}
