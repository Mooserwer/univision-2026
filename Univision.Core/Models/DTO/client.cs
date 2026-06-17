using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //클라이언트
  [Table("client")]
  public partial class client
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
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
    /// 대표자
    /// <summary>
    public string ceo { get; set; }

    /// <summary>
    /// 주소1
    /// <summary>
    public string addr1 { get; set; }

    /// <summary>
    /// 주소2
    /// <summary>
    public string addr2 { get; set; }

    /// <summary>
    /// 외국인투자여부
    /// <summary>
    public int is_foreign_invest { get; set; }

    /// <summary>
    /// 계약 여부
    /// <summary>
    public int is_contract { get; set; }

    /// <summary>
    /// 외국회사 여부
    /// <summary>
    public int is_foreign { get; set; }

    /// <summary>
    /// 외국 코드
    /// <summary>
    public string foreign_code { get; set; }

    /// <summary>
    /// 국적명
    /// <summary>
    public string country_name { get; set; }

    /// <summary>
    /// 사업자번호
    /// <summary>
    public string biz_code { get; set; }

    /// <summary>
    /// 업태 명
    /// <summary>
    public string biz_type { get; set; }

    /// <summary>
    /// 업태 코드
    /// <summary>
    public string biz_type_code { get; set; }

    /// <summary>
    /// 종목 명
    /// <summary>
    public string biz_category { get; set; }

    /// <summary>
    /// 종목 코드
    /// <summary>
    public string biz_category_code { get; set; }

    /// <summary>
    /// 산업구분
    /// <summary>
    public string biz_industry { get; set; }

    /// <summary>
    /// 산업구분코드
    /// <summary>
    public double? biz_industry_code1 { get; set; }

    public double? biz_industry_code2 { get; set; }

    /// <summary>
    /// 고정 타이틀
    /// <summary>
    public string fix_title { get; set; }

    /// <summary>
    /// 홈페이지
    /// <summary>
    public string homepage { get; set; }

    /// <summary>
    /// 사원수
    /// <summary>
    public int? employee_number { get; set; }

    /// <summary>
    /// 연 매출액
    /// <summary>
    public int? sales_amount { get; set; }

    /// <summary>
    /// 주 담당자
    /// <summary>
    public int? main_contract { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_user { get; set; }

    /// <summary>
    /// 수정일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_user { get; set; }

    public int is_inorder { get; set; }

    public int is_portfolio { get; set; }
        
    /// <summary>
    /// 구직활동 제한
    /// <summary>
    public int? offlimit { get; set; }

    /// <summary>
    /// BD등록자
    /// <summary>
    public int? bd_user_seq { get; set; }

    public string offlimit_keyword { get; set; }
    [NotMapped]
    public string business_name1 { get; set; }

    [NotMapped]
    public string business_name2 { get; set; }

    [NotMapped]
    public string biz_industry_name1 { get; set; }

    [NotMapped]
    public string biz_industry_name2 { get; set; }
  }


  public partial class client
  {

    [NotMapped]
    public DateTime? pjt_modify { get; set; }

    [NotMapped]
    public DateTime? pjt_create { get; set; }

    [NotMapped]
    public DateTime? status_modify { get; set; }

    [NotMapped]
    public DateTime? status_create { get; set; }


  }


  public partial class client
  {
    [NotMapped]
    public string create_name { get; set; }
    [NotMapped]
    public string modigy_name { get; set; }

    /// <summary>
    /// BD등록자
    /// <summary>
    [NotMapped]
    public string bd_user_name { get; set; }

  }

  public partial class client
  {
    [NotMapped]
    public string main_contract_name { get; set; }
    [NotMapped]
    public string main_contract_dept { get; set; }
  }

  public partial class client
  {
    [NotMapped]
    public int contact_seq { get; set; }
    [NotMapped]
    public string contact_name { get; set; }
    [NotMapped]
    public int contact_gender { get; set; }
    [NotMapped]
    public string contact_email { get; set; }
    [NotMapped]
    public string contact_phone { get; set; }
    [NotMapped]
    public string contact_cell_phone { get; set; }
    [NotMapped]
    public string contact_division { get; set; }
    [NotMapped]
    public string contact_position { get; set; }
    [NotMapped]
    public string memo { get; set; }
    [NotMapped]
    public int cc_seq { get; set; }

    [NotMapped]
    public string projectTitle { get; set; }
    [NotMapped]
    public string searcher_name { get; set; }
    [NotMapped]
    public DateTime? last_project_dt { get; set; }
    [NotMapped]
    public int billing_money { get; set; }
    [NotMapped]
    public int tax_seq { get; set; }
    [NotMapped]
    public string tax_name { get; set; }
    [NotMapped]
    public string tax_division { get; set; }
    [NotMapped]
    public string tax_email { get; set; }
    [NotMapped]
    public string tax_phone { get; set; }
    [NotMapped]
    public string tax_cell_phone { get; set; }
    [NotMapped]
    public string tax_deposit_email { get; set; }
    [NotMapped]
    public string tax_deposit_manager { get; set; }

    [NotMapped]
    public DateTime? lst_client_date { get; set; }

    [NotMapped]
    public int is_external_lock { get; set; } = 0;
  }


  public partial class client
  {

    /// <summary>
    /// 전체 프로젝트 수량
    /// <summary>
    [NotMapped]
    public int totalCnt { get; set; }

    /// <summary>
    /// 진행중인 프로젝트 수량
    /// <summary>
    [NotMapped]
    public int progressCnt { get; set; }

    /// <summary>
    /// 보류 프로젝트 수량
    /// </summary>
    [NotMapped]
    public int waitCnt { get; set; }

    /// <summary>
    /// 실패(취소) 프로젝트 수량
    /// <summary>
    [NotMapped]
    public int failCnt { get; set; }

    /// <summary>
    /// 완료한 프로젝트 수량
    /// </summary>
    [NotMapped]
    public int completeCnt { get; set; }

    /// <summary>
    /// 성공한 프로젝트 수량
    /// <summary>
    [NotMapped]
    public int successCnt { get; set; }

    //public string email { get; set; }

    /// <summary>
    /// AM명
    /// <summary>
    [NotMapped]
    public string am_name { get; set; }

    [NotMapped]
    public int? am_seq { get; set; }

    [NotMapped]
    public int? cf_seq { get; set; }

    [NotMapped]
    public string title { get; set; }

    [NotMapped]
    public int cal_seq { get; set; }

    [NotMapped]
    public DateTime log_date { get; set; }

    [NotMapped]
    public string log_hour { get; set; }

    [NotMapped]
    public string log_min { get; set; }

    [NotMapped]
    public string log_comment { get; set; }

    [NotMapped]
    public string contract_date { get; set; }

    [NotMapped]
    public string fee_type { get; set; }

    [NotMapped]
    public string deposit_limit { get; set; }

    [NotMapped]
    public string guarntee_type { get; set; }

    [NotMapped]
    public string is_construct_debut { get; set; }

    [NotMapped]
    public float? fix_fee_rate { get; set; }

    [NotMapped]
    public string file_directory { get; set; }

    [NotMapped]
    public string draft_contract_path { get; set; }

    [NotMapped]
    public string manual_contract_path { get; set; }

    [NotMapped]
    public string final_contract_path { get; set; }

    [NotMapped]
    public string contract_comment { get; set; }

    [NotMapped]
    public string currcurrency_name { get; set; }

    [NotMapped]
    public string incom_detail { get; set; }

    [NotMapped]
    public string position_detail { get; set; }

    [NotMapped]
    public int start_income { get; set; }

    [NotMapped]
    public int end_income { get; set; }

    [NotMapped]
    public int percentage { get; set; }

    [NotMapped]
    public int start_code_seq { get; set; }

    [NotMapped]
    public int end_code_seq { get; set; }

    [NotMapped]
    public string rate { get; set; }

    [NotMapped]
    public string feeValue { get; set; }
  }

  public partial class client
  {
    [NotMapped]
    public int log_cnt { get; set; }
  }

}


