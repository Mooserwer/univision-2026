using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //프로젝트 간편후보 테이블
    [Table("pjt_tmp_icandidate")]
    public partial class pjt_tmp_icandidate
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int pti_seq { get; set; }

       /// <summary>
       /// simple_candidate pk
       /// <summary>
       public int sc_seq { get; set; }

       /// <summary>
       /// project pk
       /// <summary>
       public int p_seq { get; set; }

       /// <summary>
       /// 메모
       /// <summary>
       public int? memo { get; set; }

    }
}


