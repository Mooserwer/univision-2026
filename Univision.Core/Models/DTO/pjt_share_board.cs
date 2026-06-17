using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO
{
    public partial class pjt_share_board
    {
        /// <summary>
        /// project seq
        /// <summary>
        public int p_seq { get; set; }

        /// <summary>
        /// pjt_share_board seq
        /// <summary>
        [Key]
        public int psb_seq { get; set; }

        /// <summary>
        /// 내용
        /// <summary>
        public string contents { get; set; }

        /// <summary>
        /// 생성일자
        /// <summary>
        public DateTime create_dt { get; set; }

        /// <summary>
        /// 생성자
        /// <summary>
        public int create_user { get; set; }

        /// <summary>
        /// 수정일자
        /// <summary>
        public DateTime modify_dt { get; set; }

        /// <summary>
        /// 수정자
        /// <summary>
        public int modify_user { get; set; }

    }

    public partial class pjt_share_board
    {
        [NotMapped]
        public string create_user_name { get; set; }

        [NotMapped]
        public List<pjt_share_reply> replyList { get; set; } = new List<pjt_share_reply>();
    }
}
