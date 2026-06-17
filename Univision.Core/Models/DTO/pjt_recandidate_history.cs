using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //프로젝트 추천후보 테이블
  [Table("pjt_recandidate_history")]
  //프로젝트 추천후보 진행사항 테이블
  public partial class pjt_recandidate_history
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int prc_seq { get; set; }

    /// <summary>
    /// project_recandidate pk
    /// <summary>
    public int pic_seq { get; set; }

    /// <summary>
    /// candidate pk
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// project pk
    /// <summary>
    public int p_seq { get; set; }

    /// <summary>
    /// 진행상태 (관심후보:0, 추천: 10, 서류통과: 20, 면접: 30, 면접통과 : 40, 채용준비 : 50, 채용확정: 60, 탈락:99)
    /// <summary>
    public int? state { get; set; }

    /// <summary>
    /// 일정
    /// <summary>
    public DateTime? schedule_date { get; set; }


    /// <summary>
    /// 인보이스 미발행 시 사유 (0: 발행, 1:replace, 9:다른 인보이스와 통합 발행)
    /// <summary>
    public int? is_no_invoice { get; set; } = 0;

    /// <summary>
    /// 실연봉(사용안할 예정)
    /// <summary>
    public int? annual_income { get; set; }
    /// <summary>
    /// 실연봉
    /// <summary>
    public double? ann_income { get; set; }
    /// <summary>
    /// 실연봉(원화환산액)
    /// <summary>
    public double? ann_income_won { get; set; }

    public string currency_cd { get; set; }
    public DateTime? guarantee_dt { get; set; }
    /// <summary>
    /// 개런티
    /// <summary>
    public int? guarantee { get; set; }

    /// <summary>
    /// 내용
    /// <summary>
    public string contents { get; set; }

    /// <summary>
    /// 파일 경로
    /// <summary>
    public string file_directory { get; set; }

    /// <summary>
    /// 파일 원본이름
    /// <summary>
    public string file_origin_path { get; set; }

    /// <summary>
    /// 파일명
    /// <summary>
    public string file_path { get; set; }


    /// <summary>
    /// 스케줄 등록
    /// </summary>
    public int is_schedule { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_user { get; set; }

    /// <summary>
    /// 수정일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_user { get; set; }


  }

  public partial class pjt_recandidate_history
  {
    [NotMapped]
    public int pin_to_top { get; set; }
    /// <summary>
    /// 후보자 한글명
    /// </summary>
    [NotMapped]
    public string kor_name { get; set; }

    /// <summary>
    /// 후보자 영문명
    /// </summary>
    [NotMapped]
    public string eng_name { get; set; }

    [NotMapped]
    public string title { get; set; }
    [NotMapped]
    public string pjt_client { get; set; }
    [NotMapped]
    public int invoice_cnt { get; set; }

    [NotMapped]
    public string create_name { get; set; }

    [NotMapped]
    public string modify_name { get; set; }


    [NotMapped]
    public int? final_kor_cr_seq { get; set; }
    [NotMapped]
    public string final_kor_resume_dir { get; set; }
    [NotMapped]
    public string final_kor_resume_name { get; set; }
    [NotMapped]
    public DateTime? final_kor_resume_date { get; set; }
    [NotMapped]
    public int? final_eng_cr_seq { get; set; }
    [NotMapped]
    public string final_eng_resume_dir { get; set; }
    [NotMapped]
    public string final_eng_resume_name { get; set; }
    [NotMapped]
    public DateTime? final_eng_resume_date { get; set; }
    [NotMapped]
    public int? final_mkor_cr_seq { get; set; }
    [NotMapped]
    public string final_mkor_resume_dir { get; set; }
    [NotMapped]
    public string final_mkor_resume_name { get; set; }
    [NotMapped]
    public DateTime? final_mkor_resume_date { get; set; }
    [NotMapped]
    public int? final_meng_cr_seq { get; set; }
    [NotMapped]
    public string final_meng_resume_dir { get; set; }
    [NotMapped]
    public string final_meng_resume_name { get; set; }
    [NotMapped]
    public DateTime? final_meng_resume_date { get; set; }

    [NotMapped]
    public string email { get; set; }

    [NotMapped]
    public string phone { get; set; }

    [NotMapped]
    public string final_school { get; set; }

    [NotMapped]
    public string final_major { get; set; }

    [NotMapped]
    public string company_name { get; set; }

    [NotMapped]
    public DateTime? birth_date { get; set; }
    [NotMapped]
    public int gender { get; set; }
    [NotMapped]
    public int pa_seq { get; set; }
    [NotMapped]
    public DateTime? agree_dt { get; set; }
    [NotMapped]
    public DateTime? send_dt { get; set; }

  }
}


