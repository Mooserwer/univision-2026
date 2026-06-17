using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Candidate
{
  public class CandidateCreateModel
  {
    public int c_seq { get; set; }
    public int? tempsaved_seq { get; set; }
    public string mail_key { get; set; }
    public int? manager_seq { get; set; }
    public string manager_name { get; set; }
    public string kor_name { get; set; }
    public string eng_name { get; set; }
    public double? is_foreign { get; set; }
    public DateTime? birth_date { get; set; }
    public double? gender { get; set; }
    public double? ex_birthdate { get; set; }
    public string country_code { get; set; }
    public string country_name { get; set; }
    public string addr1 { get; set; }
    public int? ex_addr { get; set; }
    public string phone { get; set; }
    public int? wrong_phone { get; set; }
    public string cell_phone { get; set; }
    public int? wrong_phone2 { get; set; }
    public string email1 { get; set; }
    public string email2 { get; set; }
    public int? hope_salary { get; set; }
    public int? hope_salary2 { get; set; }
    public string keyword { get; set; }
    public string sns_link1 { get; set; }
    public string sns_link2 { get; set; }
    public string sns_link3 { get; set; }

    public int? is_confidential { get; set; }
    public int? is_inactive { get; set; }

    public string confi_remark { get; set; }

    public string inactive_remark { get; set; }

    public int? reg_type { get; set; }
    public List<can_school> schoolList { get; set; } = new List<can_school>();
    public List<can_career> companyList { get; set; } = new List<can_career>();
    public List<can_place> placeList { get; set; } = new List<can_place>();
    public List<can_job> jobList { get; set; } = new List<can_job>();
    public List<can_business> busiList { get; set; } = new List<can_business>();
    public List<can_foreign_lan> foreignList { get; set; } = new List<can_foreign_lan>();
    public List<can_resume> resumeList { get; set; } = new List<can_resume>();

    public List<can_job> gpt_jobList { get; set; } = new List<can_job>();
    public List<can_business> gpt_busiList { get; set; } = new List<can_business>();

    public string memo { get; set; }

    public int is_ai_reg { get; set; } = 0;

    /*
    public can_resume korResume { get; set; }
    public can_resume engResume { get; set; }
    public can_resume etcResume { get; set; }

    public ResumeData resumeData { get; set; }
    */
  }

  public class GptBusiness
  {
    public List<can_business> business { get; set; } = new List<can_business>();
}

  public class GptJob
  {
    public List<can_job> job { get; set; } = new List<can_job>();
  }
  public class SchoolDataModel
  {
    public int cs_seq { get; set; }
    public int sc_seq { get; set; }
    public string gubun { get; set; }
    public string schoolName { get; set; }
    public int is_foreign_school { get; set; }
    public string graduate_year { get; set; }
    public string admission_year { get; set; }
    public int graduate_status { get; set; }
    public int? is_transfer { get; set; }
    public string category1_name { get; set; }
    public string major_name { get; set; }
    public float? credit { get; set; }
    public int? total_credit { get; set; }
  }

  public class CompanyDataModel
  {
    public int g_seq { get; set; }
    public int cc_seq { get; set; }
    public string companyName { get; set; }
    public string division_name { get; set; }
    public string join_dt { get; set; }
    public string leave_dt { get; set; }
    public int? annual_income { get; set; }
    public int? is_work { get; set; }
    public int? r_code { get; set; }
    public int? p_code { get; set; }
    public List<CompanyJobModel> jobList { get; set; }
  }

  public class CompanyJobModel
  {
    public float job_code3 { get; set; }
    public string job_name3 { get; set; }
  }

  public class PlaceModel
  {
    public int cp_seq { get; set; }
    public string code1 { get; set; }
    public string area1 { get; set; }
    public string code2 { get; set; }
    public string area2 { get; set; }
  }

  public class JobBusinessModel
  {
    public float code1 { get; set; }
    public float code2 { get; set; }
    public float code3 { get; set; }
  }

  public class ForeignModel
  {
    public string code { get; set; }
    public int ability { get; set; }
  }

  public class ResumeData
  {
    public bool korStatus { get; set; } = false;
    public string korPath { get; set; } = "";
    public bool engStatus { get; set; } = false;
    public string engPath { get; set; } = "";
    public bool etcStatus { get; set; } = false;
    public string etcPath { get; set; } = "";
  }

  public class CandidateAiUpdateModel : CandidateCreateModel
  {
    public CandidateCreateModel new_cand { get; set; }
  }
}