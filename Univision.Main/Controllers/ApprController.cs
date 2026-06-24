using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using Univision.Main.Infrastructure;
using Univision.Main.Infrastructure.Mailing;
using Univision.Security;

namespace Univision.Main.Controllers
{
  // 전자결재 (Appr)
  public class ApprController : BaseController
  {
    private readonly ApprovalRepository _repo = new ApprovalRepository();

    // ── 기안함 ─────────────────────────────────────────────────────
    public async Task<ActionResult> WorkList(int status = -1)
    {
      ViewBag.status = status;
      var list = await _repo.SelectWorkListAsync(AppIdentity.user_seq, status);
      return View(list);
    }

    // ── 결재함 (0 결재대기 / 1 결재완료) ──────────────────────────
    public async Task<ActionResult> ApprovalList(int tab = 0)
    {
      ViewBag.tab = tab;
      var list = await _repo.SelectApprovalListAsync(AppIdentity.user_seq, tab);
      return View(list);
    }

    // ── 기안 작성 / 수정 / 재기안 ─────────────────────────────────
    public async Task<ActionResult> Write(int ad_seq = 0, int copy = 0)
    {
      var vm = new ApprDocViewModel();

      if (copy > 0)
      {
        // 재기안: 원본 복사 → 신규 임시 문서 폼 (첨부는 복사하지 않음)
        var src = await _repo.SelectDocAsync(copy);
        if (src != null)
        {
          vm.doc.title = "[재기안] " + src.doc.title;
          vm.doc.content = src.doc.content;
          vm.doc.copy_from = copy;
          vm.lines = src.lines.Select(l => new appr_line
          {
            approver_seq = l.approver_seq,
            approver_name = l.approver_name,
            approver_position = l.approver_position,
            line_type = l.line_type
          }).ToList();
        }
      }
      else if (ad_seq > 0)
      {
        // 수정: 본인 + 임시(0)/회수(4) 만 편집 가능
        var src = await _repo.SelectDocAsync(ad_seq);
        if (src == null || src.doc.drafter_seq != AppIdentity.user_seq || (src.doc.doc_status != 0 && src.doc.doc_status != 4))
          return RedirectToAction("WorkList");
        vm = src;
      }

      ViewBag.users = await GetUserListAsync();
      return View(vm);
    }

    // ── 저장 (submit=0 임시저장 / 1 상신) ─────────────────────────
    [HttpPost]
    public async Task<JsonResult> SaveDoc(int ad_seq, string title, string content, string line_json, string ref_json, int submit, string del_files)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(title))
          return Json(new { ok = false, message = "제목을 입력해주세요." });

        var approvers = string.IsNullOrWhiteSpace(line_json)
          ? new List<appr_line>() : (JsonConvert.DeserializeObject<List<appr_line>>(line_json) ?? new List<appr_line>());
        var refs = string.IsNullOrWhiteSpace(ref_json)
          ? new List<appr_line>() : (JsonConvert.DeserializeObject<List<appr_line>>(ref_json) ?? new List<appr_line>());

        if (submit == 1 && approvers.Count == 0)
          return Json(new { ok = false, message = "상신하려면 결재선을 1명 이상 지정해야 합니다." });

        // 신규 / 수정 분기
        if (ad_seq == 0)
        {
          ad_seq = await _repo.CreateDocAsync(new appr_doc
          {
            title = title,
            content = content,
            drafter_seq = AppIdentity.user_seq,
            drafter_name = AppIdentity.name,
            drafter_ud_seq = AppIdentity.ud_seq,
            doc_status = 0,
            cur_order = 0
          });
        }
        else
        {
          var doc = await _repo.SelectDocOnlyAsync(ad_seq);
          if (doc == null || doc.drafter_seq != AppIdentity.user_seq || (doc.doc_status != 0 && doc.doc_status != 4))
            return Json(new { ok = false, message = "수정할 수 없는 문서입니다." });
          await _repo.UpdateDocContentAsync(new appr_doc { ad_seq = ad_seq, title = title, content = content });
        }

        var allLines = new List<appr_line>();
        int on = 1; foreach (var a in approvers) { a.line_type = 0; a.order_no = on++; allLines.Add(a); }
        int rn = 1; foreach (var rf in refs) { rf.line_type = 1; rf.order_no = rn++; allLines.Add(rf); }
        await _repo.ReplaceLinesAsync(ad_seq, allLines);

