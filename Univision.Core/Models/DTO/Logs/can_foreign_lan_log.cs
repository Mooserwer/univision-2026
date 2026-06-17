using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{

    [Table("can_foreign_lan_log")]
    //
    public partial class can_foreign_lan_log
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
        public int? event_uv_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? event_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        [Key]
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string code { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int ability { get; set; }

    }
}
