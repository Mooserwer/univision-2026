using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //프로젝트 추천후보 테이블
    [Table("pjt_recandidate")]
    public partial class pjt_recandidate
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
        /// 
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? create_user { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? modify_user { get; set; }


        public int pin_to_top { get; set; }
    }
}


