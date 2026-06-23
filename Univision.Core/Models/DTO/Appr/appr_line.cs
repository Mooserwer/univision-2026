using System;

namespace Univision.Core.Models.DTO
{
  // 전자결재 결재선 (APPR_LINE)
  public class appr_line
  {
    public int al_seq { get; set; }
    public int ad_seq { get; set; }
    public int order_no { get; set; }
    public int approver_seq { get; set; }
    public string approver_name { get; set; }
    public string approver_position { get; set; }
    // 0 대기 / 1 승인 / 2 반려
    public int line_status { get; set; }
    public DateTime? process_date { get; set; }
    public string opinion { get; set; }
  }
}
