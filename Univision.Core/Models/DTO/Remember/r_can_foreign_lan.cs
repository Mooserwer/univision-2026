using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public class r_can_foreign_lan
  {
    /// <summary>
    /// candidate_seq FK
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// code_foreign_lan code
    /// <summary>
    public string code { get; set; }

    /// <summary>
    /// 능력
    /// <summary>
    public int ability { get; set; }
  }
}
