using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //프로젝트 일반 후보자 관심후보 테이블
    [Table("pjt_icandidate")]
    public partial class pjt_icandidate
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int pic_seq { get; set; }

       /// <summary>
       /// project pk
       /// <summary>
       public int p_seq { get; set; }

       /// <summary>
       /// candidate pk
       /// <summary>
       public int c_seq { get; set; }

       /// <summary>
       /// 메모
       /// <summary>
       public string memo { get; set; }

    }
}


