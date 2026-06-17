using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Remember
{
  public partial class r_uv_user
  {
    /// <summary>
    /// 사용자코드
    /// <summary>
    public int uv_seq { get; set; }

    /// <summary>
    /// 이름
    /// <summary>
    public string name { get; set; }
    /// <summary>
    /// 직급
    /// <summary>
    public string rank_name { get; set; }
    /// <summary>
    /// 부서
    /// <summary>
    public string div_name { get; set; }
    public string email { get; set; }
  }

}


