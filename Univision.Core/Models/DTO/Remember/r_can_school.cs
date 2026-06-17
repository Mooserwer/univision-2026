using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public class r_can_school
  {
    public int cs_seq { get; set; }

    /// <summary>
    /// candidate pk
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 학교 정보 테이블 pk
    /// <summary>
    public int sc_seq { get; set; }

    /// <summary>
    /// 학교구분(high_list : 고등학교, univ_list : 대학교)
    /// <summary>
    public string gubun { get; set; }

    /// <summary>
    /// 졸업년도
    /// <summary>
    public string graduate_year { get; set; }

    /// <summary>
    /// 입학년도
    /// <summary>
    public string admission_year { get; set; }

    /// <summary>
    /// 졸업형태
    /// <summary>
    public int? graduate_status { get; set; }
    /// <summary>
    /// 편입여부
    /// <summary>
    public int? is_transfer { get; set; }
 
    /// <summary>
    /// 전공 계열
    /// <summary>
    public string category1_name { get; set; }

    /// <summary>
    /// 전공 명
    /// <summary>
    public string major_name { get; set; }

    /// <summary>
    /// 해외학교 여부
    /// <summary>
    public int? is_foreign_school { get; set; }

    /// <summary>
    /// 전공 계열
    /// <summary>
    public string sub_category1_name { get; set; }

    /// <summary>
    /// 전공 명
    /// <summary>
    public string sub_major_name { get; set; }

    /// <summary>
    /// 순서(낮은 수가 최신)
    /// <summary>
    public int order_no { get; set; }
  }
}
