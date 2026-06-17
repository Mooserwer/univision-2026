using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //후보자 산업 정보
  [Table("receipt_user_dtl")]
  public partial class receipt_user_dtl
  {
    /// <summary>
    /// candidate pk
    /// <summary>
    [Key]
    /// <summary>
    /// 
    /// <summary>
    public int rad_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int r_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int ra_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int rad_type { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string card_no { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? req_date { get; set; }

    public DateTime? conf_date { get; set; }

    /// <summary>
    /// 
    /// <summary>
    //public string card_name { get; set; }

    public string store_name { get; set; }

    public string store_type { get; set; }

    public string fee_type { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? money01 { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? money02 { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? confirm_money { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string comment1 { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string comment2 { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string file_path { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string file_name { get; set; }

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


