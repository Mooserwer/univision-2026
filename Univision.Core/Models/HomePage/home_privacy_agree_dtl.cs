using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.HomePage
{
    //후보자 이력서 테이블
    [Table("privacy_agree_dtl")]
    public partial class home_privacy_agree_dtl
  {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int pad_seq { get; set; }

        /// <summary>
        /// 마스터키
        /// <summary>
        public int pa_seq { get; set; }

        /// <summary>
        /// 동의서 구분
        /// <summary>
        public int? agree_type { get; set; }

        /// <summary>
        /// 고객사 코드
        /// <summary>
        public int? client_seq { get; set; }

        /// <summary>
        /// 고객사명
        /// <summary>
        public string client_name { get; set; }

        /// <summary>
        /// 동의여부
        /// <summary>
        public int? is_agree { get; set; }

        /// <summary>
        /// 필수여부
        /// <summary>
        public int? is_optional { get; set; }

        /// <summary>
        /// 내용
        /// <summary>
        public string contents { get; set; }

        /// <summary>
        /// 생성일
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 생성자
        /// <summary>
        public int? create_user { get; set; }


    }

}


