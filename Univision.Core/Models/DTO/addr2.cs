using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //주소 테이블2
    [Table("addr2")]
    public partial class addr2
    {
        /// <summary>
        /// 코드 1
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public string code1 { get; set; }

        /// <summary>
        /// 코드 2
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public string code2 { get; set; }

       /// <summary>
       /// 구 정보
       /// <summary>
       public string area2 { get; set; }

    }
}


