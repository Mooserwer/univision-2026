using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{
    [Table("client_favorite_log")]
    public partial class client_favorite_log
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
        public int cf_seq { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int uv_seq { get; set; }


    }
}
