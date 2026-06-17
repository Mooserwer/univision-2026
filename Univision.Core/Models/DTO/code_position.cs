using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //직책 테이블
    [Table("code_position")]
    public partial class code_position
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int p_code { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string p_name { get; set; }

    }
}
