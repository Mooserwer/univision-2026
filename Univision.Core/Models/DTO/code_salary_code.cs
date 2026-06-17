using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //연봉 테이블
    [Table("code_salary_code")]
    public partial class code_salary_code
    {
        /// <summary>
        /// 코드
        /// <summary>
        [Key]
        public float code { get; set; }

       /// <summary>
       /// 연봉 정보
       /// <summary>
       public string salary_name { get; set; }

    }
}


