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
    [Table("code_job2")]
    public partial class code_job2
    {
        /// <summary>
        /// 직종코드1
        /// <summary>
        [Key]
        [Column(Order=1)]
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

    }
}


