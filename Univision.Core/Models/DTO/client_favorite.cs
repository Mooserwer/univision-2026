using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //컨설턴트 관심 클라이언트 정보
    [Table("client_favorite")]
    public partial class client_favorite
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
       /// uv_user pk
       /// <summary>
       public int uv_seq { get; set; }

    }

    public partial class client_favorite
    {
        /// <summary>
        /// 한글 회사명
        /// <summary>
        [NotMapped]
        public string kor_name { get; set; }

        [NotMapped]
        public string eng_name { get; set; }
        /// <summary>
        /// 사업자번호
        /// <summary>

        [NotMapped]
        public string ceo { get; set; }

        [NotMapped]
        public string biz_code { get; set; }

        /// <summary>
        /// 담당자명
        /// <summary>
        [NotMapped]
        public string name { get; set; }

        /// <summary>
        /// 계약 여부
        /// <summary>
        [NotMapped]
        public int? is_contract { get; set; }

        [NotMapped]
        public int? is_foreign { get; set; }

        [NotMapped]
        public int? offlimit { get; set; }

        /// <summary>
        /// 산업구분
        /// <summary>
        [NotMapped]
        public string biz_industry { get; set; }

        /// <summary>
        /// 진행중인 프로젝트 수량
        /// <summary>
        [NotMapped]
        public int ing_prj { get; set; }

        /// <summary>
        /// 성공한 프로젝트 수량
        /// <summary>
        [NotMapped]
        public int success_prj { get; set; }

        /// <summary>
        /// 전체 프로젝트 수량
        /// <summary>
        [NotMapped]
        public int total_prj { get; set; }

        /// <summary>
        /// 실패(취소) 프로젝트 수량
        /// <summary>
        [NotMapped]
        public int cancel_prj { get; set; }

        /// <summary>
        /// 등록일
        /// <summary>
        [NotMapped]
        public DateTime create_dt { get; set; }

        [NotMapped]
        public DateTime? modify_dt { get; set; }

        [NotMapped]
        public string am_name { get; set; }
    }
}


