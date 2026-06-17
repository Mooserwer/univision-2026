using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //프로젝트 테이블
    [Table("project_read_history")]
    public partial class project_read_history
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int ph_seq { get; set; }

        /// <summary>
        /// 프로젝트 p_seq
        /// <summary>
        public int p_seq { get; set; }

        /// <summary>
        /// 읽음사용자
        /// <summary>
        public int? read_user { get; set; }

        /// <summary>
        /// 읽음시간
        /// <summary>
        public DateTime? read_dt { get; set; }

    }

    public partial class project_read_history
    {
        
        /// <summary>
        /// 조회
        /// </summary>
        [NotMapped]
        public string read_name { get; set; }

    }
}


