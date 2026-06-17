using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Project
{
  public class MyProjectSearchModel
  {
    public string searchOption { get; set; } = "";
    public string kor_name { get; set; } = "";
    public string searchTxt { get; set; } = "";
    public string searchTxt2 { get; set; } = "";

    public List<code_position> positionList { get; set; }

    public int? position_start { get; set; }
    public int? position_end { get; set; }

    public List<code_business2> business { get; set; } = new List<code_business2>();

    public List<code_job2> job { get; set; } = new List<code_job2>();

    public int state { get; set; } = 1;
    //관심후보 추가 시 이미 추가된 후보자가 있는 프로젝트 제외용
    public int except_c_seq { get; set; } = 0;

    public string orderOption { get; set; } = "DESC";
    public string orderTxt { get; set; } = "P.create_dt";

    public bool excelDown { get; set; } = false;

    public int pjt_type { get; set; } = 0;

    public string startDt { get; set; }
    public string endDt { get; set; }

    public int is_share { get; set; }
    public int is_all { get; set; }
  }
}
