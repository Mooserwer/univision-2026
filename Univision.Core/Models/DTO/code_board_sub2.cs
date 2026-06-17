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
    [Table("code_board_sub2")]
    //

    public partial class code_board_sub2
    {
        /// <summary>
        /// 타입
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public int type { get; set; }
        /// <summary>
        /// 대분류코드
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public int code1 { get; set; }

        /// <summary>
        /// 소분류코드
        /// <summary>
        [Key]
        [Column(Order = 3)]
        public int code2 { get; set; }

        /// <summary>
        /// 코드명
        /// <summary>
        public string code_name { get; set; }

        public int order_no { get; set; }

        public int is_hide { get; set; }

    }

    public partial class code_board_sub2
    {

        [NotMapped]
        public int sub2_doc_count { get; set; }
    }


}


