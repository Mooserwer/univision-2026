using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    [Table("code_sectors1")]
    public partial class code_sectors1
    {
        /// <summary>
        /// 코드
        /// <summary>
        [Key]
        public string code1 { get; set; }

        /// <summary>
        /// 상위 업종명
        /// <summary>
        public string code_name1 { get; set; }

    }
}
