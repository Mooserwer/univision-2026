using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //상위 주소 테이블
    [Table("inorder_file")]
    //인오더 관련 파일
    public partial class inorder_file
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int if_seq { get; set; }

        /// <summary>
        /// inorder pk
        /// <summary>
        public int i_seq { get; set; }

        /// <summary>
        /// 파일 디렉토리
        /// <summary>
        public string file_dir { get; set; }

        /// <summary>
        /// 파일 원본 이름
        /// <summary>
        public string file_origin_path { get; set; }

        /// <summary>
        /// 파일이름
        /// <summary>
        public string file_path { get; set; }

        /// <summary>
        /// 파일 확장자
        /// <summary>
        public string file_extension { get; set; }

        /// <summary>
        /// 파일 구분 명
        /// <summary>
        public string file_type_name { get; set; }

        /// <summary>
        /// 등록일
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 등록자
        /// <summary>
        public int? create_user { get; set; }

        /// <summary>
        /// 삭제 일자
        /// </summary>
        public DateTime? remove_dt { get; set; }

        /// <summary>
        /// 삭제자
        /// </summary>
        public int? remove_user { get; set; }

    }
}


