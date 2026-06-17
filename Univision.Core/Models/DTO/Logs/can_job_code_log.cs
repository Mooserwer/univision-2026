using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Univision.Core.Models.DTO.Logs
{

    [Table("can_job_code_log")]
    //
    public partial class can_job_code_log
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
        public int log_can_Seq { get; set; }

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
        public float code1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float code2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? code3 { get; set; }

    }
    public partial class can_job_code_log
    {
        [NotMapped]
        public string code_name2 { get; set; }
        [NotMapped]
        public string code_name3 { get; set; }
    }
}
