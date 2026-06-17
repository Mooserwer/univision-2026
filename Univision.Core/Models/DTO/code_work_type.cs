using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //근무 형태 테이블
    [Table("code_work_type")]
    public partial class code_work_type
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public float code { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public string type_name { get; set; }

    }
}


