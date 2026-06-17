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
  [Table("test_invoice_new_dtl")]
  public partial class test_invoice_new_dtl
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int ind_seq { get; set; }

    /// <summary>
    /// 외래키 값
    /// <summary>
    public int in_seq { get; set; }

    // --- FK 관계 선언 추가 ---
    [ForeignKey("in_seq")]
    public virtual test_invoice_new invoice_new { get; set; }

    /// <summary>
    /// 담당자 코드
    /// <summary>
    public long r_user_id { get; set; }

    /// <summary>
    /// 담당자 이름
    /// <summary>
    public string r_user_name { get; set; }

    /// <summary>
    /// 담당자 이름
    /// <summary>
    public string r_user_email { get; set; }
    /// <summary>
    /// 담당자 유니비전 코드
    /// <summary>
    public int? uv_seq{ get; set; }
    /// <summary>
    /// 담당자 부서 코드
    /// <summary>
    public long? r_division_id { get; set; }
    /// <summary>
    /// 담당자 부서 이름
    /// <summary>
    public string r_division_name { get; set; }
    /// <summary>
    /// 비율
    /// <summary>
    public decimal sales_rate { get; set; }

    /// <summary>
    /// 금액
    /// <summary>
    public decimal sales_money { get; set; }
    /// <summary>
    /// 원화 환산 액
    /// <summary>
    public decimal? sales_won { get; set; }

    /// <summary>
    /// 실제 입금액
    /// <summary>
    public decimal deposit_amt { get; set; } = 0;

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


