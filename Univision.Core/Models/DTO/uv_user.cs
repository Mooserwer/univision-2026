using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //컨설턴트 테이블
  [Table("uv_user")]
  public partial class uv_user
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int uv_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? ud_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? ua_seq { get; set; }

    /// <summary>
    /// 유저 아이디
    /// <summary>
    public string user_id { get; set; }

    /// <summary>
    /// 패스워드
    /// <summary>
    public string pwd { get; set; }

    /// <summary>
    /// 직급명
    /// </summary>
    public string rank_name { get; set; }

    /// <summary>
    /// 이메일
    /// <summary>
    public string email { get; set; }

    /// <summary>
    /// 사내전화번호(이현창 추가)
    /// <summary>
    public string tel { get; set; } = "";

    /// <summary>
    /// 휴대전화번호(이현창 추가)
    /// <summary>
    public string hp { get; set; } = "";

    /// <summary>
    /// 이름
    /// <summary>
    public string name { get; set; }

    /// <summary>
    /// 사진 이미지 폴더 경로
    /// <summary>
    public string img_dir { get; set; }

    /// <summary>
    /// 원본 파일명
    /// <summary>
    public string img_origin_path { get; set; }

    /// <summary>
    /// 수정된 파일명
    /// <summary>
    public string img_path { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 수정일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 입사일
    /// <summary>
    public DateTime? entry_date { get; set; }

    /// <summary>
    /// 퇴사일
    /// <summary>
    public DateTime? retire_date { get; set; }

    public string initial { get; set; }
    public int? is_out_login { get; set; }
    public string email_id { get; set; }
    public string email_pwd { get; set; }

    public string expertise { get; set; }
  }


  public partial class uv_user
  {
    /// <summary>
    /// 권한코드명
    /// <summary>
    [NotMapped]
    public string ua_name { get; set; }

    /// <summary>
    /// 부서코드명
    /// <summary>
    [NotMapped]
    public string ud_name { get; set; }

    /// <summary>
    /// 권한
    /// </summary>
    [NotMapped]
    public int level { get; set; }

    [NotMapped]
    public string gubun { get; set; }

    [NotMapped]
    public string pwd_rp { get; set; }

    [NotMapped]
    public int? leader_seq { get; set; }
    [NotMapped]
    public int? s_confirm_seq { get; set; }
    [NotMapped]
    public int? s_leader_seq { get; set; }
  }
}


