using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //후보자 외국어 능력 정보
    [Table("can_foreign_lan")]
    public partial class can_foreign_lan
    {
        /// <summary>
        /// candidate_seq FK
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public int c_seq { get; set; }

        /// <summary>
        /// code_foreign_lan code
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public string code { get; set; }

        /// <summary>
        /// 능력
        /// <summary>
        public int ability { get; set; }

    }
}
