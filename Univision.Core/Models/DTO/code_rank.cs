using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //직급 테이블
    [Table("code_rank")]
    public partial class code_rank
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int r_code { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string r_name { get; set; }

    }
}
