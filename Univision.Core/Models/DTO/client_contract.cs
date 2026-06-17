using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //클라이언트 계약 정보
  [Table("client_contract")]
  public partial class client_contract
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int cc_seq { get; set; }

    /// <summary>
    /// client pk
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 계약일
    /// <summary>
    public string contract_date { get; set; }

    /// <summary>
    /// 수수료율 반영구분(연봉:a, 직급:b, 고정:c)
    /// <summary>
    public string fee_type { get; set; }

    //VAT 적용 구분 (포함 : 0, 별도 : 1(기본), 영세율:2, 면세:3)
    public int? vat_type { get; set; }
    /// <summary>
    /// 금액단위
    /// <summary>
    public string currency_cd { get; set; }


    /// <summary>
    /// 입금 예정일
    /// <summary>
    public string deposit_limit { get; set; }

    /// <summary>
    /// 입금기한 숫자
    /// <summary>
    public int? deposit_period { get; set; }
    /// <summary>
    /// 입금기한 구분(없음:N, 일:D, 월:M, 년:Y)
    /// <summary>
    public string deposit_gubun1 { get; set; }

    public string deposit_gubun2 { get; set; }

    /// <summary>
    /// 개런티 적용구분(연봉:a,직급:b, 고정:c)
    /// <summary>
    public string guarntee_type { get; set; }

    /// <summary>
    /// 후보자 보증기간(숫자)
    /// <summary>
    public int? grt_period { get; set; }
    /// <summary>
    /// 후보자 구분(없음:N, 일:D, 월:M, 년:Y)
    /// <summary>
    public string grt_gubun { get; set; }

    public string grt_type { get; set; }

    /// <summary>
    /// 기초 공사비 공제 여부
    /// <summary>
    public string is_construct_debut { get; set; }
    public int? construct_debut_per { get; set; }

    /// <summary>
    /// 고정 수수료율
    /// <summary>
    public decimal? fix_fee_rate { get; set; }

    /// <summary>
    /// 폴더 경로 명
    /// <summary>
    public string file_directory { get; set; }

    /// <summary>
    /// 계약서 초안 오리지날 파일명
    /// <summary>
    public string draft_contract_origin_path { get; set; }

    /// <summary>
    /// 계약서 초안 파일명
    /// <summary>
    public string draft_contract_path { get; set; }

    /// <summary>
    /// 수동 계약서오리지날  파일 명
    /// <summary>
    public string manual_contract_origin_path { get; set; }

    /// <summary>
    /// 수동 계약서 파일 명
    /// <summary>
    public string manual_contract_path { get; set; }

    /// <summary>
    /// 최종 계약서 오리지날 파일명
    /// <summary>
    public string final_contract_origin_path { get; set; }

    /// <summary>
    /// 최종 계약서 파일명
    /// <summary>
    public string final_contract_path { get; set; }

    /// <summary>
    /// 코멘트
    /// <summary>
    public string contract_comment { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? create_user { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? modify_user { get; set; }

  }

  public partial class client_contract
  {
    [NotMapped]
    public string currency_name { get; set; }
  }
}


