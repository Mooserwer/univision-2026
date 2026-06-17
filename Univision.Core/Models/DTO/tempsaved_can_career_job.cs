using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    [Table("tempsaved_can_career_job")]
    public partial class tempsaved_can_career_job
    {
        /// <summary>
        /// can_career 테이블 seq
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public int cc_seq { get; set; }

        /// <summary>
        /// 직무 3단계 코드
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public double job_code3 { get; set; }

        /// <summary>
        /// 직무 3단계 명
        /// <summary>
        public string job_name3 { get; set; }
    }

    public partial class tempsaved_can_career_job
    {
        [NotMapped]
        public double code1 { get; set; }
        [NotMapped]
        public double code2 { get; set; }
    }
}
