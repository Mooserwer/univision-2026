using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Remember;


namespace Univision.Core.Repositories
{
  public class RememberTESTInvoiceEntityRepository : BaseRepository
  {
    public RememberTESTInvoiceEntityRepository(UnivisionContext db) : base(db)
    {

    }
    public RememberTESTInvoiceEntityRepository()
    {

    }

    public async Task<TEST_InvoiceInsertResult> InsertInvoiceWithCleanupAsync(test_invoice_new entity)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            // 1. 기존 데이터 체크 (r_invoice_id 기준)
            // 아직 삭제되지 않았고(is_deleted=0), 동일한 리멤버 ID를 가진 항목 조회
            var existingInvoice = await db.test_invoice_news
                .FirstOrDefaultAsync(x => x.r_invoice_id == entity.r_invoice_id && x.is_deleted == 0);

            if (existingInvoice != null)
            {
              // 2. 승인 여부 확인 (confirm_dt가 있으면 승인된 것)
              if (existingInvoice.confirm_dt != null)
              {
                return new TEST_InvoiceInsertResult(-2, "이미 승인 완료된 인보이스는 수정하거나 재등록할 수 없습니다.", null);
              }

              // 3. 기존 미승인 데이터 삭제 처리 (Soft Delete)
              existingInvoice.is_deleted = 1;
              existingInvoice.modify_dt = DateTime.UtcNow.AddHours(9);
              existingInvoice.remarks += $" [System: 신규 등록에 의한 자동 삭제 - {DateTime.Now}]";

              db.Entry(existingInvoice).State = EntityState.Modified;
              // 상세 내역도 함께 처리하고 싶다면 (비즈니스 정책에 따라 선택)
              // _context.invoice_new_dtl.Where(d => d.in_seq == existingInvoice.in_seq)...
            }

            //db.Entry(entity).State = EntityState.Added;
            db.test_invoice_news.Add(entity);
            await db.SaveChangesAsync();

            tran.Commit();

            return new TEST_InvoiceInsertResult(1, "Success", entity);

          }
          catch (DbEntityValidationException dbEx)
          {
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
              foreach (var validationError in validationErrors.ValidationErrors)
              {
                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
              }
            }
            tran.Rollback();
            throw dbEx;


          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;
          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<InvoiceDeletableResult> CheckDeletableAsync(long r_invoice_id)
    {
      // 1. 데이터 존재 여부 확인 (is_deleted = 0 인 것 중)
      var invoice = await db.test_invoice_news
          .FirstOrDefaultAsync(x => x.r_invoice_id == r_invoice_id && x.is_deleted == 0);

      if (invoice == null)
      {
        return new InvoiceDeletableResult(0, "존재하지 않거나 이미 삭제된 인보이스입니다.", 0);
      }

      // 2. 승인 여부 확인 (confirm_dt가 있으면 삭제 불가)
      if (invoice.confirm_dt != null)
      {
        return new InvoiceDeletableResult(-2, "이미 승인된 인보이스는 삭제할 수 없습니다.", (int)invoice.in_seq);
      }

      return new InvoiceDeletableResult(1, "삭제 가능한 상태입니다.", (int)invoice.in_seq);
    }

    public async Task<InvoiceDeletableResult> CheckRootInvoiceAsync(long r_invoice_id)
    {
      // 1. 데이터 존재 여부 확인 (is_deleted = 0 인 것 중)
      var root_invoice = await db.test_invoice_news
          .FirstOrDefaultAsync(x => x.r_invoice_id == r_invoice_id && x.is_deleted == 0);

      if (root_invoice == null)
      {
        return new InvoiceDeletableResult(0, "존재하지 않거나 이미 삭제된 인보이스입니다.", 0);
      }

      var canceled_invoice = await db.test_invoice_news
          .FirstOrDefaultAsync(x => x.pre_invoice_id == r_invoice_id && x.is_deleted == 0);

      if (canceled_invoice != null)
      {
        if (canceled_invoice.confirm_dt != null)
        {
          return new InvoiceDeletableResult(-1, "이미 취소 / 환불 승인된 인보이스 입니다.", 0); 
        }
      }

      // 2. 승인 여부 확인 (confirm_dt가 있으면 삭제 불가)
      if (root_invoice.confirm_dt != null)
      {
        return new InvoiceDeletableResult(2, "(가능)취소/환불 가능.", (int)root_invoice.in_seq);
      }

      return new InvoiceDeletableResult(1, "삭제가 가능한 상태입니다.", (int)root_invoice.in_seq);
    }


    public async Task<int> WriteApiLogAsync(long? invoiceId, long? userId, string apiPath, string method, string body, string clientIp)
    {
      string sql = @"INSERT INTO log_invoice_new_api (
                        r_invoice_id, 
                        r_request_user_id, 
                        api_path, 
                        http_method, 
                        request_body, 
                        client_ip,  -- IP 컬럼 추가
                        create_dt
                    ) 
                   VALUES (
                        @r_invoice_id, 
                        @r_request_user_id, 
                        @api_path, 
                        @http_method, 
                        @request_body, 
                        @client_ip, 
                        GETDATE()
                    );
                   SELECT CAST(SCOPE_IDENTITY() AS INT);";

      return await db.Database.SqlQuery<int>(sql,
          new SqlParameter("@r_invoice_id", (object)invoiceId ?? 0),
          new SqlParameter("@r_request_user_id", (object)userId ?? 0),
          new SqlParameter("@api_path", (object)apiPath ?? ""),
          new SqlParameter("@http_method", (object)method ?? ""),
          new SqlParameter("@request_body", (object)body ?? ""),
          new SqlParameter("@client_ip", (object)clientIp ?? "") // IP 매핑
      ).SingleAsync();
    }

    // 결과 업데이트
    public async Task UpdateApiLogAsync(int logSeq, int resultCode, string message)
    {
      if (logSeq <= 0) return;

      string sql = "UPDATE log_invoice_new_api SET result_code = @code, result_message = @msg WHERE log_seq = @seq";
      await db.Database.ExecuteSqlCommandAsync(sql,
          new SqlParameter("@code", resultCode),
          new SqlParameter("@msg", message),
          new SqlParameter("@seq", logSeq)
      );
    }




  }
}
