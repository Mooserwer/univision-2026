using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Remember.RestfulServer.Models
{
  public class CodeCandidateRegType
  {
    public int reg_type { get; set; }
    public string reg_name { get; set; }

    public CodeCandidateRegType(int reg_type, string reg_name)
    {
      this.reg_type = reg_type;
      this.reg_name = reg_name;
    }
  }
}