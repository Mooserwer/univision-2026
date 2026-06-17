using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //후보자 이력서 테이블
    [Table("tempsaved_can_resume")]
    public partial class tempsaved_can_resume
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int cr_seq { get; set; }

       /// <summary>
       /// candidate pk
       /// <summary>
       public int c_seq { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public string file_type { get; set; }

       /// <summary>
       /// 기타 파일 폴더 경로
       /// <summary>
       public string file_dir { get; set; }

       /// <summary>
       /// 기타 파일 원본 이름
       /// <summary>
       public string file_origin_path { get; set; }

       /// <summary>
       /// 기타 파일 이름
       /// <summary>
       public string file_path { get; set; }

       /// <summary>
       /// 기타 파일 확장자
       /// <summary>
       public string file_extension { get; set; }

        /// <summary>
        /// 삭제일자
        /// <summary>
        public DateTime? remove_dt { get; set; }

        /// <summary>
        /// 삭제자
        /// <summary>
        public int? remove_user { get; set; }

        /// <summary>
        /// 등록일자
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 등록자
        /// <summary>
        public int? create_user { get; set; }

    }
}


