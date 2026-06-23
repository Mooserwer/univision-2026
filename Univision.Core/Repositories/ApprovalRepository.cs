using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;

namespace Univision.Core.Repositories
{
  // 전자결재(Appr) 데이터 접근
  public class ApprovalRepository : BaseRepository
  {
    private const string DOC_COLS_LIST = @"
SELECT d.*,
       (SELECT COUNT(*) FROM APPR_FILE f WHERE f.ad_seq = d.ad_seq) AS file_count,
       (SELECT TOP 1 l.approver_name FROM APPR_LINE l WHERE l.ad_seq = d.ad_seq AND l.order_no = d.cur_order AND l.line_type = 0) AS cur_approver_name
FROM APPR_DOC d ";

    // ── 기안함: 내가 기안한 문서 (status<0 이면 전체) ──────────────
    public async Task<List<appr_doc>> SelectWorkListAsync(int drafterSeq, int status)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        string q = DOC_COLS_LIST + " WHERE d.is_deleted = 0 AND d.drafter_seq = @drafterSeq ";
        if (status >= 0) q += " AND d.doc_status = @status ";
        q += " ORDER BY d.reg_date DESC ";
        var ret = await con.QueryAsync<appr_doc>(q, new { drafterSeq, status });
        return ret.ToList();
      }
    }

    // ── 결재함: tab 0 결재대기(내 차례), 1 결재완료(내가 처리함), 2 참조 ──
    public async Task<List<appr_doc>> SelectApprovalListAsync(int approverSeq, int tab)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        string q;
        if (tab == 2)
        {
          q = DOC_COLS_LIST + @"
WHERE d.is_deleted = 0 AND d.doc_status >= 1
  AND EXISTS (SELECT 1 FROM APPR_LINE l WHERE l.ad_seq = d.ad_seq AND l.approver_seq = @approverSeq AND l.line_type = 1)
ORDER BY d.submit_date DESC ";
        }
        else if (tab == 1)
        {
          q = DOC_COLS_LIST + @"
WHERE d.is_deleted = 0
  AND EXISTS (SELECT 1 FROM APPR_LINE l WHERE l.ad_seq = d.ad_seq AND l.approver_seq = @approverSeq AND l.line_type = 0 AND l.line_status <> 0)
ORDER BY d.reg_date DESC ";
        }
        else
        {
          q = DOC_COLS_LIST + @"
WHERE d.is_deleted = 0 AND d.doc_status = 1
  AND EXISTS (SELECT 1 FROM APPR_LINE l WHERE l.ad_seq = d.ad_seq AND l.approver_seq = @approverSeq AND l.line_type = 0 AND l.order_no = d.cur_order AND l.line_status = 0)
ORDER BY d.submit_date DESC ";
        }
        var ret = await con.QueryAsync<appr_doc>(q, new { approverSeq });
        return ret.ToList();
      }
    }

    // ── 결재대기 건수(뱃지용) ──────────────────────────────────────
    public async Task<int> CountPendingApprovalAsync(int approverSeq)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        string q = @"
SELECT COUNT(*) FROM APPR_DOC d
WHERE d.is_deleted = 0 AND d.doc_status = 1
  AND EXISTS (SELECT 1 FROM APPR_LINE l WHERE l.ad_seq = d.ad_seq AND l.approver_seq = @approverSeq AND l.line_type = 0 AND l.order_no = d.cur_order AND l.line_status = 0)";
        return await con.ExecuteScalarAsync<int>(q, new { approverSeq });
      }
    }

    // ── 문서 단건 + 결재선 + 첨부 + 댓글 ───────────────────────────
    public async Task<ApprDocViewModel> SelectDocAsync(int ad_seq)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        con.Open();
        var vm = new ApprDocViewModel();
        vm.doc = await con.QueryFirstOrDefaultAsync<appr_doc>(
          @"SELECT d.*,
                   (SELECT ud_name FROM UV_USER u WHERE u.uv_seq = d.drafter_seq) AS drafter_ud_name
            FROM APPR_DOC d WHERE d.ad_seq = @ad_seq AND d.is_deleted = 0", new { ad_seq });
        if (vm.doc == null) return null;

        vm.lines = (await con.QueryAsync<appr_line>(
          "SELECT * FROM APPR_LINE WHERE ad_seq = @ad_seq ORDER BY order_no", new { ad_seq })).ToList();
        vm.files = (await con.QueryAsync<appr_file>(
          "SELECT * FROM APPR_FILE WHERE ad_seq = @ad_seq ORDER BY af_seq", new { ad_seq })).ToList();
        vm.comments = (await con.QueryAsync<appr_comment>(
          "SELECT * FROM APPR_COMMENT WHERE ad_seq = @ad_seq AND is_deleted = 0 ORDER BY ac_seq", new { ad_seq })).ToList();
        return vm;
      }
    }

    public async Task<appr_doc> SelectDocOnlyAsync(int ad_seq)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        return await con.QueryFirstOrDefaultAsync<appr_doc>(
          "SELECT * FROM APPR_DOC WHERE ad_seq = @ad_seq AND is_deleted = 0", new { ad_seq });
    }

    // ── 문서 생성 → ad_seq 반환 ────────────────────────────────────
    public async Task<int> CreateDocAsync(appr_doc d)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        string q = @"
