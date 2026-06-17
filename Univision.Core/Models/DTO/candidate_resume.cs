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
    [Table("candidate_resume")]
    public partial class candidate_resume
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
       /// 한글 이력서 폴더 경로
       /// <summary>
       public string kor_dir { get; set; }

       /// <summary>
       /// 한글 이력서 원본 이름
       /// <summary>
       public string kor_origin_path { get; set; }

       /// <summary>
       /// 한글 이력서 이름
       /// <summary>
       public string kor_path { get; set; }

       /// <summary>
       /// 한글 이력서 확장자
       /// <summary>
       public string kor_extension { get; set; }

       /// <summary>
       /// 영문 이력서 폴더 경로
       /// <summary>
       public string eng_dir { get; set; }

       /// <summary>
       /// 영문 이력서 원본 이름
       /// <summary>
       public string eng_origin_path { get; set; }

       /// <summary>
       /// 영문 이력서 이름
       /// <summary>
       public string eng_path { get; set; }

       /// <summary>
       /// 영문 이력서 확장자
       /// <summary>
       public string eng_extension { get; set; }

       /// <summary>
       /// 기타 파일 폴더 경로
       /// <summary>
       public string etc_file_dir { get; set; }

       /// <summary>
       /// 기타 파일 원본 이름
       /// <summary>
       public string etc_file_origin_apth { get; set; }

       /// <summary>
       /// 기타 파일 이름
       /// <summary>
       public string etc_file_path { get; set; }

       /// <summary>
       /// 기타 파일 확장자
       /// <summary>
       public string etc_file_extension { get; set; }

    }
}


