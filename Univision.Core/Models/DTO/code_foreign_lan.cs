using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //외국어 코드 테이블
    [Table("code_foreign_lan")]
    public partial class code_foreign_lan
    {
        /// <summary>
        /// 코드
        /// <summary>
        [Key]
        public string code { get; set; }

       /// <summary>
       /// 영문 외국어명
       /// <summary>
       public string eng_foreign_name { get; set; }

       /// <summary>
       /// 한국 외국어명
       /// <summary>
       public string kor_foreign_name { get; set; }

       /// <summary>
       /// 영문 우선순위 순서
       /// <summary>
       public float eng_sort { get; set; }

       /// <summary>
       /// 한글 우선순위 순서
       /// <summary>
       public float kor_sort { get; set; }

    }
}


