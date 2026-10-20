using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.HomePage
{
  // 개인정보 동의 마스터 (홈페이지 DB - 개인정보 최소화). 테이블명은 내부와 동일 n_privacy_agree.
  [Table("n_privacy_agree")]
  public partial class home_n_privacy_agree
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]  // pa_seq 는 내부에서 생성된 값을 복사 (identity 아님)
    public int pa_seq { get; set; }
    public string send_type { get; set; }     // S/E
    public DateTime? send_dt { get; set; }
    public string agree_code3 { get; set; }   // 동의 링크 키 (공개 페이지 조회용)
    public string agree_code1 { get; set; }   // 이름(평문) - 동의 시 입력
    public string agree_code2 { get; set; }   // 생년월일(해시) - 동의 시 입력
    public DateTime? agree_dt { get; set; }    // 동의일시
    public string agree_ip { get; set; }      // 동의 IP
    public DateTime? create_dt { get; set; }
    public int is_sync { get; set; }          // 0:미동기화 / 1:동기화완료
    public DateTime? sync_dt { get; set; }
  }

  // 개인정보 동의 상세 (약관 + 항목별 동의결과. 대상자 PII 없음)
  [Table("n_privacy_agree_dtl")]
  public partial class home_n_privacy_agree_dtl
  {
    [Key]
    public int pad_seq { get; set; }
    public int pa_seq { get; set; }
    public int? agree_type { get; set; }
    public int? client_seq { get; set; }
    public string client_name { get; set; }
    public int? is_agree { get; set; }
    public int? is_optional { get; set; }
    public string contents { get; set; }
    public DateTime? create_dt { get; set; }
  }
}
