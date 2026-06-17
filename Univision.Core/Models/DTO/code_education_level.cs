using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //학력 코드 테이블
    [Table("code_education_level")]
    public partial class code_education_level
    {
        /// <summary>
        /// 코드
        /// <summary>
        [Key]
        public float? code { get; set; }

       /// <summary>
       /// 학력 정보
       /// <summary>
       public string level_name { get; set; }

    }
}


