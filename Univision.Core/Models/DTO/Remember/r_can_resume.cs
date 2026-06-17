using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public class r_can_resume
  {
    public int c_seq { get; set; }

    /// <summary>
    /// 국문이력서 최신 내용
    /// <summary>
    public string kor_resume_body { get; set; }

    /// <summary>
    /// 영문이력서 최신 내용
    /// <summary>
    public string eng_resume_body { get; set; }
  }

  public class r_can_resume_file_info
  {
    public int cr_seq { get; set; }
    public int c_seq { get; set; }
    public string file_type { get; set; }
    public string file_dir { get; set; }
    public string file_origin_path { get; set; }
    public string file_path { get; set; }
    public string file_extension { get; set; }
    public DateTime create_dt { get; set; }
    public int create_user { get; set; }
  }
}
