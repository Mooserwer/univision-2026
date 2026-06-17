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
  [Table("director_business")]
  public partial class director_business
  {
    /// <summary>
    /// candidate pk
    /// <summary>
    [Key]
    [Column(Order = 1)]
    public int d_seq { get; set; }

    /// <summary>
    /// code_business(산업테이블) code1 
    /// <summary>
    [Key]
    [Column(Order = 2)]
    public double code1 { get; set; }

    /// <summary>
    /// code_business(산업테이블) code2 
    /// <summary>
    [Key]
    [Column(Order = 3)]
    public double code2 { get; set; }

    /// <summary>
    /// code_business(산업테이블) code3 
    /// <summary>
    public int order_no { get; set; } = 0;

  }

  public partial class director_business
  {
    [NotMapped]
    public string code_name1 { get; set; }
    [NotMapped]
    public string code_name2 { get; set; }
  }
}


