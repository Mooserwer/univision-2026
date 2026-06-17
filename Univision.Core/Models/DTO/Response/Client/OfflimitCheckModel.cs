using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
  public class OfflimitCheckModel
  {
    public string kor_name { get; set; }
    public string eng_name { get; set; }
    public string kor_name_trim { get; set; }
    public string eng_name_trim { get; set; }
    public string name_all { get; set; }
    public string name_all_trim { get; set; }
    public int offlimit { get; set; }
  }
}
