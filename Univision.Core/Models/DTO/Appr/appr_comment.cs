using System;

namespace Univision.Core.Models.DTO
{
  // 전자결재 댓글 (APPR_COMMENT)
  public class appr_comment
  {
    public int ac_seq { get; set; }
    public int ad_seq { get; set; }
    public int writer_seq { get; set; }
    public string writer_name { get; set; }
    public string content { get; set; }
    public DateTime reg_date { get; set; }
    public bool is_deleted { get; set; }
  }
}
