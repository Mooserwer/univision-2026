using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //주소 테이블3
    [Table("board_read_history")]
    //
    //
    public partial class board_read_history
    {
        /// <summary>
        /// IDX
        /// <summary>
        [Key]
        public int bh_seq { get; set; }

        /// <summary>
        /// 게시판 IDX
        /// <summary>
        public int c_seq { get; set; }

        /// <summary>
        /// 읽음사용자
        /// <summary>
        public int? read_user { get; set; }

        /// <summary>
        /// 읽음시간
        /// <summary>
        public DateTime? read_dt { get; set; }

    }
}


