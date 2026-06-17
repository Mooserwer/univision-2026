using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //프로젝트 산업 코드 테이블
    [Table("pjt_business_code")]
    public partial class pjt_business_code
    {
        /// <summary>
        /// project pk
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public int p_seq { get; set; }

        /// <summary>
        /// code_business3 code1
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public double code1 { get; set; }

        /// <summary>
        /// code_business3 code2
        /// <summary>
        [Key]
        [Column(Order = 3)]
        public double code2 { get; set; }

        /// <summary>
        /// code_business3 code3
        /// <summary>
        [Key]
        [Column(Order = 4)]
        public double code3 { get; set; }

    }

    public partial class pjt_business_code
    {
        [NotMapped]
        public string code_name3 { get; set; }
    }
}


