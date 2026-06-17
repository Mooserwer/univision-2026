using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Remember
{
  public partial class r_code_business
  {
    /// <summary>
    /// 업종 코드1
    /// <summary>
    public float code1 { get; set; }

    /// <summary>
    /// 업종 코드2
    /// <summary>
    public float code2 { get; set; }

    /// <summary>
    /// 업종 명2
    /// <summary>
    public string code_name1 { get; set; }
    /// <summary>
    /// 업종 명2
    /// <summary>
    public string code_name2 { get; set; }

    /// <summary>
    /// 사용여부
    /// <summary>
    public int? is_hide { get; set; }

    /// <summary>
    /// 순번
    /// <summary>
    public int? order_no { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? modify_dt { get; set; }

  }

}


