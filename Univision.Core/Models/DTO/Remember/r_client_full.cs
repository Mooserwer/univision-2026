using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public partial class r_client_full : r_client
  {
    public List<r_client_contract> fee_rate { get; set; } = new List<r_client_contract>();

  }
  //클라이언트
  public partial class r_client
  {
    /// <summary>
    /// pk
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 한글 회사명
    /// <summary>
    public string kor_name { get; set; }

    /// <summary>
    /// 영문 회사명
    /// <summary>
    public string eng_name { get; set; }

    /// <summary>
    /// 사업자번호
    /// <summary>
    public string biz_code { get; set; }


    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 최종계약서 업로드일
    /// <summary>
    public DateTime? upload_dt { get; set; }

    public int? contract_seq { get; set; }
    public string contract_date { get; set; }

    /// <summary>
    /// 수수료율 반영구분(연봉:a, 직급:b, 고정:c)
    /// <summary>
    public string fee_type { get; set; }

    public double? fix_fee_rate { get; set; }
  }

  public partial class r_client_contract
  {
    public string Currency_Name { get; set; }

    /// <summary>
    /// client pk
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 시작 금액
    /// <summary>
    public int? start_income { get; set; }

    /// <summary>
    /// 시작 포지션
    /// <summary>
    public string start_position { get; set; }

    /// <summary>
    /// 종료 금액
    /// <summary>
    public int? end_income { get; set; }

    /// <summary>
    /// 종료 포지션
    /// <summary>
    public string end_position { get; set; }

    /// <summary>
    /// 퍼센트지
    /// <summary>
    public decimal? percentage { get; set; }

  }

}


