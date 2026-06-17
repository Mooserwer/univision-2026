using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //프로젝트 빌링 인포
  [Table("test_invoice_new")]
  public partial class test_invoice_new
  {
    public test_invoice_new()
    {
      // NullReferenceException 방지를 위해 생성자에서 리스트 초기화
      this.invoice_new_dtls = new HashSet<test_invoice_new_dtl>();
    }
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int in_seq { get; set; }

    /// <summary>
    /// 리멤버측 인보이스 고유 번호 (인덱스 필요)
    /// </summary>
    public long r_invoice_id { get; set; }

    /// <summary>
    /// 이전 인보이스 번호 (취소 환불 시 대상 인보이스 번호)
    /// <summary>
    public long? pre_invoice_id { get; set; }

    /// <summary>
    /// 인보이스 종류(0: Contingency, 1: Retainer, 2: Retainer Balance, 3:Other, 4:환불, 5:취소)
    /// <summary>
    public int invoice_type { get; set; }

    /// <summary>
    /// 인보이스 세부 종류(컨설팅)
    /// //CASE a.pjt_type WHEN 1 THEN '채용' WHEN 2 THEN'평판조회' WHEN 3 THEN '재취업' WHEN 4 THEN '채용대행' WHEN 5 THEN '마켓 매핑' WHEN 6 THEN '사외이사추천' WHEN 9 THEN '기타' END As pjt_type_str, -- 종류명
    /// <summary>
    public int invoice_sub_type { get; set; }

    /// <summary>
    /// 인보이스 언어 (0 : 국문, 1 : 영문)
    /// <summary>
    public int? invoice_lang { get; set; }

    /// <summary>
    /// 인보이스 신청자 고유 번호
    /// <summary>
    public long r_request_user_id { get; set; }
    /// <summary>
    /// 인보이스 신청자 이름
    /// <summary>
    public string r_request_user_name { get; set; }
    /// <summary>
    /// 인보이스 신청자 메일
    /// <summary>
    public string r_request_user_email { get; set; }
    /// <summary>
    /// 신청자의 유니비전 사용자 코드
    /// <summary>
    public int? request_uv_seq { get; set; }
    /// <summary>
    /// PO No 필요 여부 (0 : 불필요, 1 : 필요)
    /// <summary>
    public int? is_po_no { get; set; } = 0;

    /// <summary>
    /// 인보이스 후보자명 표시 (0 : No, 1: Yes)
    /// <summary>
    public int? is_open_name { get; set; } = 1;

    /// <summary>
    /// 인보이스 후보자 연봉 /수수료율 표시 여부 (0 : No, 1: Yes)
    /// <summary>
    public int? is_open_annual_income { get; set; } = 1;

    /// <summary>
    /// 인보이스 제목
    /// <summary>
    public string invoice_title { get; set; }
    /// <summary>
    /// 인보이스 내용
    /// <summary>
    public string invoice_contents { get; set; }
    /// <summary>
    /// 인보이스 입금은행명
    /// <summary>
    public string deposit_bank_name { get; set; }
    /// <summary>
    /// 인보이스 은행 계좌번호
    /// <summary>
    public string deposit_bank_account { get; set; }

    /// <summary>
    /// 비고
    /// <summary>
    public string remarks { get; set; }

    /// <summary>
    /// 내부 요청사항
    /// <summary>
    public string remark_admin { get; set; }

    /// <summary>
    /// 고객사 코드
    /// <summary>
    public long r_client_id { get; set; }

    /// <summary>
    /// 고객사 명
    /// <summary>
    public string client_name { get; set; }

    /// <summary>
    /// 고객사 대표자명
    /// <summary>
    public string client_ceo { get; set; }

    /// <summary>
    /// 고객사 주소
    /// <summary>
    public string client_addr1 { get; set; }

    /// <summary>
    /// 고객사 사업자번호
    /// <summary>
    public string client_biz_code { get; set; }
    
    /// <summary>
    /// 기초조사비 공제 비율 
    /// <summary>
    public decimal? client_basement_per { get; set; }
    /// <summary>
    /// 부가세 구분 (1:과세(별도), 2:영세, 3:면세, 0:VAT 포함)
    /// <summary>
    public int? vat_type { get; set; } = 0;

    /// <summary>
    /// 보증기간 일수
    /// <summary>
    public int? warranty_days { get; set; } = 0;

    /// <summary>
    /// 입금기한일 일수
    /// <summary>
    public int? deposit_due_days { get; set; } = 0;
    
    /// <summary>
    /// project pk
    /// <summary>
    public long r_project_id { get; set; }

    /// <summary>
    /// 프로젝트 타이틀
    /// <summary>
    public string pjt_title { get; set; }

    /// <summary>
    /// candidate pk
    /// <summary>
    public long? r_candidate_id { get; set; }

    /// <summary>
    /// 후보자 명
    /// <summary>
    public string candidate_name { get; set; }

    /// <summary>
    /// 실 연봉
    /// <summary>
    public decimal ann_income { get; set; }

    /// <summary>
    /// 실 연봉 금액 단위(KRW 등)
    /// <summary>
    public string income_currency_cd { get; set; }

    /// <summary>
    /// 수수료율
    /// <summary>
    public decimal? fee_rate { get; set; }
    /// <summary>
    /// 정액 여부 (정액 : 1, 비율 : 2)
    /// <summary>
    public int billing_type { get; set; } = 2;
    /// <summary>
    /// 공급가
    /// <summary>
    public decimal billing_amt { get; set; }
    /// <summary>
    /// 부가세
    /// <summary>
    public decimal billing_vat { get; set; }
    /// <summary>
    /// 빌링 총 금액
    /// <summary>
    public decimal billing_total { get; set; }
    /// <summary>
    /// 빌링 금액 원화 단위 (KRW 등)
    /// <summary>
    public string bill_currency_cd { get; set; }
    /// <summary>
    /// 공급가(원화 환산)
    /// <summary>
    public decimal billing_amt_won { get; set; }

    /// <summary>
    /// 부가세(원화 환산)
    /// <summary>
    public decimal billing_vat_won { get; set; }

    /// <summary>
    /// 빌링 총 금액 (원화 환산)
    /// <summary>
    public decimal billing_total_won { get; set; }

    /// <summary>
    /// 입사일
    /// <summary>
    public DateTime? join_dt { get; set; }

    /// <summary>
    /// 퇴사일
    /// <summary>
    public DateTime? leave_dt { get; set; }

    /// <summary>
    /// 보증기간 만료일
    /// <summary>
    public DateTime? expire_guarantee { get; set; }

    /// <summary>
    /// 빌링 날짜
    /// <summary>
    public DateTime? billing_dt { get; set; }

    /// <summary>
    /// 세금계산서 발행 희망일
    /// <summary>
    public DateTime? tax_req_dt { get; set; }

    /// <summary>
    /// 고객사 담당자 id
    /// <summary>
    public long r_client_contact_id { get; set; }

    /// <summary>
    /// 고객사 담당자 이름
    /// <summary>
    public string client_contact_name { get; set; }

    /// <summary>
    /// 고객사 담당자 이메일
    /// <summary>
    public string client_contact_email { get; set; }

    /// <summary>
    /// 고객사 담당자 전화번호
    /// <summary>
    public string client_contact_phone { get; set; }

    /// <summary>
    /// 고객사 계산서 담당자 id
    /// <summary>
    public long r_client_tax_id { get; set; }

    /// <summary>
    /// 고객사 계산서 담당자 이름
    /// <summary>
    public string client_tax_name { get; set; }

    /// <summary>
    /// 고객사 계산서 담당자 이메일
    /// <summary>
    public string client_tax_email { get; set; }

    /// <summary>
    /// 고객사 계산서 담당자 전화번호
    /// <summary>
    public string client_tax_phone { get; set; }

    /// <summary>
    /// 후보자 소스 ( 1: 유니비전 후보, 1: 링크드인, 2:리멤버, 3:직접지원, 9: 기타)
    /// </summary>
    public int? candidate_source { get; set; }
    /// <summary>
    /// 후보자 소스 문자열
    /// </summary>
    public string candidate_source_txt { get; set; }
    /// <summary>
    /// 후보자 최종직급 코드
    /// </summary>
    public int? candidate_position { get; set; }
    /// <summary>
    /// 후보자 최종직급 문자열
    /// </summary>
    public string candidate_position_txt { get; set; }

    //=========================================================

    /// <summary>
    /// 인보이스 발행 번호
    /// <summary>
    public string invoice_no { get; set; }

    /// <summary>
    /// 매출기준일 
    /// <summary>
    public DateTime? base_dt { get; set; }
    
    /// <summary>
    /// 승인일자
    /// <summary>
    public DateTime? confirm_dt { get; set; }

    /// <summary>
    /// 승인자
    /// <summary>
    public int? confirm_user { get; set; }

    /// <summary>
    /// 발송일자
    /// <summary>
    public DateTime? send_dt { get; set; }

    /// <summary>
    /// 발송자
    /// <summary>
    public int? send_user { get; set; }

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

    /// <summary>
    /// 인보이스 파일 생성여부(0: 미생성, 1:생성)
    /// <summary>
    public int? is_file { get; set; }

    /// <summary>
    /// 인보이스 파일 폴더 경로
    /// <summary>
    public string file_dir { get; set; }

    /// <summary>
    /// 인보이스 파일 원본 이름
    /// <summary>
    public string file_origin_path { get; set; }

    /// <summary>
    /// 인보이스 파일 이름
    /// <summary>
    public string file_path { get; set; }

    /// <summary>
    /// 인보이스 파일 확장자
    /// <summary>
    public string file_extension { get; set; }

    /// <summary>
    /// 인보이스 실입금 금액
    /// <summary>
    public decimal? deposit_amt { get; set; }

    /// <summary>
    /// 인보이스 실입금 일자
    /// <summary>
    public DateTime? deposit_dt { get; set; }

    /// <summary>
    /// 인보이스 파일 생성 일자
    /// <summary>
    public DateTime? file_dt { get; set; }

    /// <summary>
    /// 인보이스 파일 생성자
    /// <summary>
    public int? file_user { get; set; }

    /// <summary>
    /// PO No
    /// <summary>
    public string po_no { get; set; }

    /// <summary>
    /// 인보이스 삭제여부( 0: x, 1: 삭제)
    /// </summary>
    public int is_deleted { get; set; } = 0;

    /// <summary>
    /// 적용환율
    /// </summary>
    public double? ex_rate { get; set; }

    // --- 관계 선언 추가 (1:N 관계) ---
    public virtual ICollection<test_invoice_new_dtl> invoice_new_dtls { get; set; }
  }

}


