using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{
    [Table("client_activity_log_log")]
    public partial class client_activity_log_log
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
        public int cal_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string title { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string log_comment { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime log_date { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int is_schedule { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int create_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? modify_user { get; set; }
    }

    public partial class client_activity_log_log
    {
        [NotMapped]
        public string event_name { get; set; } = "";
        [NotMapped]
        public string name { get; set; } = "";

    }

}
