using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //직종 테이블 3
    [Table("code_job3")]
    public partial class code_job3
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
        /// 직종코드3
        /// <summary>
        [Key]
        [Column(Order = 3)]
        public float code3 { get; set; }

        /// <summary>
        /// 직종명3
        /// <summary>
        public string code_name3 { get; set; }

    }
}


