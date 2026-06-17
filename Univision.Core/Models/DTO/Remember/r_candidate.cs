using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public class r_candidate
  {
    /// <summary>
    /// pk
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 한국명
    /// <summary>
    public string kor_name { get; set; }

    /// <summary>
    /// 영문명
    /// <summary>
    public string eng_name { get; set; }

    /// <summary>
    /// 경력일수 계산용 필드1
    /// <summary>
    public DateTime? career_start_dt { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 수정일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 경력일수 계산용 필드2
    /// <summary>
    public DateTime? career_end_dt { get; set; }
    
    /// <summary>
    /// 총경력일수(재직중이 아닐 시)
    /// <summary>
    public int? career_range { get; set; }

    /// <summary>
    /// uv_user 담당자 pk
    /// <summary>
    public int? manager_seq { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_seq { get; set; }
    
    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_seq { get; set; }

    /// <summary>
    /// 후보자 키워드(추가)
    /// <summary>
    public string keyword { get; set; }

    /// <summary>
    /// SNS 주소1,2,3
    /// <summary>
    public string sns_link1 { get; set; }
    public string sns_link2 { get; set; }
    public string sns_link3 { get; set; }
    
    /// <summary>
    /// 생년월일
    /// <summary>
    public DateTime? birth_date { get; set; }

    /// <summary>
    /// 성별(남자:1 , 여자 : 2)
    /// <summary>
    public double? gender { get; set; }

    /// <summary>
    /// 생년월일 추정 여부(예 : 1 , 아니오:0)
    /// <summary>
    public double? ex_birthdate { get; set; }
    
    /// <summary>
    /// 국적코드
    /// <summary>
    public string country_code { get; set; }

    /// <summary>
    /// 외국인여부(0: 내국인, 1:외국인)
    /// <summary>
    public double? is_foreign { get; set; }

    /// <summary>
    /// 주소1
    /// <summary>
    public string addr1 { get; set; }

    /// <summary>
    /// 주소2
    /// <summary>
    public string addr2 { get; set; }

    /// <summary>
    /// 전화번호
    /// <summary>
    public string phone { get; set; }

    /// <summary>
    /// 전화번호 결번여부(추가)
    /// <summary>
    public int? wrong_phone { get; set; }

    /// <summary>
    /// 핸드폰번호
    /// <summary>
    public string cell_phone { get; set; }

    /// <summary>
    /// 전화번호2(휴대폰) 결번여부(추가)
    /// <summary>
    public int? wrong_phone2 { get; set; }

    /// <summary>
    /// 이메일1
    /// <summary>
    public string email1 { get; set; }

    /// <summary>
    /// 이메일2
    /// <summary>
    public string email2 { get; set; }
    
    /// <summary>
    /// 해외거주여부(추가)
    /// <summary>
    public int? ex_addr { get; set; }
    /// <summary>
    /// 희망연봉
    /// <summary>
    public int? hope_salary { get; set; }
    public int? hope_salary2 { get; set; }

    /// <summary>
    /// 컨피덴셜 여부(추가)
    /// <summary>
    public int? is_confidential { get; set; }
    public string confi_remark { get; set; }
    public DateTime? confidential_date { get; set; }
    public int? confidential_user { get; set; }

    /// <summary>
    /// 인액티브 여부(추가)
    /// <summary>
    public int? is_inactive { get; set; }
    public string inactive_remark { get; set; }
    public DateTime? inactive_date { get; set; }
    public int? inactive_user { get; set; }

    /// <summary>
    /// 새로운 후보자 주의요망사항
    /// </summary>
    public int? attention_type { get; set; }
    public string attention_remark { get; set; }
    public DateTime? attention_date { get; set; }
    public int? attention_user { get; set; }

    /// <summary>
    /// 등록경로(1 : 아웃써치, 2:온라인DB, 3:잡포탈, 온라인지원자, 4:아웃플레이스먼트, 5:링크드인 , 6:기타
    /// <summary>
    public int? reg_type { get; set; }

    /// <summary>
    /// 후보자 주의요망사항(0:해당없음/1:주의요망/2:경력 결함/3:도덕 결함/4: 인터뷰결함/5:주의요망/6:유니코채용/7:탈퇴요청
    /// --> 유니코 채용 구분만 사용
    /// </summary>
    public int? caution_type { get; set; }
  }
}
