using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //업종 테이블 3
    [Table("code_business3")]
    public partial class code_business3
    {
        /// <summary>
        /// 업종코드1
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public float? code1 { get; set; }

        /// <summary>
        /// 업종코드2
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public float code2 { get; set; }

        /// <summary>
        /// 업종코드3
        /// <summary>
        [Key]
        [Column(Order = 3)]
        public float code3 { get; set; }

        /// <summary>
        /// 업종 명3
        /// <summary>
        public string code_name3 { get; set; }

    }
}