        // 첨부 삭제
        if (!string.IsNullOrWhiteSpace(del_files))
        {
          foreach (var s in del_files.Split(','))
          {
            if (int.TryParse(s, out int af))
            {
              var f = await _repo.SelectFileAsync(af);
              if (f != null && f.ad_seq == ad_seq)
              {
                await _repo.DeleteFileAsync(af, ad_seq);
                try { if (System.IO.File.Exists(f.file_origin_path)) System.IO.File.Delete(f.file_origin_path); } catch { }
              }
            }
          }
        }

        // 첨부 업로드
        var fiUpload = new FileUpload();
        for (int i = 0; i < Request.Files.Count; i++)
        {
          var file = Request.Files[i];
          if (file == null || file.ContentLength == 0) continue;
          var rst = fiUpload.UploadTemp(Server.MapPath("~/UploadedFiles"), "appr", ad_seq.ToString(), file);
          if (rst.status)
          {
            await _repo.CreateFileAsync(new appr_file
            {
              ad_seq = ad_seq,
              file_dir = "/" + rst.dbPath,
              file_origin_path = rst.filePath,
              file_path = rst.name,
              file_extension = rst.extension,
              file_size = file.ContentLength
            });
          }
        }

        if (submit == 1)
        {
          await _repo.SubmitAsync(ad_seq);
          await NotifyAsync(ad_seq, null);   // 첫 결재자에게 알림
        }

