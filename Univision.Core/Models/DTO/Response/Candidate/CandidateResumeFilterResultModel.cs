using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
  public class CandidateResumeFilterResultModel
  {
    public int is_work { get; set; } = 0;
    public int is_dup { get; set; } = 0;
    public int dup_c_seq { get; set; } = 0;

    public CandidateResumeFilterinfo info { get; set; } = new CandidateResumeFilterinfo();
  }

  public class CandidateResumeFilterinfo
  {
    public List<string> names { get; set; } = new List<string>();
    public List<string> birth { get; set; } = new List<string>();
    public List<string> email { get; set; } = new List<string>();
    public List<string> phone { get; set; } = new List<string>();
    public List<string> school { get; set; } = new List<string>();
    public List<string> company { get; set; } = new List<string>();
    public List<string> keywords { get; set; } = new List<string>();
  }
}
