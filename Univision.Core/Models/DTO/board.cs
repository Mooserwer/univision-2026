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
    [Table("board")]
    //
    public partial class board
    {
        /// <summary>
        /// IDX
        /// <summary>
        [Key]
        public int b_seq { get; set; }

        /// <summary>
        /// 게시판 구분(0:공지게 / 1: 교육게 / 2 : 지식게 / 3:자유게 / 5:파일게)
        /// <summary>
        public int b_type { get; set; }

        /// <summary>
        /// 파일게시판 대구분
        /// <summary>
        public int b_type_sub1 { get; set; }

        /// <summary>
        /// 파일게시판 소구분
        /// <summary>
        public int b_type_sub2 { get; set; }

        /// <summary>
        /// 공지여부(1:공지 / 0 : 공지아님)
        /// <summary>
        public int? b_top { get; set; }
        
        /// <summary>
        /// 제목
        /// <summary>
        public string title { get; set; }

        /// <summary>
        /// 내용
        /// <summary>
        public string contents { get; set; }

        /// <summary>
        /// 생성일자
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 생성자
        /// <summary>
        public int? create_user { get; set; }

        /// <summary>
        /// 수정일자
        /// <summary>
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 수정자
        /// <summary>
        public int? modify_user { get; set; }

    }

    /// <summary>
    /// 리스트에서 쓰임.
    /// </summary>
    public partial class board
    {
        [NotMapped]
        public int? bf_seq { get; set; } = 0;

        [NotMapped]
        public string b_type_sub_name { get; set; }

        /// <summary>
        /// 작성자명
        /// </summary>
        [NotMapped]
        public string user_name { get; set; }

        [NotMapped]
        public int read_cnt { get; set; }


        [NotMapped]
        public DateTime? last_read_date { get; set; }

        [NotMapped]
        public int reply_cnt { get; set; }


        [NotMapped]
        public List<board_file> file_list { get; set; } = new List<board_file>();
    }
}


