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
  [Table("receipt_user")]
  public partial class receipt_user
  {
    /// <summary>
    /// candidate pk
    /// <summary>
    [Key]
    /// <summary>
    /// 
    /// <summary>
    public int ra_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int r_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int uv_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? sub_money1 { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? sub_money2 { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? money_total { get; set; }

    public int? is_submit { get; set; }
    /// <summary>
    /// 
    /// <summary>
    public DateTime? submit_date { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? is_lock { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string comment { get; set; }

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

  public partial class receipt_user
  {
    [NotMapped]
    public string r_type_name { get; set; }

    [NotMapped]
    public string r_month { get; set; }

    [NotMapped]
    public string is_open_name { get; set; }
    [NotMapped]
    public string submit_name { get; set; }

    [NotMapped]
    public string file_path { get; set; }
    [NotMapped]
    public string file_name { get; set; }

    [NotMapped]
    public int? is_open { get; set; }

    [NotMapped]
    public string uv_name { get; set; }

    [NotMapped]
    public string ud_name { get; set; }

    [NotMapped]
    public DateTime? r_start { get; set; }

    [NotMapped]
    public DateTime? r_end { get; set; }
  }
}


