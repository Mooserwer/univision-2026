using System;

namespace Univision.Core.Models.DTO
{
  // 전자결재 첨부파일 (APPR_FILE)
  public class appr_file
  {
    public int af_seq { get; set; }
    public int ad_seq { get; set; }
    public string file_dir { get; set; }
    public string file_origin_path { get; set; }
    public string file_path { get; set; }
    public string file_extension { get; set; }
    public long? file_size { get; set; }
    public DateTime reg_date { get; set; }
  }
}
