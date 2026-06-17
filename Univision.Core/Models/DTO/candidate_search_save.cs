using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //주소 테이블3
  [Table("candidate_search_save")]
  //
  public partial class candidate_search_save
  {
    /// <summary>
    /// IDX
    /// <summary>
    [Key]
    public int css_seq { get; set; }


    /// <summary>
    /// 제목
    /// <summary>
    public string title { get; set; }
    /// <summary>
    /// 검색링크
    /// <summary>
    public string url { get; set; }
    /// <summary>
    /// 설명(자동)
    /// <summary>
    public string desc { get; set; }

    /// <summary>
    /// 생성일자
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 생성자
    /// <summary>
    public int? create_user { get; set; }

    /// <summary>
    /// 수정일자
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_user { get; set; }

  }

}


