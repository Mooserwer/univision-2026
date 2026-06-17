using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{
    [Table("pjt_director_log")]
    public partial class pjt_director_log
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
        public int uv_seq { get; set; }

    }
    public partial class pjt_director_log
    {
        [NotMapped]
        public string name { get; set; }
        [NotMapped]
        public string ud_name { get; set; }
    }
}
