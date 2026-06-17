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
  [Table("receipt")]
  public partial class receipt
  {
    /// <summary>
    /// candidate pk
    /// <summary>
    [Key]
    /// <summary>
    /// 
    /// <summary>
    public int r_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int r_type { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string r_month { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? r_start { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? r_end { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? is_open { get; set; }

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

  public partial class receipt
  {
    [NotMapped]
    public string r_type_name { get; set; }
    [NotMapped]
    public string is_open_name{ get; set; }

    [NotMapped]
    public string submit_name { get; set; }
    [NotMapped]
    public int? is_submit { get; set; }

    [NotMapped]
    public DateTime? submit_date { get; set; }

    [NotMapped]
    public int? rad_seq { get; set; }

    [NotMapped]
    public string file_path { get; set; }

    [NotMapped]
    public string file_name { get; set; }

    [NotMapped]
    public string user_comment { get; set; }

    [NotMapped]
    public int? money1 { get; set; }

    [NotMapped]
    public int? ra_seq { get; set; }

    [NotMapped]
    public int? money2 { get; set; }
  }
}


