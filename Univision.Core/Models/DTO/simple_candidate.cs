using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //간편후보 테이블
    [Table("simple_candidate")]
    public partial class simple_candidate
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int sc_seq { get; set; } = 0;

        /// <summary>
        /// api_company pk
        /// <summary>
        public int ac_seq { get; set; } = 0;

        /// <summary>
        /// client pk
        /// <summary>
        public int c_seq { get; set; } = 0;

        public int? p_seq { get; set; } = 0;

        /// <summary>
        /// 이름
        /// <summary>
        public string kor_name { get; set; } = "";

        /// <summary>
        /// 성별(남 :1 , 여자 : 2)
        /// <summary>
        public int? gender { get; set; } = 0;

        public string birth_str { get; set; } = "";
        /// <summary>
        /// 생년월일
        /// <summary>
        public DateTime? birthdate { get; set; }


        /// <summary>
        /// 전화번호
        /// <summary>
        public string cell_phone { get; set; } = "";

        /// <summary>
        /// 이메일
        /// <summary>
        public string email { get; set; } = "";

        /// <summary>
        /// sns_url
        /// <summary>
        public string sns_url { get; set; } = "";

        /// <summary>
        /// 기타 설명
        /// </summary>
        public string comments { get; set; } = "";
        /// <summary>
        /// 회사
        /// <summary>
        public string school { get; set; } = "";

        /// <summary>
        /// 회사
        /// <summary>
        public string company { get; set; } = "";

        /// <summary>
        /// 생성일
        /// <summary>
        public DateTime create_dt { get; set; }

        /// <summary>
        /// 생성자 uv_seq
        /// <summary>
        public int create_user { get; set; }

        /// <summary>
        /// 수정일
        /// <summary>
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 수정자 uv_seq
        /// <summary>
        public int? modify_user { get; set; }

        public string company_desc { get; set; }

        public int is_del { get; set; } = 0;
    }

    public partial class simple_candidate
    {
        [NotMapped]
        public int rowindex { get; set; }
        [NotMapped]
        public string birthDateStr { get; set; }
        [NotMapped]
        public string create_name { get; set; }
        [NotMapped]
        public string modify_name { get; set; }
        [NotMapped]
        public string create_dt_str { get; set; }
        [NotMapped]
        public string modify_dt_str { get; set; }
    }

}


