using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Remember
{
  public partial class r_code_school
  {
    /// <summary>
    /// 학교코드
    /// <summary>
    public int sc_seq { get; set; }

    /// <summary>
    /// 학교명
    /// <summary>
    public string school_name { get; set; }
    
  }

}


