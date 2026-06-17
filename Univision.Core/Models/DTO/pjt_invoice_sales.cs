using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //프로젝트 인보이스 담당자 금액선정
  [Table("pjt_invoice_sales")]
  public partial class pjt_invoice_sales
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int pis_seq { get; set; }

    /// <summary>
    /// project_invoice_info pk
    /// <summary>
    public int pii_seq { get; set; }

    /// <summary>
    /// project_recandidate pk
    /// <summary>
    public int? prc_seq { get; set; }

    /// <summary>
    /// candidate pk
    /// <summary>
    public int? c_seq { get; set; }

    /// <summary>
    /// project pk
    /// <summary>
    public int p_seq { get; set; }

    /// <summary>
    /// 담당자 uv_seq
    /// <summary>
    public int? uv_seq { get; set; }

    /// <summary>
    /// 담당자 uv_seq
    /// <summary>
    public int? ud_seq { get; set; }
    /// <summary>
    /// 비율
    /// <summary>
    public float? sales_rate { get; set; }

    /// <summary>
    /// 금액
    /// <summary>
    public double? sales_money { get; set; }
    /// <summary>
    /// 원화 환산 액
    /// <summary>
    public double? sales_won { get; set; }
    /// <summary>
    /// 실제 입금액
    /// <summary>
    public double? deposit_amt { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string comments { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? create_user { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? modify_user { get; set; }
  }
}


