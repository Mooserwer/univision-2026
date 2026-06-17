using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //프로젝트 담당자 테이블
  [Table("pjt_manager")]
  public partial class pjt_manager
  {
    /// <summary>
    /// project pk
    /// <summary>
    [Key]
    [Column(Order = 1)]
    public int p_seq { get; set; }

    /// <summary>
    /// uv_user pk
    /// <summary>
    [Key]
    [Column(Order = 2)]
    public int uv_seq { get; set; }

    public DateTime? entry_date { get; set; }

  }

  public partial class pjt_manager
  {
    /// <summary>
    /// 담당자 이름
    /// </summary>
    [NotMapped]
    public string name { get; set; }

    /// <summary>
    /// 담당자 부서명
    /// </summary>
    [NotMapped]
    public string ud_name { get; set; }
  }
}


