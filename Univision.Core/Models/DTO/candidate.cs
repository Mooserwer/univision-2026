using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //후보자 테이블
  [Table("candidate")]
  public partial class candidate
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int c_seq { get; set; }

    /// <summary>
    /// uv_user 담당자 pk
    /// <summary>
    public int? manager_seq { get; set; }

    /// <summary>
    /// 한국명
    /// <summary>
    public string kor_name { get; set; }

    /// <summary>
    /// 영문명
    /// <summary>
    public string eng_name { get; set; }

    /// <summary>
    /// 외국인여부(0: 내국인, 1:외국인)
    /// <summary>
    public double? is_foreign { get; set; }

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
    /// 핸드폰번호
    /// <summary>
    public string cell_phone { get; set; }

    /// <summary>
    /// 이메일1
    /// <summary>
    public string email1 { get; set; }

    /// <summary>
    /// 이메일2
    /// <summary>
    public string email2 { get; set; }

    /// <summary>
    /// 희망연봉
    /// <summary>
    public int? hope_salary { get; set; }
    public int? hope_salary2 { get; set; }

    /// <summary>
    /// 인터뷰 여부?
    /// <summary>
    public int? after_interview { get; set; } = 0;

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_seq { get; set; }

    /// <summary>
    /// 수정일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_seq { get; set; }

    /// <summary>
    /// 후보자 키워드(추가)
    /// <summary>
    public string keyword { get; set; }

    /// <summary>
    /// 해외거주여부(추가)
    /// <summary>
    public int? ex_addr { get; set; }

    /// <summary>
    /// 전화번호 결번여부(추가)
    /// <summary>
    public int? wrong_phone { get; set; }

    /// <summary>
    /// 전화번호2(휴대폰) 결번여부(추가)
    /// <summary>
    public int? wrong_phone2 { get; set; }

    /// <summary>
    /// 컨피덴셜 여부(추가)
    /// <summary>
    public int? is_confidential { get; set; }

    /// <summary>
    /// 인액티브 여부(추가)
    /// <summary>
    public int? is_inactive { get; set; }
    /// <summary>
    /// 등록경로(1 : 아웃써치, 2:온라인DB, 3:잡포탈, 온라인지원자, 4:아웃플레이스먼트, 5:링크드인 , 6:기타
    /// <summary>
    public int? reg_type { get; set; }

    public string sns_link1 { get; set; }
    public string sns_link2 { get; set; }
    public string sns_link3 { get; set; }

    public string confi_remark { get; set; }

    public DateTime? confidential_date { get; set; }
    public int? confidential_user { get; set; }

    public string inactive_remark { get; set; }

    public DateTime? career_start_dt { get; set; }
    public DateTime? career_end_dt { get; set; }

    public int? career_range { get; set; }

    public DateTime? inactive_date { get; set; }
    public int? inactive_user { get; set; }
    /// <summary>
    /// 후보자 주의요망사항(0:해당없음/1:주의요망/2:경력 결함/3:도덕 결함/4: 인터뷰결함/5:주의요망/6:유니코채용/7:탈퇴요청
    /// --> 유니코 채용 구분만 사용
    /// </summary>
    public int? caution_type { get; set; }

    /// <summary>
    /// 새로운 후보자 주의요망사항
    /// </summary>
    public int? attention_type { get; set; }
    public string attention_remark { get; set; }
    public DateTime? attention_date { get; set; }
    public int? attention_user { get; set; }
  }

  /// <summary>
  /// 리스트에서 쓰임.
  /// </summary>
  public partial class candidate
  {
    [NotMapped]
    public int cur_career_month { get; set; } = 0;
    /// <summary>
    /// 후보자 최종 학교명
    /// </summary>
    [NotMapped]
    public string school_name { get; set; }

    /// <summary>
    /// 후보자 최종 학교 전공
    /// </summary>
    [NotMapped]
    public string major_name { get; set; }

    /// <summary>
    /// 후보자 최종 학교 전공계열
    /// </summary>
    [NotMapped]
    public string category1_name { get; set; }

    [NotMapped]
    public int graduate_status { get; set; }

    /// <summary>
    /// 최종 회사명
    /// </summary>
    [NotMapped]
    public string company_name { get; set; }

    [NotMapped]
    public string all_company { get; set; }

    [NotMapped]
    public string all_school { get; set; }

    /// <summary>
    /// 부서명
    /// </summary>
    [NotMapped]
    public string division_name { get; set; }

    [NotMapped]
    public string memo_str { get; set; }

    /// <summary>
    /// 재직 여부
    /// </summary>
    [NotMapped]
    public int is_work { get; set; }

    /// <summary>
    /// 직급 명
    /// </summary>
    [NotMapped]
    public string r_name { get; set; }

    /// <summary>
    /// 직책 명
    /// </summary>
    [NotMapped]
    public string p_name { get; set; }

    /// <summary>
    /// 후보자 직무
    /// </summary>
    [NotMapped]
    public string jobName { get; set; }

    /// <summary>
    /// 후보자 산업
    /// </summary>
    [NotMapped]
    public string busiName { get; set; }

    [NotMapped]
    public int cr_seq { get; set; } = 0;
    /// <summary>
    /// 국문이력서 파일 seq
    /// </summary>
    [NotMapped]
    public int kor_cr_seq { get; set; } = 0;
    /// <summary>
    /// 국문이력서 파일명
    /// </summary>
    [NotMapped]
    public string kor_file_name { get; set; }
    /// <summary>
    /// 국문이력서 파일 위치
    /// </summary>
    [NotMapped]
    public string kor_file_dir { get; set; }

    /// <summary>
    /// 국문이력서 파일 확장자
    /// </summary>
    [NotMapped]
    public string kor_file_ext { get; set; }
    /// <summary>
    /// 영문이력서 파일 seq
    /// </summary>
    [NotMapped]
    public int eng_cr_seq { get; set; } = 0;
    /// <summary>
    /// 영문이력서 파일명
    /// </summary>
    [NotMapped]
    public string eng_file_name { get; set; }
    /// <summary>
    /// 영문이력서 파일 위치
    /// </summary>
    [NotMapped]
    public string eng_file_dir { get; set; }

    /// <summary>
    /// 영문이력서 파일 확장자
    /// </summary>
    [NotMapped]
    public string eng_file_ext { get; set; }

    [NotMapped]
    public int cf_seq { get; set; }

    [NotMapped]
    public string kor_foreign_name { get; set; }

    [NotMapped]
    public string kor_resume_body { get; set; }

    [NotMapped]
    public string eng_resume_body { get; set; }

    [NotMapped]
    public string abilityDesc { get; set; }

    [NotMapped]
    public string kor_keyword { get; set; }

    [NotMapped]
    public string eng_keyword { get; set; }
    [NotMapped]
    public string manager_name { get; set; }
    [NotMapped]
    public string create_name { get; set; }
    [NotMapped]
    public string modify_name { get; set; }
    [NotMapped]
    public int? tempsaved_seq { get; set; }

    [NotMapped]
    public string country_name { get; set; }

    [NotMapped]
    public int is_external_lock { get; set; }

    [NotMapped]
    public int log_cnt { get; set; }
  }
  public partial class candidate
  {
    [NotMapped]
    public view_can_activity project_history { get; set; }
  }

  public partial class candidate
  {
    [NotMapped]
    public string confidential_name { get; set; }
    [NotMapped]
    public string inactive_name { get; set; }
    [NotMapped]
    public string attention_name { get; set; }
  }
}


