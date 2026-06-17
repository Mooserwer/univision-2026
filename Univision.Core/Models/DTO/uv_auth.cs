using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //전체 권한 테이블
    [Table("uv_auth")]
    public partial class uv_auth
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int ua_seq { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public float? level { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public string name { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public float? is_use { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public DateTime? create_dt { get; set; }

    }
}