        return Json(new { ok = true, ad_seq, message = submit == 1 ? "상신되었습니다." : "임시저장 되었습니다." });
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = "저장 중 오류: " + e.Message });
      }
    }

    // ── 문서 상세 ─────────────────────────────────────────────────
    public async Task<ActionResult> Detail(int ad_seq)
    {
      var vm = await _repo.SelectDocAsync(ad_seq);
      if (vm == null) return RedirectToAction("WorkList");

      int me = AppIdentity.user_seq;
      var curLine = vm.lines.FirstOrDefault(x => x.line_type == 0 && x.order_no == vm.doc.cur_order);
      ViewBag.isDrafter = vm.doc.drafter_seq == me;
      ViewBag.isMyTurn = vm.doc.doc_status == 1 && curLine != null && curLine.approver_seq == me && curLine.line_status == 0;
      ViewBag.canRecall = vm.doc.drafter_seq == me && vm.doc.doc_status == 1;
      ViewBag.canEdit = vm.doc.drafter_seq == me && (vm.doc.doc_status == 0 || vm.doc.doc_status == 4);
      return View(vm);
    }

    [HttpPost]
    public async Task<JsonResult> Approve(int ad_seq, string opinion)
    {
      int r = await _repo.ProcessApprovalAsync(ad_seq, AppIdentity.user_seq, true, opinion);
      if (r == 1) await NotifyAsync(ad_seq, opinion);   // 다음 결재자 또는 (최종 승인 시) 기안자에게 알림
      return Json(r == 1 ? new { ok = true, message = "승인 처리되었습니다." }
                         : new { ok = false, message = "결재 권한이 없거나 처리할 수 없는 상태입니다." });
    }

    [HttpPost]
    public async Task<JsonResult> Reject(int ad_seq, string opinion)
    {
      if (string.IsNullOrWhiteSpace(opinion))
        return Json(new { ok = false, message = "반려 사유를 입력해주세요." });
      int r = await _repo.ProcessApprovalAsync(ad_seq, AppIdentity.user_seq, false, opinion);
      if (r == 1) await NotifyAsync(ad_seq, opinion);   // 기안자에게 반려 알림
      return Json(r == 1 ? new { ok = true, message = "반려 처리되었습니다." }
                         : new { ok = false, message = "결재 권한이 없거나 처리할 수 없는 상태입니다." });
    }

    [HttpPost]
    public async Task<JsonResult> Recall(int ad_seq)
    {
      bool ok = await _repo.RecallAsync(ad_seq, AppIdentity.user_seq);
      return Json(ok ? new { ok = true, message = "회수되었습니다. 수정 후 다시 상신할 수 있습니다." }
                     : new { ok = false, message = "회수할 수 없는 문서입니다." });
    }

    [HttpPost]
    public async Task<JsonResult> DeleteDoc(int ad_seq)
    {
      bool ok = await _repo.DeleteDocAsync(ad_seq, AppIdentity.user_seq);
      return Json(new { ok, message = ok ? "삭제되었습니다." : "삭제할 수 없는 문서입니다." });
    }

    [HttpPost]
    public async Task<JsonResult> AddComment(int ad_seq, string content)
    {
      if (string.IsNullOrWhiteSpace(content))
        return Json(new { ok = false, message = "내용을 입력해주세요." });
      await _repo.CreateCommentAsync(new appr_comment
      {
        ad_seq = ad_seq,
        writer_seq = AppIdentity.user_seq,
        writer_name = AppIdentity.name,
        content = content
      });
      return Json(new { ok = true });
    }

    [HttpPost]
    public async Task<JsonResult> DeleteComment(int ac_seq)
    {
      bool ok = await _repo.DeleteCommentAsync(ac_seq, AppIdentity.user_seq);
      return Json(new { ok });
    }

    // ── 첨부 다운로드 ─────────────────────────────────────────────
    public async Task<ActionResult> DownloadFile(int af_seq)
    {
      var f = await _repo.SelectFileAsync(af_seq);
      if (f == null || string.IsNullOrEmpty(f.file_origin_path) || !System.IO.File.Exists(f.file_origin_path))
        return new HttpNotFoundResult("파일을 찾을 수 없습니다.");
      return File(f.file_origin_path, System.Net.Mime.MediaTypeNames.Application.Octet, f.file_path);
    }

    // ── 결재선 선택용 직원 목록 (JSON) ────────────────────────────
    [HttpGet]
    public async Task<JsonResult> UserList()
    {
      var users = await GetUserListAsync();
      return Json(users, JsonRequestBehavior.AllowGet);
    }

    private async Task<List<object>> GetUserListAsync()
    {
      var ar = new AccountRepository();
      var users = await ar.SelectAllUser();
      return users
        .Where(u => u.uv_seq != 0)
        .OrderBy(u => u.name)
        .Select(u => (object)new { uv_seq = u.uv_seq, name = u.name })
        .ToList();
    }

    // ── 알림 메일: 진행중이면 현재 결재자, 완료/반려면 기안자에게 (실패는 무시) ──
    private async Task NotifyAsync(int ad_seq, string opinion)
    {
      try
      {
        var vm = await _repo.SelectDocAsync(ad_seq);
        if (vm == null) return;
        var doc = vm.doc;

        int recipientSeq;
        string action;
        if (doc.doc_status == 3) { recipientSeq = doc.drafter_seq; action = "반려"; }
        else if (doc.doc_status == 2) { recipientSeq = doc.drafter_seq; action = "최종 승인 완료"; }
        else if (doc.doc_status == 1)
        {
          var cur = vm.lines.FirstOrDefault(l => l.line_type == 0 && l.order_no == doc.cur_order);
          if (cur == null) return;
          recipientSeq = cur.approver_seq;
          action = "결재 요청";
        }
        else return;

        var user = await _repo.SelectUserContactAsync(recipientSeq);
        if (user == null || string.IsNullOrWhiteSpace(user.email)) return;

        string url = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("Detail", "Appr", new { ad_seq });

        var dto = new ApprNotifyDto
        {
          ToArr = new[] { user.email },
          recipient = user.name ?? "",
          actionkor = action,
          docno = doc.doc_no ?? "",
          title = doc.title ?? "",
          drafter = doc.drafter_name ?? "",
          opinion = string.IsNullOrWhiteSpace(opinion) ? "-" : opinion,
          detailurl = url
        };
        var tmpl = new TempleteDto
        {
          MailSubject = "[전자결재] {{actionkor}} - {{title}}",
          MailBody = ApprMailBody()
        };
        new MailService().SendApprNotifyMail(dto, tmpl);
      }
      catch { /* 알림 메일 실패는 무시 */ }
    }

    private static string ApprMailBody()
    {
      return @"
<div style='font-family:맑은 고딕,sans-serif;font-size:14px;color:#333;line-height:1.6;'>
  <p>{{recipient}}님,</p>
  <p><b>[{{actionkor}}]</b> 전자결재 알림입니다.</p>
  <table style='border-collapse:collapse;border:1px solid #e3e8ee;'>
    <tr><td style='padding:6px 12px;color:#888;background:#f7f9fc;'>문서번호</td><td style='padding:6px 12px;'>{{docno}}</td></tr>
    <tr><td style='padding:6px 12px;color:#888;background:#f7f9fc;'>제목</td><td style='padding:6px 12px;'>{{title}}</td></tr>
    <tr><td style='padding:6px 12px;color:#888;background:#f7f9fc;'>기안자</td><td style='padding:6px 12px;'>{{drafter}}</td></tr>
    <tr><td style='padding:6px 12px;color:#888;background:#f7f9fc;'>의견</td><td style='padding:6px 12px;'>{{opinion}}</td></tr>
  </table>
  <p style='margin-top:16px;'><a href='{{detailurl}}' style='background:#4b89dc;color:#fff;padding:9px 18px;border-radius:6px;text-decoration:none;'>문서 보기</a></p>
</div>";
    }
  }
}
