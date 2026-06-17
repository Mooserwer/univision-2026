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
    [Table("board_reply")]
    //
    //
    public partial class board_reply
    {
        /// <summary>
        /// IDX
        /// <summary>
        [Key]
        public int br_seq { get; set; }

        /// <summary>
        /// 게시판 IDX
        /// <summary>
        public int b_seq { get; set; }

        /// <summary>
        /// 댓글내용
        /// <summary>
        public string br_contents { get; set; }

        /// <summary>
        /// 생성일
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 상성자
        /// <summary>
        public int? create_user { get; set; }

    }
}


