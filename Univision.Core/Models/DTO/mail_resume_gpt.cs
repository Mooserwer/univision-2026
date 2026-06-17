using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //상위 주소 테이블
  [Table("mail_resume_gpt")]
  //
  public partial class mail_resume_gpt
  {
    /// <summary>
    /// 
    /// <summary>
    [Key]
    [MaxLength(255)]
    public string dv_timestamp { get; set; }

    public int? c_seq { get; set; }
    /// <summary>
    /// 
    /// <summary>
    public string gpt_result { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string gpt_result_busi { get; set; }
    /// <summary>
    /// 
    /// <summary>
    public string gpt_result_job { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? reg_date { get; set; }

  }


}


