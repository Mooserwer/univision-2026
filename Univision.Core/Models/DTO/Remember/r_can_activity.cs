using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public class r_can_activity
  {
    public int ca_seq { get; set; }

    /// <summary>
    /// candidate pk
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 메모
    /// <summary>
    public string memo { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 수정일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_seq { get; set; }

    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_seq { get; set; }
  }
}
