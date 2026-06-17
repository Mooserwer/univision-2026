using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public class r_project_full_v2 : r_project_v2
  {
    public List<r_project_user> am { get; set; } = new List<r_project_user>();
    public List<r_project_user> sm { get; set; } = new List<r_project_user>();

    public List<r_pjt_place> placeList { get; set; } = new List<r_pjt_place>();
    public List<r_pjt_recandidate_v2> candidates { get; set; } = new List<r_pjt_recandidate_v2>();
    
  }

  //프로젝트 테이블
  public partial class r_project_v2
  {
    /// <summary>
    /// pk
    /// <summary>
    public int p_seq { get; set; }

    /// <summary>
    /// 회사 seq
    /// </summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 회사 국문명
    /// </summary>
    public string kor_name { get; set; }
    /// <summary>
    /// 회사 영문명
    /// </summary>
    public string eng_name { get; set; }

    /// <summary>
    /// 프로젝트 구분
    /// </summary>
    public int pjt_type { get; set; }

    /// <summary>
    /// 프로젝트 구분(명)
    /// </summary>
    public string pjt_type_str { get; set; }


    /// <summary>
    /// 프로젝트 상태 ( 진행 : 1, 보류 : 2, 실패 : 3, 완료 : 4, 성공 : 5)
    /// <summary>
    public int? pjt_status { get; set; }

    /// <summary>
    /// 프로젝트 상태 
    /// <summary>
    public string pjt_status_str { get; set; }

    /// <summary>
    /// 프로젝트 상태 사유
    /// <summary>
    public string status_comment { get; set; }

    /// <summary>
    /// PE프로젝트 구분
    /// <summary>
    public int? is_pe { get; set; }
    /// <summary>
    /// PE프로젝트 구분
    /// <summary>
    public int? is_posting { get; set; }

    /// <summary>
    /// 제목
    /// <summary>
    public string title { get; set; }

    /// <summary>
    /// 예상연봉
    /// <summary>
    public double? exp_salary { get; set; }
    /// <summary>
    /// 예상연봉(원화)
    /// <summary>
    public double? exp_salary_won { get; set; }
    /// <summary>
    /// 예상연봉 금액단위
    /// <summary>
    public string currency_cd { get; set; }
    /// <summary>
    /// 수수료율
    /// <summary>
    public decimal? fee_rate { get; set; }

    /// <summary>
    /// 실제 position
    /// <summary>
    public string position_str { get; set; }

    /// <summary>
    /// position 테이블 seq
    /// <summary>
    public int? position_seq { get; set; }
    /// <summary>
    /// position 테이블 seq 이름
    /// <summary>
    public string position_name { get; set; }

    /// <summary>
    /// 산업코드 1단계
    /// </summary>
    public double? business_code1 { get; set; }
    public string business_name1 { get; set; }
    /// <summary>
    /// 산업코드 2단계
    /// </summary>
    public double? business_code2 { get; set; }
    public string business_name2 { get; set; }
    /// <summary>
    /// 보조산업코드 1단계
    /// </summary>
    public double? sub_business_code1 { get; set; }
    public string sub_business_name1 { get; set; }
    /// <summary>
    /// 보조산업코드 2단계
    /// </summary>
    public double? sub_business_code2 { get; set; }
    public string sub_business_name2 { get; set; }
    /// <summary>
    /// 직무코드 1단계
    /// </summary>
    public int job_code1 { get; set; }
    public string job_name1 { get; set; }
    /// <summary>
    /// 직무코드 2단계
    /// </summary>
    public int job_code2 { get; set; }
    public string job_name2 { get; set; }
    /// <summary>
    /// 보조직무코드 1단계
    /// </summary>
    public int? sub_job_code1 { get; set; }
    public string sub_job_name1 { get; set; }
    /// <summary>
    /// 보조직무코드 2단계
    /// </summary>
    public int? sub_job_code2 { get; set; }
    public string sub_job_name2 { get; set; }

    /// <summary>
    /// 학력조건
    /// </summary>
    public string edu_name { get; set; }
    /// <summary>
    /// 언어조건(언어 종류)
    /// </summary>
    public string language_name { get; set; }

    /// <summary>
    /// 언어조건(언어 레벨)
    /// </summary>
    public string language_level { get; set; }

    /// <summary>
    /// 담당업무
    /// </summary>
    public string assign_task { get; set; }

    /// <summary>
    /// 자격요건
    /// </summary>
    public string requirement { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 수정일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    ///완료/성공/종료 시 일자
    /// <summary>
    public DateTime? close_dt { get; set; }

    public int create_user { get; set; }

    public int modify_user { get; set; }

    public string internal_contents {get;set;}
    public string client_require { get; set; }
    public string internal_note { get; set; }

    public int experience_type { get; set; }
    public int experience_year { get; set; }
  }

  public partial class r_pjt_recandidate_v2
  {
    public int pic_seq { get; set; }

    public int p_seq { get; set; }

    public int c_seq { get; set; }

    public DateTime create_dt { get; set; }

    public int create_user { get; set; }

    public int cur_state { get; set; }
    public string cur_state_str { get; set; }
    public int pre_state { get; set; }
    public string pre_state_str { get; set; }

    public List<r_pjt_recandidate_history_v2> history = new List<r_pjt_recandidate_history_v2>();

  }

  public partial class r_pjt_recandidate_history_v2
  {
    public int prc_seq { get; set; }
    public int pic_seq { get; set; }
    public int p_seq { get; set; }

    public int c_seq { get; set; }

    /// <summary>
    /// 진행상태 (관심후보:0, 추천: 10, 서류통과: 20, 면접: 30, 면접통과 : 40, 채용준비 : 50, 채용확정: 60, 탈락:99)
    /// <summary>
    public int? state { get; set; }

    /// <summary>
    /// 진행상태 (관심후보:0, 추천: 10, 서류통과: 20, 면접: 30, 면접통과 : 40, 채용준비 : 50, 채용확정: 60, 탈락:99)
    /// <summary>
    public string state_str { get; set; }
    /// <summary>
    /// 일정
    /// <summary>
    public DateTime? schedule_date { get; set; }

    public string contents { get; set; }
    public string file_origin_path { get; set; }
    public string file_path { get; set; }
    public int  is_no_invoice { get; set; }
    public float? ann_income { get; set; }
    public float? ann_income_won { get; set; }
    public string  currency_cd { get; set; }
    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_user { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? modify_user { get; set; }
  }
}


