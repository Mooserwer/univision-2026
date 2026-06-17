using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //부서 테이블
    [Table("uv_division")]
    public partial class uv_division
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int ud_seq { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public string ud_name { get; set; }

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


