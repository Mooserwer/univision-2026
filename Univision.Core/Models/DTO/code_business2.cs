using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //업종 테이블 2
    [Table("code_business2")]
    public partial class code_business2
    {
        /// <summary>
        /// 업종 코드1
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public float code1 { get; set; }

        /// <summary>
        /// 업종 코드2
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public float code2 { get; set; }

       /// <summary>
       /// 업종 명2
       /// <summary>
       public string code_name2 { get; set; }

    }
}


