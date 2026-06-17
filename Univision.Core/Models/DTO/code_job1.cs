using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //직종 테이블 1
    [Table("code_job1")]
    public partial class code_job1
    {
        /// <summary>
        /// 직종코드1
        /// <summary>
        [Key]
        public float code1 { get; set; }

       /// <summary>
       /// 직종명1
       /// <summary>
       public string code_name1 { get; set; }

    }
}


