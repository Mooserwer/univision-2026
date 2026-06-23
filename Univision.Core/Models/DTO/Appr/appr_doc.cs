using System;

namespace Univision.Core.Models.DTO
{
  // 전자결재 기안 문서 (APPR_DOC)
  public class appr_doc
  {
    public int ad_seq { get; set; }
    public string doc_no { get; set; }   // 자동 채번 (기안-yyMM-0001), 상신 시 부여
    public string title { get; set; }
    public string content { get; set; }
    public int drafter_seq { get; set; }
    public string drafter_name { get; set; }
    public int? drafter_ud_seq { get; set; }
    // 0 임시저장 / 1 진행중 / 2 승인완료 / 3 반려 / 4 회수
    public int doc_status { get; set; }
    public int cur_order { get; set; }
    public int? copy_from { get; set; }
    public DateTime reg_date { get; set; }
    public DateTime? mod_date { get; set; }
    public DateTime? submit_date { get; set; }
    public DateTime? complete_date { get; set; }
    public bool is_deleted { get; set; }

    // 조회 표시용(매핑/조인)
    public string drafter_ud_name { get; set; }
    public string cur_approver_name { get; set; }   // 현재 결재 차례 결재자명
    public int file_count { get; set; }
  }
}
