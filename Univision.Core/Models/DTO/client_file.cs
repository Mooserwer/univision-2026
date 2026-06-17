using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //클라이언트 관련 파일 테이블
    [Table("client_file")]
    public partial class client_file
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int cf_seq { get; set; }

       /// <summary>
       /// client pk
       /// <summary>
       public int c_seq { get; set; }

       /// <summary>
       /// 구분타입
       /// <summary>
       public string cf_type { get; set; }

       /// <summary>
       /// 디렉토리
       /// <summary>
       public string directory { get; set; }

       /// <summary>
       /// 원본 파일명
       /// <summary>
       public string origin_file_path { get; set; }

       /// <summary>
       /// 파일명
       /// <summary>
       public string file_path { get; set; }

       /// <summary>
       /// 확장자
       /// <summary>
       public string extension { get; set; }

       /// <summary>
       /// 등록일
       /// <summary>
       public DateTime? create_dt { get; set; }

       /// <summary>
       /// 등록자
       /// <summary>
       public int? create_user { get; set; }

        [NotMapped]
        public string uv_name { get; set; }

        [NotMapped]
        public string cf_type_desc { get; set; }
    }
}


