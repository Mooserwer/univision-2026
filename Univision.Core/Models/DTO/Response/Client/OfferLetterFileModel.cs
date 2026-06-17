using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
  public class OfferLetterFileModel
  {
    public int c_seq { get; set; }
    public int p_seq { get; set; }
    public string title { get; set; }
    public string kor_name { get; set; }
    public string file_directory { get; set; }
    public string file_origin_path { get; set; }
    public string file_path { get; set; }
    public DateTime schedule_date { get; set; }    
  }
}
