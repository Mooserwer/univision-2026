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
    [Table("code_can_education")]
    //후보자 학력 코드
    public partial class code_can_education
    {
        /// <summary>
        /// 코드
        /// <summary>
        [Key]
        public float? code { get; set; }

        /// <summary>
        /// 명칭
        /// <summary>
        public string name { get; set; }

    }
}


