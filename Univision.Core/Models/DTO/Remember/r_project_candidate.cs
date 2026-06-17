using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  //프로젝트 추천후보 테이블
  
  //프로젝트 추천후보 진행사항 테이블
  public partial class r_project_candidate
  {
    /// <summary>
    /// project pk
    /// <summary>
    public int p_seq { get; set; }

    /// <summary>
    /// candidate pk
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 진행상태 (관심후보:0, 추천: 10, 서류통과: 20, 면접: 30, 면접통과 : 40, 채용준비 : 50, 채용확정: 60, 탈락:99)
    /// <summary>
    public int? state { get; set; }

    /// <summary>
    /// 진행상태 (관심후보:0, 추천: 10, 서류통과: 20, 면접: 30, 면접통과 : 40, 채용준비 : 50, 채용확정: 60, 탈락:99)
    /// <summary>
    public string state_str { get; set; }
    /// <summary>
    /// 일정
    /// <summary>
    public DateTime? schedule_date { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_user { get; set; }
  }

}


