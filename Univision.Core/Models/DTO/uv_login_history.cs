using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //로그인 히스토리 테이블
    [Table("uv_login_history")]
    public partial class uv_login_history
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int ul_seq { get; set; }

       /// <summary>
       /// uv_user pk
       /// <summary>
       public int uv_seq { get; set; }

       /// <summary>
       /// 로그인 시간
       /// <summary>
       public DateTime login_dt { get; set; }

       /// <summary>
       /// 로그인 아이피
       /// <summary>
       public string login_ip { get; set; }

    }
}


