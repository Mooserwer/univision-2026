using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Remember;
using Univision.Core.Repositories;

namespace Univision.Remember.RestfulServer.Controllers
{
  [RoutePrefix("test/invoices/contingency")]
  public class InvoiceTestController : ApiController
  {
    // POST tasks
    [HttpPost]
    [Route("")]
    public async Task<IHttpActionResult> Post([FromBody] r_invoice_v2 model)
    {
      // 1. 요청 원본 정보 추출 (로그용)
      string rawJson = Newtonsoft.Json.JsonConvert.SerializeObject(model);
      string apiPath = Request.RequestUri.AbsolutePath;
      string clientIp = System.Web.HttpContext.Current.Request.UserHostAddress;
      int logSeq = 0;

      try
      {

        // 2. Repository 인스턴스 생성
        using (RememberTESTInvoiceEntityRepository rir = new RememberTESTInvoiceEntityRepository())
        {
          // 2. 초기 로그 기록 (시작 시점)
          logSeq = await rir.WriteApiLogAsync(model?.id, model?.request_user_id, apiPath, "POST", rawJson, clientIp);

          // 3. 유효성 검사
          if (model == null)
          {
            await rir.UpdateApiLogAsync(logSeq, -1, "전송된 데이터가 없습니다. (Payload is null)");
            return Json(new { result = -1, message = "전송된 데이터가 없습니다." });
          }


          if (model.status == "CANCELED" || model.status == "REFUNDED")
          {
            List<string> errorMessages_First = new List<string>();
            //[취소/환불]에 대해서는 체크 필요
            if (model.root_invoice_id.HasValue && model.root_invoice_id.Value > 0)
            {
              var root_invoice = await rir.CheckRootInvoiceAsync(model.root_invoice_id.Value);
              if (root_invoice.ResultCode == 0 || root_invoice.ResultCode == -1)
              {
                errorMessages_First.Add(root_invoice.Message);
                throw new ValidationException(errorMessages_First);
              }
            }
            else
            {
              errorMessages_First.Add("취소/환불 대상 인보이스ID 가 유효하지 않습니다.");
              throw new ValidationException(errorMessages_First);
            }
          }


          var entity = MapToEntityV2(model);

          // 4. 데이터 저장 및 정리 작업
          var resultObj = await rir.InsertInvoiceWithCleanupAsync(entity);

          // 5. 성공 로그 업데이트
          await rir.UpdateApiLogAsync(logSeq, resultObj.ResultCode, resultObj.Message);
          return Json(new
          {
            result = resultObj.ResultCode,
            message = resultObj.Message,
            id = entity.in_seq,
            debug = new
            {
              내용 = "[테스트용] 성공적으로 저장 되었습니다.\n아래는 내부 매핑 자료 데이터 입니다.\n보내신 내용이 정상적으로 잘 전달 되었는지 검토하실 수 있습니다.",
              인보이스_고유번호 = entity.r_invoice_id,
              인보이스_제목 = entity.invoice_title,
              인보이스_본문 = entity.invoice_contents,
              인보이스_언어 = entity.invoice_lang == 1 ? "영문" : "국문",
              세금구분 = entity.vat_type == 1 ? "과세(VAT 별도)" : (entity.vat_type == 2 ? "영세" : (entity.vat_type == 3 ? "면세" : "과세(VAT 포함)")),

              인보이스_타입 = entity.invoice_type == 0 ? "채용" :
                       entity.invoice_type == 1 ? "선수금" :
                       entity.invoice_type == 2 ? "잔금" :
                       entity.invoice_type == 3 ? "컨설팅" :
                       entity.invoice_type == 4 ? "환불" :
                       entity.invoice_type == 5 ? "취소" : "",

              인보이스_서브_타입 = entity.invoice_sub_type == 1 ? "채용" :
                       entity.invoice_sub_type == 2 ? "평판조회" :
                       entity.invoice_sub_type == 3 ? "재취업" :
                       entity.invoice_sub_type == 4 ? "채용대행" :
                       entity.invoice_sub_type == 5 ? "마켓매핑" :
                       entity.invoice_sub_type == 6 ? "사외이사추천" : "기타",

              대상_인보이스 = entity.pre_invoice_id.HasValue ? entity.pre_invoice_id.Value.ToString() : "(취소/환불에만 해당)",

              신청자명 = entity.r_request_user_name,
              신청자코드 = entity.r_request_user_id,
              신청자이메일 = entity.r_request_user_email,

              후보자명_표시_여부 = entity.is_open_name == 1 ? "표시함 (Yes)" : "표시안함 (No)",
              수수료율_표시_여부 = entity.is_open_annual_income == 1 ? "표시함 (Yes)" : "표시안함 (No)",
              PO넘버_포함_여부 = entity.is_po_no == 1 ? "표시함 (Yes)" : "표시안함 (No)",

              입금은행 = entity.deposit_bank_name,

              // --- 고객사 상세 정보 추가 ---
              고객사명 = entity.client_name,
              고객사코드 = entity.r_client_id,
              고객사대표 = entity.client_ceo ?? "-",
              사업자번호 = entity.client_biz_code ?? "-",
              고객사주소 = entity.client_addr1 ?? "-",
              기초조사비_퍼센트 = entity.client_basement_per.Value,
              // ----------------------------

              프로젝트명 = entity.pjt_title,
              프로젝트코드 = entity.r_project_id,
              후보자명 = entity.candidate_name,
              후보자코드 = entity.r_candidate_id,
              입사일자 = entity.join_dt?.ToString("yyyy-MM-dd") ?? "-",

              추천인정보출처 = entity.candidate_source_txt,
              후보자최종직급 = entity.candidate_position_txt,

              연봉 = entity.ann_income.ToString("N0") + entity.income_currency_cd,
              발행일자 = entity.tax_req_dt?.ToString("yyyy-MM-dd") ?? "-",
              발행금액_수정_여부 = entity.billing_type == 1 ? "정액(수동)" : "비율(자동)",
              수수료율 = (entity.fee_rate ?? 0).ToString("N2") + "%",

              공급_금액 = entity.billing_amt.ToString("N0"),
              세금_금액 = entity.billing_vat.ToString("N0"),
              발행_금액 = entity.billing_total.ToString("N0"),
              총_금액 = entity.billing_total.ToString("N0") + " " + entity.bill_currency_cd,
              보증일 = entity.warranty_days + "일",
              보증_만료일 = entity.expire_guarantee?.ToString("yyyy-MM-dd") ?? "-",
              입금기한 = entity.deposit_due_days + "일",

              고객사담당자명 = entity.client_contact_name,
              고객사담당자코드 = entity.r_client_contact_id,
              고객사담당자이메일 = entity.client_contact_email,
              고객사담당자전화 = entity.client_contact_phone,

              계산서담당자명 = entity.client_tax_name,
              계산서담당자코드 = entity.r_client_tax_id,
              계산서이메일 = entity.client_tax_email,
              계산서전화 = entity.client_tax_phone,

              매출기여도 = entity.invoice_new_dtls.Select(d => new
              {
                이름 = d.r_user_name,
                코드 = d.r_user_id,
                금액 = d.sales_money.ToString("N0"),
                배분율 = d.sales_rate.ToString("N2") + "%",
                역할 = d.comments
              })
            }
          });
        }
      }
      catch (ValidationException ex)
      {
        string combinedErrors = string.Join(" | ", ex.Errors);
        if (logSeq > 0)
        {
          using (var rir = new RememberTESTInvoiceEntityRepository())
          {
            await rir.UpdateApiLogAsync(logSeq, -1, "[Validation Fail] " + combinedErrors);
          }
        }
        return Json(new { result = -1, message = "데이터 검증 오류", errors = ex.Errors });
      }
      catch (Exception e)
      {
        string fieldErrorInfo = "";
        string innerMsg = "";

        // 1. Entity Framework 검증 오류 상세 분석 (EF 6.x 기준)
        if (e is System.Data.Entity.Validation.DbEntityValidationException ve)
        {
          var errorMessages = ve.EntityValidationErrors
              .SelectMany(x => x.ValidationErrors)
              .Select(x => $"[필드: {x.PropertyName}] 오류: {x.ErrorMessage}");

          fieldErrorInfo = string.Join(" / ", errorMessages);
          innerMsg = "데이터 검증 실패: " + fieldErrorInfo;
        }
        // 2. DB 업데이트 오류 (제약 조건, 타입 불일치 등)
        else if (e is System.Data.Entity.Infrastructure.DbUpdateException de)
        {
          var baseEx = de.GetBaseException();
          innerMsg = "DB 저장 오류: " + baseEx.Message;

          // 특정 필드 언급이 메시지에 포함된 경우 추출 시도
          if (baseEx.Message.Contains("column"))
          {
            innerMsg = $"[필드/데이터 타입 오류] {baseEx.Message}";
          }
        }
        // 3. 기타 일반 오류
        else
        {
          innerMsg = e.InnerException?.InnerException?.Message
                     ?? e.InnerException?.Message
                     ?? e.Message;
        }

        // 로그 기록 (상세 내역 포함)
        if (logSeq > 0)
        {
          using (var rir = new RememberTESTInvoiceEntityRepository())
          {
            await rir.UpdateApiLogAsync(logSeq, -1, "[Critical Error] " + innerMsg);
          }
        }

        // 클라이언트 응답 (에러가 난 지점을 명확히 전달)
        return Json(new
        {
          result = -1,
          message = innerMsg,
          detail = fieldErrorInfo // 필드 오류가 있을 경우 별도 제공
        });
      }
    }

