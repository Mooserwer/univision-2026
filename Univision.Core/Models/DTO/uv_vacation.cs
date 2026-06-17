using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO
{
  //연차 정보 테이블
  [Table("uv_vacation")]
  public partial class uv_vacation
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int va_seq { get; set; }

    /// <summary>
    /// uv_user pk
    /// <summary>
    public int uv_seq { get; set; }

    /// <summary>
    /// 기준년도
    /// <summary>
    public float? va_year { get; set; }

    /// <summary>
    /// 연차 수
    /// <summary>
    public float? va_num { get; set; }

    /// <summary>
    /// 작년도 초과 연차
    /// <summary>
    public float? va_last { get; set; }

    /// <summary>
    /// 입사일
    /// <summary>
    public string comment { get; set; }


  }

  public partial class uv_vacation
  {
    [NotMapped]
    public float cur_va_num { get; set; } = 0;
    [NotMapped]
    public float cur_use_all { get; set; } = 0;

    [NotMapped]
    public float use_total { get; set; } = 0;
    [NotMapped]
    public float use_total_ready { get; set; } = 0;
    [NotMapped]
    public float use_vacation { get; set; } = 0;
    [NotMapped]
    public float use_vacation_ready { get; set; } = 0;

    [NotMapped]
    public float use_not_shown { get; set; } = 0;
    [NotMapped]
    public float use_etc { get; set; } = 0;
    [NotMapped]
    public float use_work { get; set; } = 0;
  }
}
