using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
  public class CandidateListViewModel : EntityListViewModel
  {
    public CandidateSearchModel search { get; set; }
    public CandidateSearchOptionModel search_opt { get; set; }
    public List<candidate> list { get; set; }
  }

  public class CandidateSearchModel
  {
    public void replace_search_text()
    {
      this.total = replace_oth(this.total);
      this.name = replace_oth(this.name);
      this.email = replace_oth(this.email).ToLower();
      this.school = replace_oth(this.school);
      this.major = replace_oth(this.major);
      this.company = replace_oth(this.company);
      this.sns = replace_oth(this.sns);
      this.keyword = replace_oth(this.keyword);
      this.memo = replace_oth(this.memo);
      this.expr = replace_oth(this.expr);
    }

    string replace_oth(string val)
    {
      if (!String.IsNullOrEmpty(val))
      {
        return val.Replace("\\", "").Replace("'", "").Replace("*", "");
      }
      else
      {
        return "";
      }

    }

    public int is_sugg { get; set; } = 0;
    public string total { get; set; } = "";
    public string name { get; set; } = "";
    //외국인여부 검색 추가
    public bool foreign { get; set; }
    public string startBirth { get; set; } = "";
    public string endBirth { get; set; } = "";
    public string phone { get; set; } = "";
    public string email { get; set; } = "";
    //성별 검색 추가
    public int gender { get; set; }

    public int career_start { get; set; }
    public int career_end { get; set; }


    public string school { get; set; } = "";

    public string expr { get; set; } = "";
    //해외 학교여부
    public bool foreign_school { get; set; }
    //전공구분
    public string category1_name { get; set; } = "";
    //전공
    public string major { get; set; } = "";
    //최종학력
    public List<int> final_edu { get; set; } = new List<int>();

    public string reg_type { get; set; } = "";
    //회사명
    public string company { get; set; } = "";
    //최종회사만 검색
    public bool final_company { get; set; }

    //직급
    public string position { get; set; } = "";

    public string group_name { get; set; } = "";
    //직위
    public string position_name { get; set; } = "";
    public string isWork { get; set; } = "";
    //어학능력
    public string language { get; set; } = "";
    //어학능력
    public int ability { get; set; }
    //키워드
    public string keyword { get; set; } = "";
    //등록일
    public string create_dt_start { get; set; } = "";
    public string create_dt_end { get; set; } = "";

    public string modify_dt_start { get; set; } = "";
    public string modify_dt_end { get; set; } = "";

    public List<code_business_dtl> business { get; set; } = new List<code_business_dtl>();
    public string business_search_type { get; set; } = "OR";
    public List<code_job_dtl> job { get; set; } = new List<code_job_dtl>();
    public string job_search_type { get; set; } = "OR";
    public string sns { get; set; } = "";

    public int confidential { get; set; } = -1;
    public int inactive { get; set; } = -1;
    public int recent_hire { get; set; } = -1;

    public string memo { get; set; } = "";

    public List<int> except_c_seq { get; set; } = new List<int>();
    public List<string> keyword_list { get; set; } = new List<string>();

    public string orderOption { get; set; } = "DESC";
    public string orderTxt { get; set; } = "modify_dt_idx";
  }

  public class CandidateSearchOptionModel
  {
    public bool total { get; set; } = true;
    public string total_and_or { get; set; } = "OR";
    public bool total_include_personal { get; set; } = true;
    public bool total_include_edu { get; set; } = true;
    public bool total_include_expr { get; set; } = true;
    public bool total_include_keyword { get; set; } = true;
    public bool total_include_memo { get; set; } = true;
    public bool total_include_resume { get; set; } = true;
    


    public bool name { get; set; } = false;

    public bool inc_no_phone { get; set; } = true;
    public bool inc_no_email { get; set; } = true;

    public bool school { get; set; } = true;

    public string school_and_or { get; set; } = "OR";   

    public bool expr { get; set; } = true;
    public string expr_and_or { get; set; } = "OR";
    public bool major { get; set; } = true;

    public string major_and_or { get; set; } = "OR";
    public bool keyword { get; set; } = true;
    public string keyword_and_or { get; set; } = "OR";
    public bool memo { get; set; } = true;
    public string memo_and_or { get; set; } = "OR";
    public bool resume { get; set; } = true;
    public string resume_and_or { get; set; } = "OR";
  }
}
