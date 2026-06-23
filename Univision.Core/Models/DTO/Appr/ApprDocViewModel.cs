using System.Collections.Generic;

namespace Univision.Core.Models.DTO
{
  // 기안 상세/작성 화면용 합성 모델
  public class ApprDocViewModel
  {
    public appr_doc doc { get; set; } = new appr_doc();
    public List<appr_line> lines { get; set; } = new List<appr_line>();
    public List<appr_file> files { get; set; } = new List<appr_file>();
    public List<appr_comment> comments { get; set; } = new List<appr_comment>();
  }
}
