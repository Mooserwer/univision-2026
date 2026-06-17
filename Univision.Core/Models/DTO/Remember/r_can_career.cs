using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public class r_can_career
  {
    public int cc_seq { get; set; }

    /// <summary>
    /// 후보자 seq
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 회사명
    /// <summary>
    public string company_name { get; set; }

    /// <summary>
    /// 부서명
    /// <summary>
    public string division_name { get; set; }

    /// <summary>
    /// 입사일
    /// <summary>
    public string join_dt { get; set; }

    /// <summary>
    /// 퇴사일
    /// <summary>
    public string leave_dt { get; set; }

    /// <summary>
    /// 재직여부
    /// <summary>
    public int? is_work { get; set; }

    /// <summary>
    /// 연봉
    /// <summary>
    public int? annual_income { get; set; }

    /// <summary>
    /// 순서(낮은 수가 최신)
    /// <summary>
    public int order_no { get; set; }
  }
}
