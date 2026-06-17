using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //후보자 진행상황 
  [Table("director_activity")]
  public partial class director_activity
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int da_seq { get; set; }

    /// <summary>
    /// candidate pk
    /// <summary>
    public int d_seq { get; set; }

    /// <summary>
    /// candidate pk
    /// <summary>
    public int? cl_seq { get; set; }


    /// <summary>
    /// 진행상황 선택
    /// <summary>
    public string ca_status { get; set; }

    /// <summary>
    /// 일자
    /// <summary>
    public DateTime? ca_date { get; set; }

    public int memo_type { get; set; } = 0;
    /// <summary>
    /// 메모
    /// <summary>
    
    public string memo { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_seq { get; set; }

    /// <summary>
    /// 수정일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_seq { get; set; }

  }

  public partial class director_activity
  {
    /// <summary>
    /// 고객사 한글명
    /// </summary>
    [NotMapped]
    public string kor_name { get; set; }

    [NotMapped]
    public string type_str { get; set; }
    /// <summary>
    /// 작성자 이름
    /// </summary>
    [NotMapped]
    public string create_name { get; set; }
  }
}


