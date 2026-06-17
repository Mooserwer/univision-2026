using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Univision.Core.Models.DTO.Logs
{
    [Table("pjt_manager_log")]
    public partial class pjt_manager_log
    {
        /// <summary>
        /// 
        /// <summary> 
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

    public partial class pjt_manager_log
    {
        /// <summary>
        /// 담당자 이름
        /// </summary>
        [NotMapped]
        public string name { get; set; }

        /// <summary>
        /// 담당자 부서명
        /// </summary>
        [NotMapped]
        public string ud_name { get; set; }
    }
}
