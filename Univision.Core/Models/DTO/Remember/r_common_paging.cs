using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public class r_common_paging<T>
  {
    public int total_cnt { get; set; } = 0;

    /// <summary>
    /// candidate pk
    /// <summary>
    public List<T> item { get; set; } = new List<T>();
  }
}
