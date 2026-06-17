using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //후보자 활동내역
  public partial class view_can_activity
  {
    /// <summary>
    /// 타입(후보자 활동내역 : CA, 프로젝트 관련 내역 : PJ)
    /// <summary>
    public string table_type { get; set; }
    /// <summary>
    /// 테이블 고유 seq
    /// <summary>
    public int seq { get; set; }

    /// <summary>
    /// 후보자 코드
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 관련 프로젝트 코드
    /// <summary>
    public int p_seq { get; set; }

    /// <summary>
    /// 후보자 활동 상태값
    /// <summary>
    public string ca_status { get; set; }

    /// <summary>
    /// 관련 프로젝트 진행 상태코드
    /// <summary>
    public int state { get; set; }

    /// <summary>
    /// 채용확정 시 연봉
    /// <summary>
    public int annual_income { get; set; }

    public double? ann_income { get; set; }
    public double? ann_income_won { get; set; }
    public string ann_currency_cd { get; set; }
    /// <summary>
    /// 채용확정 시 보증기간
    /// <summary>
    public int guarantee { get; set; }

    /// <summary>
    /// 날짜(면접일, 미팅일 등)
    /// <summary>
    public DateTime? ca_date { get; set; }
    /// <summary>
    /// 내용
    /// <summary>
    public string memo { get; set; }

    /// <summary>
    /// 파일 url
    /// <summary>
    public string file_directory { get; set; }

    /// <summary>
    /// 파일 실제 경로
    /// <summary>
    public string file_origin_path { get; set; }

    /// <summary>
    /// 파일명
    /// <summary>
    public string file_path { get; set; }


    /// <summary>
    /// 작성일자
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 작성자
    /// <summary>
    public int? create_seq { get; set; }

    /// <summary>
    /// 수정일자
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_seq { get; set; }
  }

  public partial class view_can_activity
  {
    public string create_dt_str { get; set; }
    /// <summary>
    /// 고객사명
    /// <summary>
    public string client_name { get; set; }

    public string position_str { get; set; }

    public string position_nm { get; set; }
    /// <summary>
    /// 후보자명
    /// <summary>
    public string candidate_name { get; set; }
    /// <summary>
    /// 작성자명
    /// <summary>
    public string create_name { get; set; }

    /// <summary>
    /// 프로젝트 제목
    /// <summary>
    public string pjt_title { get; set; }

    public int pjt_status { get; set; }

    public int pjt_recan_his_cnt { get; set; }

    public int is_agree { get; set; } = 0;
    public int client_seq { get; set; }
    public DateTime? send_dt { get; set; }
    public DateTime? agree_dt { get; set; }

  }


}


