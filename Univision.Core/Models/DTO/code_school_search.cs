using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //학교 테이블
    [Table("code_school_search")]
    public partial class code_school_search
  {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int css_seq { get; set; }


        /// <summary>
        /// 학교 명
        /// <summary>
        public string school_name { get; set; }

        /// <summary>
        /// 검색키워드
        /// <summary>
        public string search_str { get; set; }
    }
}


