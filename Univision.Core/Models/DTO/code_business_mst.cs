using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //상위 업종 테이블 1
    [Table("code_business_mst")]
  
    //상위 업종 테이블 1
    public partial class code_business_mst
    {
        /// <summary>
        /// 코드
        /// <summary>
        [Key]
        public float code1 { get; set; }

        /// <summary>
        /// 상위 업종명
        /// <summary>
        public string code_name1 { get; set; }

        /// <summary>
        /// 사용여부
        /// <summary>
        public float? is_hide { get; set; }

        /// <summary>
        /// 순번
        /// <summary>
        public int? order_no { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? modify_dt { get; set; }

    }
}


