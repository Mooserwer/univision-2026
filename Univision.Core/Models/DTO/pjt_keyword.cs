using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO
{
    //프로젝트 키워드 테이블
    [Table("pjt_keyword")]
    public partial class pjt_keyword
    {
        /// <summary>
        /// 프로젝트 seq
        /// <summary>
        public int? p_seq { get; set; }

        /// <summary>
        /// 키워드 seq
        /// <summary>
        [Key]
        public int? pk_seq { get; set; }

        /// <summary>
        /// 키워드 명
        /// <summary>
        public string pk_name { get; set; }
    }
}
