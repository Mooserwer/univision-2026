using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Project
{
  public class ProjectCreateModel
  {
    public int p_seq { get; set; }
    public int log_cnt { get; set; }
    public int c_seq { get; set; }
    public string company_name { get; set; }
    public int pjt_type { get; set; }
    public double? recruit_number { get; set; }
    public double? is_posting { get; set; }
    public string title { get; set; }
    public int? position_seq { get; set; }
    public string position_str { get; set; }
    public double? experience_type { get; set; }
    public double? expreience_number { get; set; }
    public double? edu_code { get; set; }
    public string edu_name { get; set; }
    public string foreign_lang { get; set; }
    public string foreign_lang_name { get; set; }
    public double? foreign_level { get; set; }
    public string assign_task { get; set; }
    public string requirement { get; set; }
    public string internal_contents { get; set; }
    
    public double? is_kor_resume { get; set; }
    public double? is_eng_resume { get; set; }

    public int? is_pe_pjt { get; set; } = 0;
    public int? is_portfolio { get; set; }
    public double? is_self_introduction { get; set; }
    public double? is_etc { get; set; }
    public string etc_comment { get; set; }
    public double? is_pre_interview { get; set; }
    public double? is_number { get; set; }
    public double? interview_number { get; set; }
    public double? is_personality_test { get; set; }
    public double? gender_type { get; set; }
    public double? start_age { get; set; }
    public double? end_age { get; set; }
    public int? target_school { get; set; }
    public string target_school_campus { get; set; }
    public string target_school_nm { get; set; }
    public string target_major { get; set; }
    public string target_major_nm { get; set; }
    public int? target_company { get; set; }
    public string target_company_nm { get; set; }
    public int? confidentiallity { get; set; }
    public int? expected_salary { get; set; }
    public double? exp_salary { get; set; }
    public double? exp_salary_won { get; set; }
    public string currency_cd { get; set; }
    public decimal? expected_won { get; set; }
    public decimal? fee_rate { get; set; }
    public double? pjt_status { get; set; }
    public string status_comment { get; set; }
    public string share_comments { get; set; }
    public string secret_info { get; set; }
    public double? share_fee_rate { get; set; }
    public int? create_user { get; set; }
    public double? business_code1 { get; set; }
    public double? business_code2 { get; set; }
    public string business_name1 { get; set; }
    public string business_name2 { get; set; }
    public double? sub_business_code1 { get; set; }
    public double? sub_business_code2 { get; set; }
    public string sub_business_name1 { get; set; }
    public string sub_business_name2 { get; set; }
    public int job_code1 { get; set; }
    public int job_code2 { get; set; }
    public string job_name1 { get; set; }
    public string job_name2 { get; set; }
    public int? sub_job_code1 { get; set; }
    public int? sub_job_code2 { get; set; }
    public string sub_job_name1 { get; set; }
    public string sub_job_name2 { get; set; }
    public int? is_share_pjt { get; set; } = 0;
    public int? is_share_3m { get; set; } = 0;
    public int? is_cowork { get; set; } = 0;

    public int is_my_key { get; set; } = 0;

    public int? is_pe { get; set; } = 0;

    public int? no_business { get; set; } = 0;
    public int? no_job { get; set; } = 0;

    public DateTime? open_dt { get; set; }

    public DateTime? close_dt { get; set; }

    public List<pjt_director> amList { get; set; } = new List<pjt_director>();
    public List<pjt_manager> searcherList { get; set; } = new List<pjt_manager>();
    //public List<pjt_job_code> jobList { get; set; } = new List<pjt_job_code>();
    //public List<pjt_business_code> busiList { get; set; } = new List<pjt_business_code>();
    public List<pjt_place> placeList { get; set; } = new List<pjt_place>();
    public List<pjt_keyword> keywordList { get; set; } = new List<pjt_keyword>();


    //전자 세금계산서 담당자
    public int? cc_seq { get; set; }
    public string tax_division { get; set; }
    public string tax_name { get; set; }
    public string tax_email { get; set; }
    public string tax_phone { get; set; }
    public string tax_cell_phone { get; set; }
    public string tax_deposit_manager { get; set; }
    public string tax_deposit_email { get; set; }

    //담당자
    public int? ctc_seq { get; set; }
    public string contact_name { get; set; }
    public int contact_gender { get; set; }
    public string contact_email { get; set; }
    public string contact_phone { get; set; }
    public string contact_cell_phone { get; set; }
    public string contact_division { get; set; }
    public string contact_position { get; set; }

    public List<code_position> positionList { get; set; }

    /// <summary>
    /// 클라이언트에서 넘어오는 프로젝트 카피를 위한 타입.
    /// </summary>
    public string type { get; set; }

    /// <summary>
    /// 인오더 seq
    /// </summary>
    public int i_seq { get; set; } = 0;
  }

  public class AmSearcherModel
  {
    public int uv_seq { get; set; }
  }

  public class JobBusinessModel
  {
    public float code1 { get; set; }
    public float code2 { get; set; }
    public float code3 { get; set; }
  }

  public class PlaceModel
  {
    public string code1 { get; set; }
    public string area1 { get; set; }
    public string code2 { get; set; }
    public string area2 { get; set; }
  }

  public class KeywordModel
  {
    public string pk_name { get; set; }
  }
}