using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //상위 업종 테이블 1
    [Table("code_business1")]
    public partial class code_business1
    {
        /// <summary>
        /// 코드
        /// <summary>
        [Key]
        public float? code1 { get; set; }

       /// <summary>
       /// 상위 업종명
       /// <summary>
       public string code_name1 { get; set; }

    }
}


