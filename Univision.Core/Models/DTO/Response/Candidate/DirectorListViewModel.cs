using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
  public class DirectorListViewModel : EntityListViewModel
  {
    public DirectorSearchModel search { get; set; }
    public List<director> list { get; set; }
  }

  public class DirectorSearchModel
  {
    public void replace_search_text()
    {
      this.total = replace_oth(this.total);
      this.name = replace_oth(this.name);
      this.email = replace_oth(this.email);
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
    //회사명
    public string company { get; set; } = "";
    //최종회사만 검색
    public bool final_company { get; set; }

    //직급
    public string position { get; set; } = "";

    public string group_name { get; set; } = "";
    //직위
    public string position_name { get; set; } = "";
    public int position_type { get; set; } = 0;
    public int expr_type { get; set; } = 0;

    public int od_type { get; set; } = 0;
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
    public string orderTxt { get; set; } = "A.create_dt";
  }
}
