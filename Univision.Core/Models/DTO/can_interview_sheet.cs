using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //후보자 인터뷰 시트
    [Table("can_interview_sheet")]
    public partial class can_interview_sheet
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int cis_seq { get; set; }

       /// <summary>
       /// candidate pk
       /// <summary>
       public int c_seq { get; set; }

       /// <summary>
       /// 메모
       /// <summary>
       public string memo { get; set; }

       /// <summary>
       /// 인터뷰 일자
       /// <summary>
       public DateTime interview_dt { get; set; }

       /// <summary>
       /// 등록일
       /// <summary>
       public DateTime? create_dt { get; set; }

       /// <summary>
       /// 등록자
       /// <summary>
       public int? create_seq { get; set; }

    }

    public partial class can_interview_sheet
    {
        /// <summary>
        /// 등록자명
        /// </summary>
        [NotMapped]
        public string uv_name { get; set; }
    }
}


