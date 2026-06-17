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
    [Table("board_file")]
    //
    //
    public partial class board_file
    {
        /// <summary>
        /// IDX
        /// <summary>
        [Key]
        public int bf_seq { get; set; }

        /// <summary>
        /// 게시판 IDX
        /// <summary>
        public int b_seq { get; set; }

        /// <summary>
        /// 파일경로
        /// <summary>
        public string file_dir { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string file_origin_path { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string file_path { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string file_extension { get; set; }

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

    }
}


