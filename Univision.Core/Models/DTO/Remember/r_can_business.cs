using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public class r_can_business
  {
    public int c_seq { get; set; }

    /// <summary>
    /// code_business(산업테이블) code1 
    /// <summary>
    public double code1 { get; set; }

    /// <summary>
    /// code_business(산업테이블) code2 
    /// <summary>
    public double code2 { get; set; }

    /// <summary>
    /// code_business(산업테이블) code3 
    /// <summary>
    public int order_no { get; set; } = 0;
  }
}
