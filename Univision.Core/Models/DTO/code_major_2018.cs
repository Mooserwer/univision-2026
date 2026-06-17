using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //2018 학과 테이블
    [Table("code_major_2018")]
    public partial class code_major_2018
    {
        /// <summary>
        /// 학과 코드
        /// <summary>
        [Key]
        public string major_code { get; set; }

       /// <summary>
       /// 학과명
       /// <summary>
       public string major_name { get; set; }

       /// <summary>
       /// 구분
       /// <summary>
       public string gubun { get; set; }

       /// <summary>
       /// 카테고리1 명
       /// <summary>
       public string category1_name { get; set; }

       /// <summary>
       /// 카테고리1 코드
       /// <summary>
       public string category1_code { get; set; }

       /// <summary>
       /// 카테고리2 명
       /// <summary>
       public string category2_name { get; set; }

       /// <summary>
       /// 카테고리2 코드
       /// <summary>
       public string category2_code { get; set; }

       /// <summary>
       /// 카테고리3 명
       /// <summary>
       public string category3_name { get; set; }

       /// <summary>
       /// 카테고리3 코드
       /// <summary>
       public string category3_code { get; set; }

    }
}


