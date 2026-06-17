using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //클라이언트 담당자
    [Table("client_contact")]
    public partial class client_contact
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int cc_seq { get; set; }

       /// <summary>
       /// client pk
       /// <summary>
       public int c_seq { get; set; }

       /// <summary>
       /// 담당자 명
       /// <summary>
       public string name { get; set; }

        public int gender { get; set; }

       /// <summary>
       /// 이메일
       /// <summary>
       public string email { get; set; }

       /// <summary>
       /// 전화번호
       /// <summary>
       public string phone { get; set; }

       /// <summary>
       /// 휴대폰 번호
       /// <summary>
       public string cell_phone { get; set; }

       /// <summary>
       /// 부서
       /// <summary>
       public string division { get; set; }

       /// <summary>
       /// 직급
       /// <summary>
       public string position { get; set; }

        /// <summary>
        /// 메모
        /// <summary>
        public string memo { get; set; }

        /// <summary>
        /// 등록일
        /// <summary>
        public DateTime? create_dt { get; set; }

       /// <summary>
       /// 등록자
       /// <summary>
       public int? create_user { get; set; }

       /// <summary>
       /// 수정일
       /// <summary>
       public DateTime? modify_dt { get; set; }

       /// <summary>
       /// 수정자
       /// <summary>
       public int? modify_user { get; set; }

    }

    public partial class client_contact
    {
        /// <summary>
        /// 업체명
        /// <summary>
        [NotMapped]
        public string kor_name { get; set; }

        [NotMapped]
        public string eng_name { get; set; }

        [NotMapped]
        public string am_name { get; set; }

        /// <summary>
        /// 담당자
        /// <summary>
        [NotMapped]
        public string uv_name { get; set; }

        [NotMapped]
        public string sex { get; set; }

        [NotMapped]
        public int p_seq { get; set; }

        [NotMapped]
        public int is_external_lock { get; set; } = 0;

        [NotMapped]
        public int log_cnt { get; set; } = 0;
    }
}


