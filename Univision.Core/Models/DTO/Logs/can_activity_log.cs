using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{
    //후보자 진행상황 로그테이블
    [Table("can_activity_log")]
    public partial class can_activity_log
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
        public int ca_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? cl_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string ca_status { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? ca_date { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string memo { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? create_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? modify_seq { get; set; }

    }
}
