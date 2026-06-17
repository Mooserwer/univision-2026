using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    [Table("makeup_request")]
    public partial class makeup_request
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int mr_idx { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int req_user { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int p_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? request_date { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string resume_type { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? cr_seq { get; set; }
        public string cr_dir { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public DateTime? receipt_date { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int receipt_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? complete_date { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int complete_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? del_date { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? del_yn { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int del_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int reg_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? reg_date { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int mod_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? mod_date { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string status { get; set; }

    }

    public partial class makeup_request
    {
        [NotMapped]
        public string name { get; set; }
        [NotMapped]
        public string kor_name { get; set; }
        [NotMapped]
        public string title { get; set; }
    }
}


