using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //로그인 실패 정보 테이블
    [Table("uv_failed_login")]
    public partial class uv_failed_login
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int uf_seq { get; set; }

       /// <summary>
       /// uv_user pk
       /// <summary>
       public int uv_seq { get; set; }

       /// <summary>
       /// 로그인 아이디
       /// <summary>
       public int? login_id { get; set; }

       /// <summary>
       /// 로그인 아이피
       /// <summary>
       public int? login_ip { get; set; }

       /// <summary>
       /// 로그인 시도 시간
       /// <summary>
       public int? login_time { get; set; }

    }
}


