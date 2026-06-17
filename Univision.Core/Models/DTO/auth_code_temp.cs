using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO
{
    [Table("auth_code_temp")]
    public partial class auth_code_temp
    {
        /// <summary>
        /// 인증코드 seq
        /// <summary>
        [Key]
        public int a_seq { get; set; }

        /// <summary>
        /// 로그인 유저 아이디.
        /// <summary>
        public string user_id { get; set; }

        /// <summary>
        /// 인증코드
        /// <summary>
        public string auth_code { get; set; }

        /// <summary>
        /// 생성일
        /// <summary>
        public DateTime reg_dt { get; set; }

    }
}
