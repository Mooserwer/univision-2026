using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Remember
{
  public partial class r_client_full_v2 : r_client_v2
  {

    /// <summary>
    /// 고객사 AM LIST
    /// <summary>
    public List<r_client_manager_v2> am { get; set; } = new List<r_client_manager_v2>();
    /// <summary>
    /// 고객사 담당자
    /// <summary>
    public List<r_client_contact_v2> contact { get; set; } = new List<r_client_contact_v2>();
    /// <summary>
    /// 고객사 세금계산서 담당자
    /// <summary>
    public List<r_client_tax_contact_v2> tax_contact { get; set; } = new List<r_client_tax_contact_v2>();
    /// <summary>
    /// 고객사 메모
    /// <summary>
    public List<r_client_activity_log_v2> memo { get; set; } = new List<r_client_activity_log_v2>();

    /// <summary>
    /// 고객사 계약정보
    /// <summary>
    public r_client_contract_v2 contract { get; set; }

    /// <summary>
    /// 고객사 파일
    /// <summary>
    public List<r_client_file_v2> file { get; set; } = new List<r_client_file_v2>();
  }
  //클라이언트
  public partial class r_client_v2
  {
    /// <summary>
    ///PK(고객사코드)
    /// <summary>
    public int c_seq { get; set; }
    /// <summary>
    ///고객사 국문명
    /// <summary>
    public string kor_name { get; set; }
    /// <summary>
    ///고객사 영문명
    /// <summary>
    public string eng_name { get; set; }
    /// <summary>
    ///대표자명
    /// <summary>
    public string ceo { get; set; }
    /// <summary>
    ///주소
    /// <summary>
    public string addr1 { get; set; }
    /// <summary>
    ///국내사/외국계사 구분 (0:국내사 / 1 :외국계사)
    /// <summary>
    public int is_foreign_invest { get; set; } = 0;
    /// <summary>
    ///수동 계약여부 (0 : 미계약 / 1 : 계약)
    /// <summary>
    public int is_contract { get; set; } = 0;
    /// <summary>
    ///외국 주소 여부 (1 : 외국 소재지)
    /// <summary>
    public int is_foreign { get; set; } = 0;
    /// <summary>
    ///국가코드
    /// <summary>
    public string foreign_code { get; set; }
    /// <summary>
    ///국가명
    /// <summary>
    public string country_name { get; set; }
    /// <summary>
    ///사업자등록번호
    /// <summary>
    public string biz_code { get; set; }
    /// <summary>
    ///업태
    /// <summary>
    public string biz_type { get; set; }
    /// <summary>
    ///종목
    /// <summary>
    public string biz_category { get; set; }
    /// <summary>
    ///산업 대분류 코드
    /// <summary>
    public int? biz_industry_code1 { get; set; }
    /// <summary>
    ///산업 소분류 코드
    /// <summary>
    public int? biz_industry_code2 { get; set; }
    /// <summary>
    ///구인제목 고정 제목 (ex : 국내 IT 스타트업)
    /// <summary>
    public string fix_title { get; set; }
    /// <summary>
    ///홈페이지 주소
    /// <summary>
    public string homepage { get; set; }
    /// <summary>
    ///사원수
    /// <summary>
    public int employee_number { get; set; } = 0;
    /// <summary>
    ///연간매출액
    /// <summary>
    public int sales_amount { get; set; } = 0;
    /// <summary>
    ///오프리밋 여부 (0: 미해당, 1:모든 임직원, 2:유니코 채용 임직원, 3:기타 케이스)
    /// <summary>
    public int offlimit { get; set; } = 0;
    /// <summary>
    ///최초등록일자
    /// <summary>
    public DateTime create_dt { get; set; }
    /// <summary>
    ///최초등록자 (사용자 코드)
    /// <summary>
    public int create_user { get; set; }
    /// <summary>
    ///최종수정일자
    /// <summary>
    public DateTime modify_dt { get; set; }
    /// <summary>
    ///최종수정자 (사용자 코드)
    /// <summary>
    public int modify_user { get; set; }
    /// <summary>
    ///BD / 인오더 여부 (0: BD, 1:인오더, 2:올인원, 3::BD매니져관리)
    /// <summary>
    public int is_inorder { get; set; } = 0;
    /// <summary>
    ///회사명 동의어 수동관리 (,로 구분)
    /// <summary>
    public string offlimit_keyword { get; set; }
    /// <summary>
    ///PE사 여부 (0: 미해당, 1: PE사)
    /// <summary>
    public int is_portfolio { get; set; } = 0;
    
  }

  /// <summary>
  /// AM
  /// </summary>
  public partial class r_client_manager_v2
  {
    /// <summary>
    ///PK
    /// <summary>
    public int seq { get; set; }
    /// <summary>
    ///고객사 코드
    /// <summary>
    public int c_seq { get; set; }
    /// <summary>
    ///사용자 코드
    /// <summary>
    public int uv_seq { get; set; }
    /// <summary>
    ///컨설턴트 이름
    /// <summary>
    public string uv_name { get; set; }

  }

  /// <summary>
  /// 고객사 담당자
  /// </summary>
  public partial class r_client_contact_v2
  {
    /// <summary>
    ///PK(고객사담당자)
    /// <summary>
    public int cc_seq { get; set; }
    /// <summary>
    ///고객사 코드
    /// <summary>
    public int c_seq { get; set; }
    /// <summary>
    ///담당자명
    /// <summary>
    public string name { get; set; }
    /// <summary>
    ///성별 (1: 남, 2:여)
    /// <summary>
    public int gender { get; set; } = 0;
    /// <summary>
    ///이메일
    /// <summary>
    public string email { get; set; }
    /// <summary>
    ///전화번호
    /// <summary>
    public string phone { get; set; }
    /// <summary>
    ///휴대폰번호
    /// <summary>
    public string cell_phone { get; set; }
    /// <summary>
    ///부서
    /// <summary>
    public string division { get; set; }
    /// <summary>
    ///직급
    /// <summary>
    public string position { get; set; }
    /// <summary>
    ///기타 메모
    /// <summary>
    public string memo { get; set; }
    /// <summary>
    ///최초등록일자
    /// <summary>
    public DateTime create_dt { get; set; }
    /// <summary>
    ///최초등록자 (사용자 코드)
    /// <summary>
    public int create_user { get; set; }
    /// <summary>
    ///최종수정일자
    /// <summary>
    public DateTime modify_dt { get; set; }
    /// <summary>
    ///최종수정자 (사용자 코드)
    /// <summary>
    public int modify_user { get; set; }

  }

  /// <summary>
  /// 고객사 계산서 담당자
  /// </summary>
  public partial class r_client_tax_contact_v2
  {
    /// <summary>
    ///PK(세금계산서 담당자)
    /// <summary>
    public int ctc_seq { get; set; }
    /// <summary>
    ///고객사코드
    /// <summary>
    public int c_seq { get; set; }
    /// <summary>
    ///부서
    /// <summary>
    public string division { get; set; }
    /// <summary>
    ///이름
    /// <summary>
    public string name { get; set; }
    /// <summary>
    ///이메일
    /// <summary>
    public string email { get; set; }
    /// <summary>
    ///전화번호
    /// <summary>
    public string phone { get; set; }
    /// <summary>
    ///휴대폰번호
    /// <summary>
    public string cell_phone { get; set; }
    /// <summary>
    ///발행용 이름
    /// <summary>
    public string deposit_manager { get; set; }
    /// <summary>
    ///발행용 이메일
    /// <summary>
    public string deposit_email { get; set; }
    /// <summary>
    ///최초등록일자
    /// <summary>
    public DateTime create_dt { get; set; }
    /// <summary>
    ///최초등록자 (사용자 코드)
    /// <summary>
    public int create_seq { get; set; }
    /// <summary>
    ///최종수정일자
    /// <summary>
    public DateTime modify_dt { get; set; }
    /// <summary>
    ///최종수정자 (사용자 코드)
    /// <summary>
    public int modify_seq { get; set; }

  }

  /// <summary>
  /// 고객사 메모
  /// </summary>
  public partial class r_client_activity_log_v2
  {
    /// <summary>
    ///PK(메모)
    /// <summary>
    public int cal_seq { get; set; }
    /// <summary>
    ///고객사코드
    /// <summary>
    public int c_seq { get; set; }
    /// <summary>
    ///제목
    /// <summary>
    public string title { get; set; }
    /// <summary>
    ///내용
    /// <summary>
    public string log_comment { get; set; }
    /// <summary>
    ///내용에 대한 일정(스케쥴 연동 시)
    /// <summary>
    public DateTime? log_date { get; set; }
    /// <summary>
    ///스케쥴 연동 여부 (1:연동)
    /// <summary>
    public int is_schedule { get; set; } = 0;
    /// <summary>
    ///최초등록일자
    /// <summary>
    public DateTime create_dt { get; set; }
    /// <summary>
    ///최초등록자 (사용자 코드)
    /// <summary>
    public int create_user { get; set; }
    /// <summary>
    ///최종수정일자
    /// <summary>
    public DateTime modify_dt { get; set; }
    /// <summary>
    ///최종수정자 (사용자 코드)
    /// <summary>
    public int modify_user { get; set; }
    /// <summary>
    ///메모 종류 (1 : BD/Progress, 2: 회사정보 , 9:기타)
    /// <summary>
    public int act_type { get; set; }

  }

  /// <summary>
  /// 고객사 파일
  /// </summary>
  public partial class r_client_file_v2
  {
    /// <summary>
    ///PK(파일)
    /// <summary>
    public int cf_seq { get; set; }
    /// <summary>
    ///고개사코드
    /// <summary>
    public int c_seq { get; set; }
    /// <summary>
    ///파일종류 (1:사업자등록증, 2:조직도, 3:기타, 4:계약서)
    /// <summary>
    public string cf_type { get; set; }

    /// <summary>
    ///경로관련
    /// <summary>
    public string origin_file_path { get; set; }
    /// <summary>
    ///파일명
    /// <summary>
    public string file_path { get; set; }
    /// <summary>
    ///확장자
    /// <summary>
    public string extension { get; set; }
    /// <summary>
    ///등록일자
    /// <summary>
    public DateTime create_dt { get; set; }
    /// <summary>
    ///등록자
    /// <summary>
    public int create_user { get; set; }

  }

  public partial class r_client_contract_v2
  {

    public int cc_seq { get; set; }
    /// <summary>
    ///고객사 코드
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    ///수수료 기준 ('A' : 연봉 기반, 'B' : 직급 기반, 'C' : 고정수수료)
    /// <summary>
    public string fee_type { get; set; }
    /// <summary>
    ///고정수수료 일 경우 고정 요율
    /// <summary>
    public float? fix_fee_rate { get; set; }
    /// <summary>
    ///금액단위
    /// <summary>
    public string currency_cd { get; set; }
    /// <summary>
    ///부가세 기준 (0:VAT포함, 1:VAT별도, 2:영세, 3:면세)
    /// <summary>
    public int vat_type { get; set; }
    /// <summary>
    ///최초계약일
    /// <summary>
    public string contract_created { get; set; }
    /// <summary>
    ///최종계약일
    /// <summary>
    public string contract_updated { get; set; }
    /// <summary>
    ///기초조사비 공제여부 (Y:공제/N:해당없음)
    /// <summary>
    public string is_construct_debut { get; set; }
    /// <summary>
    ///입금일 기준1 (ex: 계산서를 수령한 날로부터)
    /// <summary>
    public string deposit_gubun1 { get; set; }
    /// <summary>
    ///입금일 기준2 (ex: 2)
    /// <summary>
    public int? deposit_period { get; set; }
    /// <summary>
    ///입금일 기준3 (ex: 주 이내)
    /// <summary>
    public string deposit_gubun2 { get; set; }
    /// <summary>
    ///보증기간 기준1 (ex : 입사일로 부터)
    /// <summary>
    public string grt_type { get; set; }
    /// <summary>
    ///보증기간 기준2 (ex : 90)
    /// <summary>
    public int grt_period { get; set; }
    /// <summary>
    ///보증기간 기준3 (ex : 일)
    /// <summary>
    public string grt_gubun { get; set; }
    /// <summary>
    ///계약참고사항
    /// <summary>
    public string contract_comment { get; set; }

    /// <summary>
    /// 고객사 AM LIST
    /// <summary>
    public List<r_contract_fee_rate_v2> fee_rate { get; set; } = new List<r_contract_fee_rate_v2>();

  }

  public partial class r_contract_fee_rate_v2
  {
    /// <summary>
    /// client pk
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 시작 금액
    /// <summary>
    public float? start_income { get; set; }

    /// <summary>
    /// 시작 포지션
    /// <summary>
    public string start_position { get; set; }

    /// <summary>
    /// 종료 금액
    /// <summary>
    public float? end_income { get; set; }

    /// <summary>
    /// 종료 포지션
    /// <summary>
    public string end_position { get; set; }

    /// <summary>
    /// 퍼센트지
    /// <summary>
    public float? percentage { get; set; }

  }

}


