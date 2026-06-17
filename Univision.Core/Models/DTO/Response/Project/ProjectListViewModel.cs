using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Project
{
  public class ProjectListViewModel : EntityListViewModel
  {
    public ProjectStatusCntModel cntData { get; set; }
    public ProjectSearchModel search { get; set; }
    public List<project> list { get; set; }
  }

  public class ProjectSearchModel
  {
    public void replace_search_text()
    {
      this.total = replace_oth(this.total);
      this.title = replace_oth(this.title);
      this.client = replace_oth(this.client);
      this.user_name = replace_oth(this.user_name);
      this.assignment = replace_oth(this.assignment);
      this.requirement = replace_oth(this.requirement);
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
    
    public int state { get; set; } = 1; // 단계
    public int pjt_type { get; set; } = 0; // 프로젝트 종류
    public string total { get; set; } = ""; //통합 검색 키워드
    public List<code_position> positionList { get; set; } // 직급코드
    public int position_start { get; set; } = 0;// 직급 시작
    public int position_end { get; set; } = 0;// 직급 끝

    public List<code_business2> business { get; set; } = new List<code_business2>(); // 산업
    public List<code_job2> job { get; set; } = new List<code_job2>(); // 직무


    public string user_name { get; set; } = "";
    public string client { get; set; } = "";
    public string title { get; set; } = "";

    public string assignment { get; set; } = "";
    public string requirement { get; set; } = "";

    //기간검색
    public string range_field { get; set; } = "A.create_dt_dix";
    public string dt_start { get; set; } = "";
    public string dt_end { get; set; } = "";

    public string orderOption1 { get; set; } = "DESC";
    public string orderTxt1 { get; set; } = "create_dt_idx";

    public string orderOption2 { get; set; } = "DESC";
    public string orderTxt2 { get; set; } = "A.modify_dt_idx";

    public int? is_key_first { get; set; } = 1;

    public int is_pe { get; set; } = 9;
  }
}
