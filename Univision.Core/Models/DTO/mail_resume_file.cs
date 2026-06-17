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
  [Table("mail_resume_file")]
  //
  //
  public partial class mail_resume_file
  {
    /// <summary>
    /// 
    /// <summary>
    [Key]
    [MaxLength(255)]
    [Column(Order = 1)]
    public string dv_timestamp { get; set; }

    /// <summary>
    /// 
    /// <summary>
    [Key]
    [Column(Order = 2)]
    public decimal dn_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_file_name { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_file_path { get; set; }

    /// <summary>
    /// 
    /// <summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public decimal dn_idx { get; set; }

    /// <summary>
    /// 파일구분
    /// <summary>
    public string resume_type { get; set; }

  }

  public partial class mail_resume_file
  {
    [NotMapped]
    public string file_path { get; set; }

    [NotMapped]
    public string file_path2 { get; set; }
  }

}


