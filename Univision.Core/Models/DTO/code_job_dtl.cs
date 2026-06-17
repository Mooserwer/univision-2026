using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //직종 테이블 2
    [Table("code_job_dtl")]
    public partial class code_job_dtl
    {
        /// <summary>
        /// 직종코드1
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public float code1 { get; set; }

        /// <summary>
        /// 직종코드2
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public float code2 { get; set; }

        /// <summary>
        /// 직종 명2
        /// <summary>
        public string code_name2 { get; set; }

        /// <summary>
        /// 사용여부
        /// <summary>
        public float? is_hide { get; set; }

        /// <summary>
        /// 순번
        /// <summary>
        public int? order_no { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? modify_dt { get; set; }

    }

    public partial class code_job_dtl
    {
        [NotMapped]
        public string code_name1 { get; set; }
    }
}