INSERT INTO APPR_DOC (title, content, drafter_seq, drafter_name, drafter_ud_seq, doc_status, cur_order, copy_from, reg_date)
VALUES (@title, @content, @drafter_seq, @drafter_name, @drafter_ud_seq, @doc_status, @cur_order, @copy_from, GETDATE());
SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await con.ExecuteScalarAsync<int>(q, d);
      }
    }

    // ── 문서 본문 수정(임시/회수 상태 편집) ────────────────────────
    public async Task UpdateDocContentAsync(appr_doc d)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        await con.ExecuteAsync(
          "UPDATE APPR_DOC SET title=@title, content=@content, mod_date=GETDATE() WHERE ad_seq=@ad_seq",
          new { d.title, d.content, d.ad_seq });
    }

    // ── 결재선 전체 교체 ───────────────────────────────────────────
    public async Task ReplaceLinesAsync(int ad_seq, List<appr_line> lines)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        con.Open();
        using (var tx = con.BeginTransaction())
        {
          await con.ExecuteAsync("DELETE FROM APPR_LINE WHERE ad_seq=@ad_seq", new { ad_seq }, tx);
          foreach (var l in lines)
          {
            await con.ExecuteAsync(@"
INSERT INTO APPR_LINE (ad_seq, order_no, approver_seq, approver_name, approver_position, line_type, line_status)
VALUES (@ad_seq, @order_no, @approver_seq, @approver_name, @approver_position, @line_type, 0)",
              new { ad_seq, l.order_no, l.approver_seq, l.approver_name, l.approver_position, l.line_type }, tx);
          }
          tx.Commit();
        }
      }
    }

    // ── 상신: 진행중(1) + cur_order=1, 결재선 대기 초기화 ──────────
    public async Task SubmitAsync(int ad_seq)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        con.Open();
        using (var tx = con.BeginTransaction())
        {
          await con.ExecuteAsync(
            "UPDATE APPR_LINE SET line_status=0, process_date=NULL, opinion=NULL WHERE ad_seq=@ad_seq", new { ad_seq }, tx);
          await con.ExecuteAsync(
            "UPDATE APPR_DOC SET doc_status=1, cur_order=1, submit_date=GETDATE(), complete_date=NULL, mod_date=GETDATE() WHERE ad_seq=@ad_seq", new { ad_seq }, tx);
          tx.Commit();
        }
      }
    }

    // ── 회수: 진행중(1) → 회수(4), 결재선 초기화 ───────────────────
    public async Task<bool> RecallAsync(int ad_seq, int drafterSeq)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        con.Open();
        var doc = await con.QueryFirstOrDefaultAsync<appr_doc>(
          "SELECT * FROM APPR_DOC WHERE ad_seq=@ad_seq AND is_deleted=0", new { ad_seq });
        if (doc == null || doc.drafter_seq != drafterSeq || doc.doc_status != 1) return false;

        using (var tx = con.BeginTransaction())
        {
          await con.ExecuteAsync("UPDATE APPR_LINE SET line_status=0, process_date=NULL, opinion=NULL WHERE ad_seq=@ad_seq", new { ad_seq }, tx);
          await con.ExecuteAsync("UPDATE APPR_DOC SET doc_status=4, cur_order=0, mod_date=GETDATE() WHERE ad_seq=@ad_seq", new { ad_seq }, tx);
          tx.Commit();
        }
        return true;
      }
    }

    // ── 결재 처리(승인/반려). 결과: -1 권한없음/상태오류, 1 처리됨 ──
    public async Task<int> ProcessApprovalAsync(int ad_seq, int approverSeq, bool approve, string opinion)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        con.Open();
        var doc = await con.QueryFirstOrDefaultAsync<appr_doc>(
          "SELECT * FROM APPR_DOC WHERE ad_seq=@ad_seq AND is_deleted=0", new { ad_seq });
        if (doc == null || doc.doc_status != 1) return -1;

        var apprLines = (await con.QueryAsync<appr_line>(
          "SELECT * FROM APPR_LINE WHERE ad_seq=@ad_seq AND line_type=0 ORDER BY order_no", new { ad_seq })).ToList();
        var cur = apprLines.FirstOrDefault(x => x.order_no == doc.cur_order);
        if (cur == null || cur.approver_seq != approverSeq || cur.line_status != 0) return -1;

        int maxOrder = apprLines.Count == 0 ? 0 : apprLines.Max(x => x.order_no);

        using (var tx = con.BeginTransaction())
        {
          await con.ExecuteAsync(
            "UPDATE APPR_LINE SET line_status=@st, process_date=GETDATE(), opinion=@opinion WHERE al_seq=@al_seq",
            new { st = approve ? 1 : 2, opinion, cur.al_seq }, tx);

          if (!approve)
          {
            await con.ExecuteAsync("UPDATE APPR_DOC SET doc_status=3, complete_date=GETDATE() WHERE ad_seq=@ad_seq", new { ad_seq }, tx);
          }
          else if (doc.cur_order >= maxOrder)
          {
            await con.ExecuteAsync("UPDATE APPR_DOC SET doc_status=2, complete_date=GETDATE() WHERE ad_seq=@ad_seq", new { ad_seq }, tx);
          }
          else
          {
            await con.ExecuteAsync("UPDATE APPR_DOC SET cur_order = cur_order + 1 WHERE ad_seq=@ad_seq", new { ad_seq }, tx);
          }
          tx.Commit();
        }
        return 1;
      }
    }

    // ── 삭제(소프트): 본인 + 임시(0)/회수(4)/반려(3) 만 ────────────
    public async Task<bool> DeleteDocAsync(int ad_seq, int drafterSeq)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        int n = await con.ExecuteAsync(
          "UPDATE APPR_DOC SET is_deleted=1 WHERE ad_seq=@ad_seq AND drafter_seq=@drafterSeq AND doc_status IN (0,3,4)",
          new { ad_seq, drafterSeq });
        return n > 0;
      }
    }

    // ── 첨부 ───────────────────────────────────────────────────────
    public async Task CreateFileAsync(appr_file f)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        await con.ExecuteAsync(@"
INSERT INTO APPR_FILE (ad_seq, file_dir, file_origin_path, file_path, file_extension, file_size, reg_date)
VALUES (@ad_seq, @file_dir, @file_origin_path, @file_path, @file_extension, @file_size, GETDATE())", f);
    }

    public async Task<appr_file> SelectFileAsync(int af_seq)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        return await con.QueryFirstOrDefaultAsync<appr_file>("SELECT * FROM APPR_FILE WHERE af_seq=@af_seq", new { af_seq });
    }

    public async Task<bool> DeleteFileAsync(int af_seq, int ad_seq)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        int n = await con.ExecuteAsync("DELETE FROM APPR_FILE WHERE af_seq=@af_seq AND ad_seq=@ad_seq", new { af_seq, ad_seq });
        return n > 0;
      }
    }

    // ── 댓글 ───────────────────────────────────────────────────────
    public async Task CreateCommentAsync(appr_comment c)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        await con.ExecuteAsync(@"
INSERT INTO APPR_COMMENT (ad_seq, writer_seq, writer_name, content, reg_date)
VALUES (@ad_seq, @writer_seq, @writer_name, @content, GETDATE())", c);
    }

    public async Task<bool> DeleteCommentAsync(int ac_seq, int writerSeq)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        int n = await con.ExecuteAsync(
          "UPDATE APPR_COMMENT SET is_deleted=1 WHERE ac_seq=@ac_seq AND writer_seq=@writerSeq", new { ac_seq, writerSeq });
        return n > 0;
      }
    }
  }
}
