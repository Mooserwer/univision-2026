using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{
    [Table("can_school_log")]
    public partial class can_school_log
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
        public int cs_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int sc_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string gubun { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string sch1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string graduate_year { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string admission_year { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? graduate_status { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_transfer { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? credit { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? total_credit { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string category1_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string major_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string sub_category1_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string sub_major_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_foreign_school { get; set; }

        public int? order_no { get; set; }
    }

    public partial class can_school_log
    {
        [NotMapped]
        public string schoolName { get; set; }
        [NotMapped]
        public string campusName { get; set; }
    }
}
