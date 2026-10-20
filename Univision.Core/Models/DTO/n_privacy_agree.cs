using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  // 개인정보 동의 마스터 (신규 PrivacyCreate 플로우 - 내부 DB)
  [Table("n_privacy_agree")]
  public partial class n_privacy_agree
  {
    [Key]
    public int pa_seq { get; set; }
    public string send_type { get; set; }     // 발송구분 S:SMS / E:mail
    public string send_addr { get; set; }     // 발신주소(발신번호 또는 발신메일)
    public string can_name { get; set; }      // 대상자명
    public string can_contact { get; set; }   // 연락처(전화 또는 이메일)
    public int? profile_id { get; set; }      // 대상 프로필 키 (Remember profile id)
    public string sender_email { get; set; }  // 발신자 통지 이메일 (동의완료 시 수신)
    public DateTime? send_dt { get; set; }     // 발송일자
    public string full_txt { get; set; }      // 동의서 전문
    public DateTime? agree_dt { get; set; }    // 동의일자
    public string agree_ip { get; set; }      // 동의 IP
    public string agree_code1 { get; set; }   // 본인확인: 이름(평문)
    public string agree_code2 { get; set; }   // 본인확인: 생년월일(해시)
    public string agree_code3 { get; set; }   // 동의 링크 키
    public DateTime? create_dt { get; set; }
    public int? create_user { get; set; }
  }

  // 매핑 제외 보조 필드
  public partial class n_privacy_agree
  {
    [NotMapped] public int client_seq { get; set; }
    [NotMapped] public string client_name { get; set; }
  }

  // 개인정보 동의 상세 (수집·이용 / 제3자 제공 등 항목별)
  [Table("n_privacy_agree_dtl")]
  public partial class n_privacy_agree_dtl
  {
    [Key]
    public int pad_seq { get; set; }
    public int pa_seq { get; set; }           // 마스터키
    public int? agree_type { get; set; }      // 1:수집이용 / 2:제3자제공
    public int? client_seq { get; set; }
    public string client_name { get; set; }
    public int? is_agree { get; set; }        // 동의여부
    public int? is_optional { get; set; }     // 필수여부
    public string contents { get; set; }      // 동의 항목 내용(약관)
    public DateTime? create_dt { get; set; }
    public int? create_user { get; set; }
  }
}
