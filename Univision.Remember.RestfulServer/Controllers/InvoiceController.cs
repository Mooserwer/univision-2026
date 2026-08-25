using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Remember;
using Univision.Core.Repositories;
using Univision.Remember.RestfulServer.Infrastructure.Mailing;

namespace Univision.Remember.RestfulServer.Controllers
{
  [RoutePrefix("invoices")]
  public class InvoiceController : ApiController
  {
    // POST tasks
    [HttpPost]
    [Route("")]
    public async Task<IHttpActionResult> Post([FromBody] r_invoice_v2 model)
    {
      // 1. 요청 원본 정보 추출 (로그용) — 정제된 model 이 아닌 원본 request body 를 그대로 기록
      //    (Web API 기본 버퍼링이라 [FromBody] 바인딩 이후에도 원본 본문 재읽기 가능)
      string rawJson;
      try
      {
        rawJson = Request.Content != null ? await Request.Content.ReadAsStringAsync() : "";
      }
      catch
      {
        // 원본 본문 읽기 실패 시에만 정제된 model 로 폴백
        rawJson = Newtonsoft.Json.JsonConvert.SerializeObject(model);
      }
      string apiPath = Request.RequestUri.AbsolutePath;
      string clientIp = System.Web.HttpContext.Current.Request.UserHostAddress;
      int logSeq = 0;

      try
      {

        // 2. Repository 인스턴스 생성
        using (RememberInvoiceEntityRepository rir = new RememberInvoiceEntityRepository())
        {
          // 2. 초기 로그 기록 (시작 시점)
          logSeq = await rir.WriteApiLogAsync(model?.id, model?.request_user_id, apiPath, "POST", rawJson, clientIp);

          // 3. 유효성 검사
          if (model == null)
          {
            await rir.UpdateApiLogAsync(logSeq, -1, "전송된 데이터가 없습니다. (Payload is null)");
            return Json(new { result = -1, message = "전송된 데이터가 없습니다." });
          }

          /*
           * 취소 환불 건 임시로 수신 받을 수 있도록 허용(2027년 이후에는 막아도 되지 않을까 싶음)
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
          */


          var entity = MapToEntityV2(model);

          // 3-1. 환불(채용) 인보이스: 직전 인보이스 용역비 기준 환불 산식을 remarks 에 자동 추가
          if (model.status == "REFUNDED" && model.key_project != null && model.key_project.category_sub_type == "RECRUITMENT")
          {
            string refundInfo = await BuildRefundRemarksAsync(rir, entity);
            if (!string.IsNullOrEmpty(refundInfo))
              entity.remarks = string.IsNullOrEmpty(entity.remarks) ? refundInfo : (entity.remarks + "\n" + refundInfo);
          }

          // 4. 데이터 저장 및 정리 작업
          var resultObj = await rir.InsertInvoiceWithCleanupAsync(entity);

          // 5. 성공 로그 업데이트
          await rir.UpdateApiLogAsync(logSeq, resultObj.ResultCode, resultObj.Message);

          // 6. 인보이스 정상 수신 시 발행 알림 메일 발송 (실패해도 수신/저장 처리에는 영향 없음)
          //    [매핑필요/확인] 성공 판단 코드(ResultCode == 1) 값이 맞는지 확인 필요.
          if (resultObj.ResultCode == 1)
          {
            SendInvoiceReceivedMail(entity);
          }

          return Json(new
          {
            result = resultObj.ResultCode,
            message = resultObj.Message,
            id = entity.in_seq
          });
        }
      }
      catch (ValidationException ex)
      {
        string combinedErrors = string.Join(" | ", ex.Errors);
        if (logSeq > 0)
        {
          using (var rir = new RememberInvoiceEntityRepository())
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
          using (var rir = new RememberInvoiceEntityRepository())
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

    private invoice_new MapToEntityV2(r_invoice_v2 model)
    {

      List<string> errorMessages = new List<string>();
      List<string> warningMessages = new List<string>();
      string title = String.Empty;
      string description = String.Empty;

      // [1] 마스터 정보 검증
      if (model.id <= 0) errorMessages.Add("[인보이스 ID] 정보가 유효하지 않습니다.");

      //[취소/환불]에 대해서는 체크 안함
      if (model.status != "CANCELED" && model.status != "REFUNDED")
      {
        if (string.IsNullOrWhiteSpace(model.title)) errorMessages.Add("인보이스 제목 정보가 없습니다.");
        if (string.IsNullOrWhiteSpace(model.description)) errorMessages.Add("인보이스 본문 정보가 없습니다.");
        title = model.title;
        description = model.description;
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

      //0원 인보이스 허용예정
      //if (model.total_amount == 0) errorMessages.Add("[총 금액] 은 0일 수 없습니다.");
      //0원 인보이스 허용예정
      //if (model.supply_amount == 0) errorMessages.Add("[공급 금액] 은 0일 수 없습니다.");
      //else
      //{
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

      //}
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
        if (Math.Abs(100m - totalRate) > 0.01m)// 0.1% 오차 허용 으로 수정 필요
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
            case "국민은행": bank_account = "389801-01-140852"; break;
            case "국민은행(외화전용)": bank_account = "389868-11-010539"; break;
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
        title = title.Replace("인재추천 서비스 수수료", "채용컨설팅 용역비");
        //
        if (model.tax_include) tax_type = 0;
        else tax_type = 1;
      }
      else if (model.tax_type == "ZERO_RATED") tax_type = 2;
      else if (model.tax_type == "EXEMPT")
      {
        tax_type = 3;
        description = description.Replace("관한 용역비를 청구", "관한 서비스 용역수수료를 청구");
      }
      else
      {
        errorMessages.Add("과세 구분 정보가 유효하지 않거나 누락되었습니다.");
      }

      // 환불(채용) 인보이스의 remarks 자동 산식은 Post() 에서 직전 인보이스 조회 후 처리
      // (BuildRefundRemarksAsync — DB 비동기 조회 필요하므로 이 동기 매핑 메서드에서 제외)

      // [5] 에러가 있으면 여기서 중단
      if (errorMessages.Count > 0)
      {
        throw new ValidationException(errorMessages);
      }

      var entity = new invoice_new
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
        is_open_name = (model.is_show_candidate_name ? 1 : 0),
        is_open_annual_income = (model.is_show_commission_rate ? 1 : 0),
        invoice_title = title, //model.title,
        invoice_contents = description, //model.description,
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
          entity.invoice_new_dtls.Add(new invoice_new_dtl
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

    // ─────────────────────────────────────────────────────────
    //  환불(채용) 인보이스 remarks 자동 산식 문자열 생성.
    //  직전(원본) 인보이스(pre_invoice_id = model.root_invoice_id)의 용역비(billing_amt)·금액단위(bill_currency_cd)를
    //  조회해 Main invoice-refund-create.js 와 동일한 환불 계산식을 구성한다.
    //  원본 인보이스 조회가 안되면 '연봉' 이하 내용 없이 (후보자/입사일/퇴사일까지만) 반환.
    // ─────────────────────────────────────────────────────────
    private async Task<string> BuildRefundRemarksAsync(RememberInvoiceEntityRepository rir, invoice_new entity)
    {
      string joinStr = entity.join_dt.HasValue ? entity.join_dt.Value.ToString("yyyy-MM-dd") : "";
      string leaveStr = entity.leave_dt.HasValue ? entity.leave_dt.Value.ToString("yyyy-MM-dd") : "";

      string result =
          "---------- 아래는 자동으로 추가되는 환불 관련 정보입니다 ----------\n"
        + "- 후보자 : " + entity.candidate_name + "\n"
        + "- 입사일 : " + joinStr + "\n"
        + "- 퇴사일 : " + leaveStr;

      // 직전(원본) 인보이스 조회 — 용역비/금액단위 확보
      invoice_new origin = null;
      if (entity.pre_invoice_id.HasValue && entity.pre_invoice_id.Value > 0)
        origin = await rir.SelectOriginInvoiceAsync(entity.pre_invoice_id.Value);

      // 원본 인보이스가 없으면 연봉 이하 내용 없이 진행
      if (origin == null)
        return result;

      decimal serviceFee = origin.billing_amt;      // 용역비 (원본 발행 공급가 BILLING_AMT)
      string feeCurrency = origin.bill_currency_cd;  // 금액단위 BILL_CURRENCY_CD

      // 보증일수 / 근무일수 (Main: grt_day = 만료일-입사일, work_day = 퇴사일-입사일 + 1)
      int grtDay = (entity.expire_guarantee.HasValue && entity.join_dt.HasValue)
          ? (int)(entity.expire_guarantee.Value.Date - entity.join_dt.Value.Date).TotalDays : 0;
      int workDay = (entity.leave_dt.HasValue && entity.join_dt.HasValue)
          ? (int)(entity.leave_dt.Value.Date - entity.join_dt.Value.Date).TotalDays + 1 : 0;

      // 기초조사비 공제 (client_basement_per 는 % 값. base = 용역비 * rate * 0.01)
      decimal baseRate = entity.client_basement_per ?? 0m;
      bool hasBaseDeduction = baseRate > 0m;
      decimal baseAmt = hasBaseDeduction ? serviceFee * (baseRate * 0.01m) : 0m;

      string expireStr = entity.expire_guarantee.HasValue ? entity.expire_guarantee.Value.ToString("yyyy-MM-dd") : "";

      result +=
          "\n- 연봉 : " + entity.ann_income.ToString("N0") + " " + entity.income_currency_cd
        + "\n- 용역비 : " + serviceFee.ToString("N0") + " " + feeCurrency
        + "\n- 보증일 : " + grtDay + " 일 (" + expireStr + ")"
        + "\n- 근무일 : " + workDay + " 일"
        + "\n- 기초조사비 공제 : " + (hasBaseDeduction ? "Y (" + baseRate + " %)" : "N")
        + "\n- 계산식 : (용역비 - 기초조사비) * (보증일수 - 근무일수 / 보증일수)\n";

      // 계산 문자열 (Main invoice-refund-create.js 와 동일 형식)
      if (grtDay > 0 && workDay > 0 && grtDay > workDay)
      {
        decimal refundAmt = Math.Round((serviceFee - baseAmt) * ((grtDay - workDay) / (decimal)grtDay), MidpointRounding.AwayFromZero);
        string basePart = hasBaseDeduction
            ? "(" + serviceFee.ToString("N0") + " * " + baseRate + "%))"
            : "(0))";
        result += "(" + serviceFee.ToString("N0") + " - " + basePart + " * "
                + "(" + grtDay + "(보증일수) - " + workDay + "(근무일) / " + grtDay + "(보증일수))"
                + " = " + refundAmt.ToString("N0");
      }
      else
      {
        result += "(계산 불가: 보증일수/근무일수 정보를 확인해 주세요.)";
      }

      return result;
    }

    public bool IsValidCurrency(string currencyCode)
    {
      return System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures)
          .Select(c => new System.Globalization.RegionInfo(c.Name))
          .Any(r => r.ISOCurrencySymbol.Equals(currencyCode, StringComparison.InvariantCultureIgnoreCase));
    }

    // ─────────────────────────────────────────────────────────
    //  인보이스 수신 시 발행 알림 메일 발송.
    //  Univision.Main InvoiceController.InvoiceContengencySubmit 의 메일 발송 프로세스와 동일하게 구성.
    //  동일한 템플릿(NewInvoiceCreateTemplete) 사용. 매핑이 명확한 항목만 채우고,
    //  애매한 항목은 [매핑필요] 주석으로 남겨둠 — 직접 채워 넣을 것.
    //  메일 발송 실패가 수신/저장 처리에 영향 주지 않도록 내부에서 예외를 삼킴.
    // ─────────────────────────────────────────────────────────
    private void SendInvoiceReceivedMail(invoice_new entity)
    {
      try
      {
        // 인보이스 종류 표기 (Main invoice_type_str 대응)
        string invoice_type_str;
        switch (entity.invoice_type)
        {
          case 0: invoice_type_str = "성공 인보이스"; break;
          case 1: invoice_type_str = "선수금 인보이스"; break;
          case 2: invoice_type_str = "잔금 인보이스"; break;
          case 3: invoice_type_str = "컨설팅 인보이스"; break;
          case 4: invoice_type_str = "환불 인보이스"; break;
          case 5: invoice_type_str = "취소 인보이스"; break;
          default: invoice_type_str = "인보이스"; break;
        }

        // VAT 구분 표기 (Main 은 Utils.ReturnVatTypeTxt 사용 — RestfulServer 엔 Utils 가 없어 인라인 매핑)
        // [매핑필요] 실제 vat_type 코드 → 표기 매핑이 맞는지 확인 필요.
        string vat_type_str;
        switch (entity.vat_type ?? -1)
        {
          case 0: vat_type_str = "과세(포함)"; break;
          case 1: vat_type_str = "과세(별도)"; break;
          case 2: vat_type_str = "영세율"; break;
          case 3: vat_type_str = "면세"; break;
          default: vat_type_str = ""; break;
        }

        // 후보자 정보 테이블 (채용 건에 한함). Main candidate_info_table 과 동일 레이아웃.
        string candidate_info_table = "";
        if (entity.r_candidate_id > 0 && !string.IsNullOrWhiteSpace(entity.candidate_name))
        {
          candidate_info_table = @"
<tr>
  <th rowspan='4' style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important; background-color:#007bff; color:#fff'>후보자 정보</th>
  <th style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important; background-color:#007bff; color:#fff'>이름</th>
  <td style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important'>" + entity.candidate_name + @"</td>
  <th style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important; background-color:#007bff; color:#fff'>최종직급</th>
  <td style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important'>" + entity.candidate_position_txt + @"</td>
</tr>
<tr>
  <th style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important; background-color:#007bff; color:#fff'>입사일</th>
  <td style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important'>" + (entity.join_dt.HasValue ? entity.join_dt.Value.ToString("yyyy-MM-dd") : "") + @"</td>
  <th style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important; background-color:#007bff; color:#fff'>후보자소스</th>
  <td style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important'>" + entity.candidate_source_txt + @"</td>
</tr>
<tr>
  <th style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important; background-color:#007bff; color:#fff'>연봉</th>
  <td colspan='3' style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important'>" + entity.ann_income.ToString("N0") + " [" + entity.income_currency_cd + @"]</td>
</tr>
<tr>
  <th style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important; background-color:#007bff; color:#fff'>후보자명 표시</th>
  <td style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important'>" + (entity.is_open_name == 1 ? "표시" : "숨김") + @"</td>
  <th style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important; background-color:#007bff; color:#fff'>연봉/수수료율 표시</th>
  <td style='padding:5px; vertical-align:top; border:1px solid #dee2e6 !important'>" + (entity.is_open_annual_income == 1 ? "표시" : "숨김") + @"</td>
</tr>
<tr><td colspan='5' style='padding:1px; border:1px solid #dee2e6 !important'>&nbsp;</td></tr>
";
        }

        // Fee sharing 정보 (참여자별 배분) — Main share_str 대응. entity.invoice_new_dtls 사용.
        string feeshare = "";
        if (entity.invoice_new_dtls != null)
        {
          foreach (var d in entity.invoice_new_dtls)
          {
            feeshare += (!string.IsNullOrEmpty(feeshare) ? "<br/><br/>" : "")
              + " - " + d.r_user_name + " ( " + d.sales_rate + "% / " + d.sales_money.ToString("N0") + " ) : " + d.comments;
          }
        }

        // 수수료율 표기
        string feerate_str = (entity.billing_type == 1) ? "정액" : (entity.fee_rate.GetValueOrDefault().ToString() + "%");

        // 수신자 목록
        // [매핑필요] Main 은 프로젝트 AM/SM/매출대상자 + unico@ 로 발송. API 수신 맥락엔 그 정보가 없어
        //           일단 (발행요청자 + 매출대상자 + unico@) 로 구성함. 실제 수신 정책에 맞게 조정할 것.
        var toList = new List<string>();
        //if (!string.IsNullOrWhiteSpace(entity.r_request_user_email)) toList.Add(entity.r_request_user_email);
        //if (entity.invoice_new_dtls != null)
        //  foreach (var d in entity.invoice_new_dtls)
        //    if (!string.IsNullOrWhiteSpace(d.r_user_email) && !toList.Contains(d.r_user_email)) toList.Add(d.r_user_email);
        toList.Add("unico@unicosearch.com");
        

        var mailData = new NewInvoiceCreateDto
        {
          ToArr = toList.ToArray(),
          // 발신자: 발행요청자
          From = (!string.IsNullOrWhiteSpace(entity.r_request_user_email))
                   ? new System.Net.Mail.MailAddress(entity.r_request_user_email, string.IsNullOrWhiteSpace(entity.r_request_user_name) ? entity.r_request_user_email : entity.r_request_user_name)
                   : null,
          name = entity.r_request_user_name,
          // [매핑필요] Main 은 메일제목에 프로젝트명 사용 — 여기선 프로젝트 제목(pjt_title)으로 대체.
          title = entity.pjt_title,
          invoicetype = invoice_type_str,
          billingdt = entity.create_dt.HasValue ? entity.create_dt.Value.ToString("yyyy-MM-dd") : "",
          taxreqdt = entity.tax_req_dt.HasValue ? entity.tax_req_dt.Value.ToString("yyyy-MM-dd") : "",
          comment = string.IsNullOrEmpty(entity.remarks) ? "[없음]" : entity.remarks.Replace("\r\n", "\n").Replace("\n", "<br />"),
          language = entity.invoice_lang == 0 ? "국문" : "영문",
          pono = entity.is_po_no == 1 ? "<span style='color:red'>필요</span>" : "불필요",
          candidateinfo = candidate_info_table,
          vattype = "[" + vat_type_str + "]",
          feerate = feerate_str,
          billingamt = entity.billing_amt.ToString("N0"),
          // [매핑필요] 공제금액(선수금) 대응 필드 없음. 필요 시 채울 것. (일단 미사용)
          retaineramt = "",
          currency = entity.bill_currency_cd,
          fee = entity.billing_total.ToString("N0"),
          amt = entity.billing_amt.ToString("N0"),
          vat = entity.billing_vat.ToString("N0"),
          clientname = entity.client_name,
          ceo = entity.client_ceo,
          address = entity.client_addr1,
          bizcode = entity.client_biz_code,
          contactname = entity.client_contact_name,
          contactemail = entity.client_contact_email,
          contactphone = entity.client_contact_phone,
          etaxname = entity.client_tax_name,
          etaxmail = entity.client_tax_email,
          etaxphone = entity.client_tax_phone,
          invoicetitle = entity.invoice_title,
          invoicecontents = string.IsNullOrEmpty(entity.invoice_contents) ? "" : entity.invoice_contents.Replace("\r\n", "\n").Replace("\n", "<br />"),
          bankaccount = entity.deposit_bank_account,
          bankname = entity.deposit_bank_name,
          feeshare = feeshare,
          // [매핑필요] Univision 프로젝트 상세 URL. entity.r_project_id 는 Remember 프로젝트 ID 라 내부 p_seq 와 다름. 실제 링크 규칙으로 교체할 것.
          pjturl = "https://headhunting-pro.rememberapp.co.kr/key-projects/"+entity.r_project_id.ToString()+ "/candidates",
          invurl = "https://headhunting-pro.rememberapp.co.kr/invoice/" + entity.r_invoice_id.ToString(),
        };

        // 발신 계정: 발행요청자 이메일 (픽업 배달이라 비밀번호는 사용 안 함)
        var mService = new MailService(entity.r_request_user_email);
        mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());

        // [매핑필요] Main 은 담당자(narae@, jhkim@)에게 별도 발송함. 필요 시 아래 주석 해제 후 수신자 확정.
        mailData.ToArr = new[] { "narae@unicosearch.com", "jhkim@unicosearch.com" };
        mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());
      }
      catch
      {
        // 메일 발송 실패는 인보이스 수신 처리에 영향 주지 않도록 무시 (필요 시 로깅 추가).
      }
    }

    // GET invoices/12345
    [HttpGet]
    [Route("{invoice_id:long}")]
    public async Task<IHttpActionResult> GetDeletable(long invoice_id)
    {
      try
      {
        using (RememberInvoiceEntityRepository rir = new RememberInvoiceEntityRepository())
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
