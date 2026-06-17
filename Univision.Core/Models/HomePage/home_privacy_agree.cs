using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.HomePage
{
  //후보자 이력서 테이블
  [Table("privacy_agree")]
  public partial class home_privacy_agree
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int pa_seq { get; set; }

    /// <summary>
    /// 발송구분(S:SMS / E : mail)
    /// <summary>
    public string send_type { get; set; }

    public string send_addr { get; set; }

    /// <summary>
    /// 후보자명(수기)
    /// <summary>
    public string can_name { get; set; }

    /// <summary>
    /// 연락처(전화번호 또는 이메일)
    /// <summary>
    public string can_contact { get; set; }

    /// <summary>
    /// 후보자코드
    /// <summary>
    public int? c_seq { get; set; }

    /// <summary>
    /// 발송일자
    /// <summary>
    public DateTime? send_dt { get; set; }

    /// <summary>
    /// 동의서 전문
    /// <summary>
    public string full_txt { get; set; }

    /// <summary>
    /// 동의일자
    /// <summary>
    public DateTime? agree_dt { get; set; }

    /// <summary>
    /// 동의IP
    /// <summary>
    public string agree_ip { get; set; }

    /// <summary>
    /// 동의인증코드1
    /// <summary>
    public string agree_code1 { get; set; }

    /// <summary>
    /// 동의 인증코드 2
    /// <summary>
    public string agree_code2 { get; set; }

    /// <summary>
    /// 동의 인증코드 3
    /// <summary>
    public string agree_code3 { get; set; }

    /// <summary>
    /// 생성일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 생성자
    /// <summary>
    public int? create_user { get; set; }

    public string create_email { get; set; }
  }

  public partial class home_privacy_agree
  {
    /// <summary>
    /// 본인인증코드
    /// <summary>
    [NotMapped]
    public int agree_gubun { get; set; }
    [NotMapped]
    public string summary { get; set; }
    [NotMapped]
    public int client_seq { get; set; }
    [NotMapped]
    public string client_name { get; set; }
    [NotMapped]
    public string create_name { get; set; }
  }
}