    private test_invoice_new MapToEntityV2(r_invoice_v2 model)
    {

      List<string> errorMessages = new List<string>();
      List<string> warningMessages = new List<string>();

      // [1] 마스터 정보 검증
      if (model.id <= 0) errorMessages.Add("[인보이스 ID] 정보가 유효하지 않습니다.");

      //[취소/환불]에 대해서는 체크 안함
      if (model.status != "CANCELED" && model.status != "REFUNDED")
      {
        if (string.IsNullOrWhiteSpace(model.title)) errorMessages.Add("인보이스 제목 정보가 없습니다.");
        if (string.IsNullOrWhiteSpace(model.description)) errorMessages.Add("인보이스 본문 정보가 없습니다.");
      }

      if (model.user.id <= 0) errorMessages.Add("신청자 정보가 유효하지 않습니다.");
      if (string.IsNullOrWhiteSpace(model.user.name)) errorMessages.Add("신청자 명은 필수 항목입니다.");

      int language = 0;

      //[취소/환불]에 대해서는 체크 안함
      if (model.status != "CANCELED" && model.status != "REFUNDED")
      {
        if (model.language == "KR") language = 0;
        else if (model.language == "EN") language = 1;
        else
        {
          warningMessages.Add("[인보이스 언어] 가 유효하지 않아 **국문 인보이스로 Default 설정** 됩니다.");
        }
      }

      if (model.total_amount == 0) errorMessages.Add("[총 금액] 은 0일 수 없습니다.");

      if (model.supply_amount == 0) errorMessages.Add("[공급 금액] 은 0일 수 없습니다.");
      else
      {
        if ((model.supply_amount + model.tax_amount) != model.total_amount)
        {
          model.total_amount = model.supply_amount + model.tax_amount;
          warningMessages.Add($"[공급 금액 + 부가세] 와 [총 금액]와 달라 [공급 금액 + 부가세]기준으로 처리 됩니다. (변경 된 총 금액 : {model.total_amount:N0})");
        }


        if (model.apply_amount != model.total_amount)
        {
          model.apply_amount = model.total_amount;
          warningMessages.Add($"[발행 금액] 과 [총 금액] 이 달라 [발행 금액]기준으로 처리 됩니다. (변경 된 발행 금액 : {model.apply_amount:N0})");
        }

      }
      if (string.IsNullOrWhiteSpace(model.amount_currency))
      {
        errorMessages.Add("[발행금액 화폐 단위] 정보가 유효하지 않습니다.");
      }
      else if (!IsValidCurrency(model.amount_currency))
      {
        errorMessages.Add($"[발행금액 화폐 단위] 가 올바르지 않습니다: {model.amount_currency}");
      }

      int invoice_type = -1;
      int invoice_sub_type = -1;
      if (model.key_project == null) errorMessages.Add("[프로젝트 정보(Object)] 정보가 유효하지 않습니다.");
      else
      {
        if (model.key_project.id <= 0) errorMessages.Add("[프로젝트 ID] 정보가 유효하지 않습니다.");

        if (model.key_project.type == "RECRUITMENT")
        {

          if (model.key_project.category_sub_type == "RECRUITMENT")
          {
            invoice_type = 0;
            invoice_sub_type = 1;
          }
          else if (model.key_project.category_sub_type == "ADVANCED_PAYMENT")
          {
            invoice_type = 1;
            invoice_sub_type = 1;
          }
          else errorMessages.Add("[프로젝트 서브 타입] 정보가 유효하지 않습니다.");
        }
        else if (model.key_project.type == "CONSULTING")
        {

          //프로젝트 서브 타입<br>- RECRUITER: 채용대행<br>- ADVANCED_PAYMENT: 선수금<br>- RPO: 채용대행<br>- REPUTATION_CHECK: 평판조회
          //CASE a.pjt_type WHEN 1 THEN '채용' WHEN 2 THEN'평판조회' WHEN 3 THEN '재취업' WHEN 4 THEN '채용대행' WHEN 5 THEN '마켓 매핑' WHEN 6 THEN '사외이사추천' WHEN 9 THEN '기타' END As pjt_type_str, -- 종류명
          //Enum(RECRUITMENT, ADVANCED_PAYMENT, RPO, REPUTATION_CHECK, OUTPLACEMENT, LONGLIST, OUTSIDE_DIRECTOR, ETC)
          if (model.key_project.category_sub_type == "RECRUITMENT")
          {
            invoice_type = 3;
            invoice_sub_type = 4;
          }
          else if (model.key_project.category_sub_type == "RPO")
          {
            invoice_type = 3;
            invoice_sub_type = 4;
          }
          else if (model.key_project.category_sub_type == "REPUTATION_CHECK")
          {
            invoice_type = 3;
            invoice_sub_type = 2;
          }
          else if (model.key_project.category_sub_type == "OUTPLACEMENT")
          {
            invoice_type = 3;
            invoice_sub_type = 3;
          }
          else if (model.key_project.category_sub_type == "LONGLIST")
          {
            invoice_type = 3;
            invoice_sub_type = 5;
          }
          else if (model.key_project.category_sub_type == "OUTSIDE_DIRECTOR")
          {
            invoice_type = 3;
            invoice_sub_type = 6;
          }
          else if (model.key_project.category_sub_type == "ETC")
          {
            invoice_type = 3;
            invoice_sub_type = 9;
          }
          else if (model.key_project.category_sub_type == "ADVANCED_PAYMENT")
          {
            invoice_type = 1;
            invoice_sub_type = 1;
          }
          else errorMessages.Add("[프로젝트 서브 타입] 정보가 유효하지 않습니다.");
        }
        else errorMessages.Add("[프로젝트 타입] 정보가 유효하지 않습니다.");
      }

      //[취소/환불]에 대해서는 인보이스 타입 수정
      if (model.status == "CANCELED") invoice_type = 5; //취소
      else if (model.status == "REFUNDED") invoice_type = 4; //환불


      //채용 인보이스에 한하여 후보자 정보 체크 및 보증기간 처리
      DateTime? expire_warranty = null;
      if (model.key_project.category_sub_type == "RECRUITMENT")
      {
        if (model.key_project_candidate == null) errorMessages.Add("[후보자 정보(Object)] 정보가 유효하지 않습니다.");
        else
        {
          if (model.key_project_candidate.id <= 0) errorMessages.Add("[후보자 ID] 정보가 유효하지 않습니다.");

          //[취소/환불]에 대해서는 체크 안함
          if (string.IsNullOrWhiteSpace(model.key_project_candidate.name)) errorMessages.Add("[후보자 이름] 정보가 유효하지 않습니다.");
          if (model.key_project_candidate.salary <= 0) errorMessages.Add("[후보자 연봉] 정보가 유효하지 않습니다.");

          if (string.IsNullOrWhiteSpace(model.key_project_candidate.salary_currency))
          {
            errorMessages.Add("[연봉 화폐 단위] 정보가 유효하지 않습니다.");
          }
          else if (!IsValidCurrency(model.key_project_candidate.salary_currency))
          {
            errorMessages.Add($"[연봉 화폐 단위] 가 올바르지 않습니다: {model.key_project_candidate.salary_currency}");
          }

          if (!model.is_modify_apply_amount)
            if (model.key_project_candidate.commission_rate <= 0) errorMessages.Add("[수수료율] 정보가 유효하지 않습니다.");
          // [TO DO] 수수료율 * 연봉 이 공급금액과 동일한지 검증 필요

          if (string.IsNullOrWhiteSpace(model.key_project_candidate.reference)) errorMessages.Add("[후보자 - 추천인 정보 출처] 정보가 유효하지 않습니다.");
          if (string.IsNullOrWhiteSpace(model.key_project_candidate.job_rank)) errorMessages.Add("[후보자 - 최종직급] 정보가 유효하지 않습니다.");

          //최종직급 항목도 추가 예정임 
          if (!model.key_project_candidate.joining_date.HasValue) errorMessages.Add("[후보자 입사일] 정보가 유효하지 않습니다.");

          //[환불]에 대해서는 체크 필요
          if (model.status == "REFUNDED")
            if (!model.key_project_candidate.leaving_date.HasValue) errorMessages.Add("[후보자 퇴사일] 정보가 유효하지 않습니다.");

          if (model.key_project_candidate.joining_date.HasValue && model.warranty_days > 0)
          {
            // 보증 만료일 계산
            expire_warranty = model.key_project_candidate.joining_date.Value.AddDays(model.warranty_days);
          }
        }
      }
      else
      {
        //후보자 객체 
        if (model.key_project_candidate == null)
          model.key_project_candidate = new r_key_project_candidate_v2();
      }

      if (model.issued_date == DateTime.MinValue) errorMessages.Add("[발행일] 정보가 유효하지 않습니다.");

      if (model.account == null) errorMessages.Add("[고객사 정보(Object)] 정보가 유효하지 않습니다.");
      else
      {
        if (model.account.id <= 0) errorMessages.Add("[고객사 ID] 정보가 유효하지 않습니다");
        if (string.IsNullOrWhiteSpace(model.account.name)) errorMessages.Add("[고객사명] 정보가 유효하지 않습니다.");
      }

      if (model.invoice_contact == null) errorMessages.Add("[고객사 담당자 정보(Object)] 정보가 유효하지 않습니다.");
      else
      {
        if (model.invoice_contact.id <= 0) errorMessages.Add("[고객사 담당자 ID] 정보가 유효하지 않습니다");
        if (string.IsNullOrWhiteSpace(model.invoice_contact.name)) errorMessages.Add("[고객사 담당자명] 정보가 유효하지 않습니다.");
        if (string.IsNullOrWhiteSpace(model.invoice_contact.email)) errorMessages.Add("[고객사 담당자 이메일] 정보가 유효하지 않습니다.");
      }

      if (model.tax_contact == null) errorMessages.Add("[고객사 계산서담당 정보(Object)] 정보가 유효하지 않습니다.");
      else
      {
        if (model.tax_contact.id <= 0) errorMessages.Add("[고객사 계산서담당 ID] 정보가 유효하지 않습니다");
        if (string.IsNullOrWhiteSpace(model.tax_contact.name)) errorMessages.Add("[고객사 계산서담당자명] 정보가 유효하지 않습니다.");
        if (string.IsNullOrWhiteSpace(model.tax_contact.email)) errorMessages.Add("[고객사 계산서담당자 이메일] 정보가 유효하지 않습니다.");
      }

      // [3] 상세 내역 검증 (금액 및 기여도 합계)
      if (model.participants != null && model.participants.Any())
      {
        // 1. 금액 합계 검증
        decimal totalPartAmount = model.participants.Sum(x => x.contribution_amount);
        if (Math.Abs(model.supply_amount - totalPartAmount) > 1m)
        {
          errorMessages.Add($"[담당자별 배분금액 합계] ({totalPartAmount:N0})가 공급가 ({model.supply_amount:N0})와 일치하지 않습니다.");
        }

        // 2. 기여도(비율) 합계 검증 추가
        // contribution_rate가 float/double일 수 있으므로 decimal로 변환하여 합산
        decimal totalRate = model.participants.Sum(x => x.contribution_rate);

        // 부동소수점 오차를 고려하여 100과의 차이가 매우 미세한지 확인 //(정확히 100이어야 함)
        if (Math.Abs(100m - totalRate) > 0.001m)// 0.01% 오차 허용 으로 수정 필요
        {
          errorMessages.Add($"[참여자별 기여도 합계] 가 100%가 아닙니다. (현재 합계: {totalRate}%)");
        }
      }
      else
      {
        errorMessages.Add("[참여자 정보] 가 최소 1명 이상 필요합니다.");
      }

      string bank_account = "";
      string bank_name = "";
      //[취소/환불]에 대해서는 체크 안함
      if (model.status != "CANCELED" && model.status != "REFUNDED")
      {
        if (model.amount_currency != "KRW")
        {
          // 외화 전용 (고정)
          bank_name = "국민은행(외화전용)";
          bank_account = "389868-11-010539";
        }
        else
        {
          // 국내 통화 (KRW)
          bank_name = model.bank_name;
          switch (model.bank_name)
          {
            case "국민은행": bank_account = "389868-11-010539"; break;
            case "신한은행": bank_account = "100-008-022740"; break;
            case "우리은행": bank_account = "305-047998-13-001"; break;
            case "하나은행": bank_account = "249-890003-35004"; break;
            case "농협": bank_account = "317-0025-5995-61"; break;
            default:
              errorMessages.Add("입금은행 정보가 유효하지 않거나 누락되었습니다.");
              break;
          }
        }
      }

      int tax_type = -1;
      if (model.tax_type == "TAXABLE")
      {
        if (model.tax_include) tax_type = 0;
        else tax_type = 1;
      }
      else if (model.tax_type == "ZERO_RATED") tax_type = 2;
      else if (model.tax_type == "EXEMPT") tax_type = 3;
      else
      {
        errorMessages.Add("과세 구분 정보가 유효하지 않거나 누락되었습니다.");
      }

      // 채용환불에 대한 환불 시 생성 (작업 필요)
      string refund_string = String.Empty;
      if (model.key_project.category_sub_type == "RECRUITMENT" && model.status == "REFUNDED")
      {

        //직전인보이스 매출액, 보증일, 연봉 추출

        refund_string = $"---------- 아래는 자동으로 추가되는 환불 관련 정보입니다 ----------\n" +
                        $"- 후보자 : ${model.key_project_candidate.name}\n" +
                        $"- 입사일 : ${model.key_project_candidate.joining_date}\n" +
                        $"- 퇴사일 : ${model.key_project_candidate.leaving_date}\n" +
                        $"- 연봉  : ${model.key_project_candidate.salary} ${model.key_project_candidate.salary_currency}\n" +
                        $"- 용역비 : 33,000 USD\n" +
                        $"- 보증일 : 180 일(2026 - 09 - 01)\n" +
                        $"- 근무일 : 13 일\n" +
                        $"- 기초조사비 공제: Y(30 %)\n" +
                        $"- 계산식 : (용역비 - 기초조사비) * (보증일수 - 근무일수 / 보증일수)\n" +
                        $"(33, 000 - (33, 000 * 30 %)) * (180(보증일수) - 13(근무일) / 180(보증일수)) = 21,432";


      }

      // [5] 에러가 있으면 여기서 중단
      if (errorMessages.Count > 0)
      {
        throw new ValidationException(errorMessages);
      }

      var entity = new test_invoice_new
      {
        r_invoice_id = model.id,
        pre_invoice_id = model.root_invoice_id,
        invoice_type = invoice_type,
        invoice_sub_type = invoice_sub_type,
        invoice_lang = language,
        r_request_user_id = model.user.id,
        r_request_user_name = model.user.name,
        r_request_user_email = model.user.email,
        is_po_no = (model.is_po_number ? 1 : 0),
        is_open_name = (model.is_show_candidates_name ? 1 : 0),
        is_open_annual_income = (model.is_show_commission_rate ? 1 : 0),
        invoice_title = model.title,
        invoice_contents = model.description,
        deposit_bank_name = bank_name,
        deposit_bank_account = bank_account,
        remarks = model.remark,
        r_client_id = model.account.id,
        client_name = model.account.name,
        client_ceo = model.account.ceo_name,
        client_addr1 = model.account.address,
        client_biz_code = model.account.business_code,
        vat_type = tax_type,
        join_dt = model.key_project_candidate.joining_date,
        leave_dt = model.key_project_candidate.leaving_date,
        warranty_days = model.warranty_days,
        expire_guarantee = expire_warranty,
        deposit_due_days = model.deposit_due_days,
        r_client_contact_id = model.invoice_contact.id,
        client_contact_name = model.invoice_contact.name,
        client_contact_email = model.invoice_contact.email,
        client_contact_phone = model.invoice_contact.mobile_phone_number,
        r_client_tax_id = model.tax_contact.id,
        client_tax_name = model.tax_contact.name,
        client_tax_email = model.tax_contact.email,
        client_tax_phone = model.tax_contact.mobile_phone_number,
        client_basement_per = (decimal)model.account.base_research_deduction_rate,
        r_project_id = model.key_project.id,
        pjt_title = model.key_project.title,
        r_candidate_id = model.key_project_candidate.id,
        candidate_name = model.key_project_candidate.name,
        ann_income = model.key_project_candidate.salary,
        income_currency_cd = model.key_project_candidate.salary_currency,
        fee_rate = model.key_project_candidate.commission_rate,
        billing_type = model.is_modify_apply_amount ? 1 : 0,
        billing_amt = model.supply_amount,
        billing_vat = model.tax_amount,
        billing_total = model.total_amount,
        billing_amt_won = (model.amount_currency == "KRW" ? model.supply_amount : 0m),
        billing_vat_won = (model.amount_currency == "KRW" ? model.tax_amount : 0m),
        billing_total_won = (model.amount_currency == "KRW" ? model.total_amount : 0m),
        bill_currency_cd = model.amount_currency,
        billing_dt = model.issued_date,
        tax_req_dt = model.issued_date,
        base_dt = model.issued_date,
        candidate_source_txt = model.key_project_candidate.reference,
        candidate_position_txt = model.key_project_candidate.job_rank,
        is_file = 0,
        deposit_amt = 0,
        create_dt = DateTime.UtcNow.AddHours(9),
        is_deleted = 0
      };

      // 상세 내역 매핑
      if (model.participants != null)
      {
        foreach (var item in model.participants)
        {
          entity.invoice_new_dtls.Add(new test_invoice_new_dtl
          {
            r_user_id = item.id,
            r_user_name = item.name,
            r_user_email = item.email,
            sales_money = (decimal)item.contribution_amount,
            sales_rate = (decimal)item.contribution_rate,
            sales_won = (model.amount_currency == "KRW" ? (decimal)item.contribution_amount : 0m),
            comments = item.role,
            create_dt = entity.create_dt,
          });
        }
      }
      return entity;
    }

    public bool IsValidCurrency(string currencyCode)
    {
      return System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures)
          .Select(c => new System.Globalization.RegionInfo(c.Name))
          .Any(r => r.ISOCurrencySymbol.Equals(currencyCode, StringComparison.InvariantCultureIgnoreCase));
    }

    // GET invoices/12345
    [HttpGet]
    [Route("{invoice_id:long}")]
    public async Task<IHttpActionResult> GetDeletable(long invoice_id)
    {
      try
      {
        using (RememberTESTInvoiceEntityRepository rir = new RememberTESTInvoiceEntityRepository())
        {
          // 삭제 가능 여부 확인
          var check = await rir.CheckDeletableAsync(invoice_id);

          return Json(new
          {
            result = check.ResultCode,
            deletable = check.ResultCode == 1,
            message = check.Message
          });
        }
      }
      catch (Exception e)
      {
        return Json(new { result = -1, message = e.Message });
      }
    }

    public class ValidationException : Exception
    {
      public List<string> Errors { get; }
      public ValidationException(List<string> errors) : base("입력 데이터 유효성 검증 실패")
      {
        Errors = errors;
      }
    }
  }
}
