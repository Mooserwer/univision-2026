using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //주소 테이블3
    [Table("addr3")]
    public partial class addr3
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public string code1 { get; set; }

        /// <summary>
        /// 
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public string code2 { get; set; }

        /// <summary>
        /// 
        /// <summary>
        [Key]
        [Column(Order = 3)]
        public string code3 { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public string area3 { get; set; }

    }
}


