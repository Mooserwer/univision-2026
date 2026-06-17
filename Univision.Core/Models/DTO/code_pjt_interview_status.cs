using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //프로젝트 인터뷰 상태 테이블
    [Table("code_pjt_interview_status")]
    public partial class code_pjt_interview_status
    {
        /// <summary>
        /// 코드
        /// <summary>
        [Key]
        public int cpi_code { get; set; }

       /// <summary>
       /// 코드 명
       /// <summary>
       public string code_name { get; set; }

    }
}


