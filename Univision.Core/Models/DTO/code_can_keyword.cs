using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  [Table("code_can_keyword")]
  //주간 이메일 리포팅 마스터
  public partial class code_can_keyword
  {
    /// <summary>
    /// 
    /// <summary>
    [Key]
    public int ck_seq { get; set; }

    /// <summary>
    /// 키워드 구분(1:산업, 2:직무, 3:스킬, 4:특징..)
    /// <summary>
    public float key_type { get; set; }

    /// <summary>
    /// 키워드
    /// <summary>
    public string keyword_str { get; set; }

    /// <summary>
    /// 대분류 코드(산업/직무 연관 시)
    /// <summary>
    public float? code_mst { get; set; }

    /// <summary>
    /// 소분류 코드(산업/직무 연관 시)
    /// <summary>
    public float? code_dtl { get; set; }

    /// <summary>
    /// 회사코드(회사 연관 시)
    /// <summary>
    public int? company_cd { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? modify_dt { get; set; }

  }

  public partial class code_can_keyword
  {
    [NotMapped]
    public string key_type_str { get; set; }

    [NotMapped]
    public string name { get; set; }
  }
}


