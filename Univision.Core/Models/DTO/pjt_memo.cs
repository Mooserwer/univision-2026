using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //프로젝트 메모 히스토리 
    [Table("pjt_memo")]
    public partial class pjt_memo
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int pm_seq { get; set; }

       /// <summary>
       /// project pk
       /// <summary>
       public int p_seq { get; set; }

       /// <summary>
       /// 메모
       /// <summary>
       public string memo { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public DateTime? create_dt { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public int? create_user { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public DateTime? modify_dt { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public int? modify_user { get; set; }

    }

    public partial class pjt_memo
    {
        [NotMapped]
        public string create_user_name { get; set; }
    }
}


