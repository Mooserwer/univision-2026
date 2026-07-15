using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using Univision.Main.Infrastructure;
using Univision.Main.Models.GPT;
using Univision.Security;
using ITextImage = iTextSharp.text.Image;
using SpirePdfDoc = Spire.Pdf.PdfDocument;
// iTextSharp.text.pdf.parser.Path 와 System.IO.Path 충돌 방지 → using 제거, 완전 한정명 사용

namespace Univision.Main.Controllers
{
  public class nGptController : BaseController
  {
    // ─────────────────────────────────────────────────────────
    //  메인 뷰 — 800px 팝업 (레이아웃 없음)
    // ─────────────────────────────────────────────────────────
    public ActionResult MakeupAI()
    {
      return View();
    }

    // ─────────────────────────────────────────────────────────
    //  이력서 AI 신규등록 — 후보자 추출 페이지 (레이아웃 없음)
    // ─────────────────────────────────────────────────────────
    public ActionResult CandidateAI()
    {
      return View();
    }

    // ─────────────────────────────────────────────────────────
    //  이력서 파일 저장 (서버 텍스트추출 없이 임시 폴더에만 저장)
    //  내용 분석은 클라이언트에서 OpenAI 직접 업로드로 처리한다.
    //  반환: { ok, result: can_resume(file_type=""), temp_folder }
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    public ActionResult CandidateResumeUpload(HttpPostedFileBase file)
    {
      try
      {
        if (file == null || file.ContentLength == 0)
          return Json(new { ok = false, message = "파일이 없습니다." });

        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        string uploadTmpFolder = Utils.ReturnUniqueValue(AppIdentity.user_seq);
        var rst = fiUpload.UploadTemp(path, "Temp", uploadTmpFolder, file);

        if (!rst.status)
          throw new Exception(rst.statusMessage);

        var cr = new can_resume()
        {
          file_dir         = rst.dbPath,
          file_origin_path = rst.filePath,
          file_path        = rst.name,
          file_extension   = rst.extension,
          file_type        = ""
        };

        return Json(new
        {
          ok = true,
          message = "Upload Complete.",
          result = cr,
          temp_folder = uploadTmpFolder
        });
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = "[Error] " + e.Message });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  PDF → Word 변환 페이지
    // ─────────────────────────────────────────────────────────
    public ActionResult PdfToWord()
    {
      return View();
    }

    [HttpPost]
    public ActionResult PdfToWord(HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        SearchEngineRepository ser = new SearchEngineRepository();
        string path = Server.MapPath("~/UploadedFiles");
        var rst = fiUpload.UploadTemp(path, "PDFCONVERT", "UPLOAD", file);

        if (!rst.status)
          throw new Exception(rst.statusMessage);

        Spire.Pdf.License.LicenseProvider.SetLicenseKey("XbgnAQDxg0XYHgvSJ+3odG7syXqSPH2GsCw6CFr6JsqgIshAMr5pysViL41k+G7HkwIwcQvvv7/UFQUTf1jaA1la218ELXw5aZbRMgqdxfq6Mbsgkmn6N4mey6e/CACovGKq/b0IWPRr5Kt/eYzzsJZL0f315E+4fM3zV4evd+lWj9K+5rWPJwgH3mUgOuEGA6pMROO/p6mUtQ6i3Zy3FxUno6xsYVgskuP6XRVLAEx0ZOUlBgfh5J08/BYuPaoT5S1Mej3yv3nJCsrgcYi3q9/XsNH74ZuSEBY79AkrtJIihpL4wxHcJkzXA/giipWOe/MsTI3go7WOPtrwirVGJ0n22hJTa4jVf70au5jhtggJlnVEoDWpfUyCGbkDKKL2PYG0Aq5ogQRgrvLiAbjNW2iqmOzAZnROxFeJnTGscbGTr4bErw/HUroqmXTErqu86YPr6PMQB7xg/lfAtXif/IpovLvYeEjm+VqjcFaDlgq+T9pGH4kkloK8R0lXqHuVO6j96FBExSxRBnO8N5Ioq/cIpzc73pwcGAOwK740NTU0707Fl3PWXAB03o7rkh2ur+aq+ruJVfvOV0HrKGuM7uymrXEaxcg7jdkedOnD7SBxfOai9vRXPf+1bI6hih+miAVkimWCVrEG+jmbAPaKiDfbunr5L1/0EdJnSu4LaQeadi8eseJPN1ihReVWeXcet+j6fxCy/tqBLKEALabXGhPokq793nHQilnRJLna6YMMVCyX0TIjaLJ0MqOGNWlWOnNDfFq/XYqHAZ1fzsFVb7h0P3bbfRKXgPRo6TOr1iSK/mGlwuGeV7hzKxATeF3ZmHW7LBDCsqKJqLqQSqMY2ijoq9iNecQZ0YKqjXjKkJ6wT85hZz4QyYxwxy1X4QwRn8w6tavfBMwzl6a36yIavimAsBfJMlomk49kbbiQJkuZVlbzP10fzM1QsxVjTCQkMI9W4/mYwTnvWnHwWZOBixLHNOaor3/TTRt8Jq6d+hMetxj8hYhDI35pLJgHHZoKQ1wKcumTqYKn90swAjQSru8T5OWA5tZsoaN9qNII7IgHWdmzJssACqq2Pf3Ei+P5o/+mh27kLIjlZhlzW41/61tfvBWAIC+NrvBtGHPHpD/S0mvwY1QilCh6x7ZP3qvuGNvCcX/W/fB5SEhBXqMQhufMlXgk58i4LQrd9n7AnSwtsmTGino2HfIiTRh8eACz4zyr082Ir6lyTy4C9+eY5dE0JI6tcb5ke+59ceMgDYzb5mcrFGtcY5s4RgS9hgGBwLV0/dtIy+Hvcg+E1qRgnfuJYkEKzoxaTBmGXJLK+pE9kqpZSk6KOuzJLj82Rh/GbsQPueLJHm+Yw3CcEH9SRe/eHgNZrfJSE9uL4DMtxd5Vg8ppKfD2Cc6TzLyZEmAYLkjH5o5RqI9/wgSSnIfmZ0rwoJMaONnbIZeq6NJUWjnlNFYrxOC2mY9ikbLAMhEYj80fY7ueoiJijhqFblFWns6Y02fRjVjrMS5gtd0EEQ0m4gzAbTygMwdxWbVCJ1DpOWsMa25eiWTYCN9QlhYkL4qeLwU0g9u8tHh7kTCrkBM=");

        FileInfo info = new FileInfo(rst.filePath);
        if (!info.Exists)
          return new HttpNotFoundResult("파일을 찾을 수 없습니다.");

        string converted_path = System.IO.Path.GetDirectoryName(rst.filePath) + @"\CONVERT\";
        string converted_name = System.IO.Path.GetFileNameWithoutExtension(rst.filePath);
        string converted_full = converted_path + converted_name + ".docx";

        var doc = new SpirePdfDoc();
        doc.LoadFromFile(rst.filePath);
        doc.ConvertOptions.SetPdfToDocOptions(true, true);

        FileInfo info2 = new FileInfo(converted_full);
        if (!info2.Exists)
        {
          doc.SaveToFile(converted_full, Spire.Pdf.FileFormat.DOCX);
          System.IO.File.Delete(rst.filePath);
          doc.Dispose();
        }

        // Spire.PDF 평가판 워터마크 문구 제거
        RemoveSpireEvaluationWarning(converted_full);

        byte[] fileBytes = ReadAllBytes(converted_full);
        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, converted_name + ".docx");
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = "[Error] " + e.Message });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  HWP 파일 업로드 → 텍스트 추출 (HWP 전용 폴백)
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<ActionResult> GptResumeUpload()
    {
      try
      {
        var file = Request.Files.Count > 0 ? Request.Files[0] : null;
        if (file == null || file.ContentLength == 0)
          return Json(new { ok = false, message = "파일이 없습니다." });

        var ext = System.IO.Path.GetExtension(file.FileName).ToLower();
        var fileName = file.FileName;

        var tempDir = Server.MapPath("~/UploadedFiles/gpt_temp/");
        System.IO.Directory.CreateDirectory(tempDir);
        var tempName = Guid.NewGuid().ToString("N") + ext;
        var tempPath = System.IO.Path.Combine(tempDir, tempName);

        file.SaveAs(tempPath);

        string fileContent = "";
        try
        {
          if (ext == ".pdf")
          {
            var creator = GetCreatorFromPdf(tempPath);
            if (creator != "react-pdf")
              fileContent = ExtractTextFromPdf(tempPath);
          }

          if (string.IsNullOrEmpty(fileContent))
          {
            FileUpload fiUpload = new FileUpload();
            SearchEngineRepository ser = new SearchEngineRepository();
#if DEBUG
            string source = "/media/uploadfiles/kmpdl/20250313.docx";
#else
            string kmpdlName = fiUpload.CopyTempFileForKMPDL(tempPath, tempName);
            string source = "/media/uploadfiles/kmpdl/" + kmpdlName;
#endif
            fileContent = ser.TempResumeCheck(source);
          }
        }
        finally
        {
          // System.IO.File 을 명시적으로 사용 (Controller.File() 메서드와 충돌 방지)
          if (System.IO.File.Exists(tempPath))
            System.IO.File.Delete(tempPath);
        }

        if (string.IsNullOrEmpty(fileContent))
          return Json(new { ok = false, message = "파일 내용을 인식할 수 없습니다. (텍스트 추출 실패)" });

        return Json(new
        {
          ok = true,
          file_content = fileContent,
          result = new { file_path = fileName, file_extension = ext }
        });
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = e.Message });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  Makeup DOCX 파일 생성 (2026 버전)
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<JsonResult> MakeMakeup2026(string makeup_model, string file_name, string file_type)
    {
      try
      {
        var model = JsonConvert.DeserializeObject<MakeupCreateModel>(makeup_model);

        if (string.IsNullOrWhiteSpace(model.candidate))
          return Json(new { ok = false, message = "후보자 이름을 확인 할 수 없습니다." });

        string req_name = AppIdentity.name;
        string file_type_str = (file_type == "E") ? "E" : "";
        string template_name = (file_type == "E") ? "EN_makeup_blank.docx" : "KR_makeup_blank.docx";

        string fileName = string.Format("{0}_{1}_{2}(by GPT).docx", file_name, file_type_str, req_name);
        string samplePath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/makeupSample/"), template_name);
        string saveDirPath = Server.MapPath("~/UploadedFiles/gpt_makeup_2026/");
        string file_dir = System.IO.Path.Combine(Utils.GetRootUrl(Request), saveDirPath);
        System.IO.Directory.CreateDirectory(file_dir);

        MakeupMaker maker = new MakeupMaker();
        (bool, string) result;
        if (file_type == "E")
          result = await maker.MakeEng(model, samplePath, file_dir, fileName);
        else
          result = await maker.Make(model, samplePath, file_dir, fileName);

        if (!result.Item1)
          throw new Exception(result.Item2);

        return Json(new
        {
          ok = true,
          // 파일명에 &, 공백, 괄호 등 특수문자가 있어도 다운로드되도록 파일명 구간을 URL 인코딩
          file_url = Request.Url.GetLeftPart(UriPartial.Authority) + "/UploadedFiles/gpt_makeup_2026/" + Uri.EscapeDataString(result.Item2),
          file_name = result.Item2,
          message = ""
        });
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = e.Message });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  gpt_use_history — 이력 생성 (처리 시작)
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<JsonResult> CreateGptUseHistory(
      byte use_type,
      string file_path,
      string req_lang,
      string gpt_model)
    {
      try
      {
        var repo = new GptUseHistoryRepository();
        var hist = new gpt_use_history
        {
          use_type = use_type,
          file_path = file_path,
          file_full_path = "",
          req_lang = req_lang,
          gpt_model = gpt_model,
          create_seq = AppIdentity.user_seq
        };
        int guh_seq = await repo.CreateAsync(hist);
        return Json(new { ok = true, guh_seq });
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = e.Message });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  gpt_use_history — 완료 업데이트
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<JsonResult> CompleteGptUseHistory(
      int guh_seq,
      string result_nc,
      string result_oc,
      string output_file,
      string output_path,
      decimal proc_sec)
    {
      try
      {
        var repo = new GptUseHistoryRepository();
        await repo.CompleteAsync(guh_seq, result_nc, result_oc, output_file, output_path, proc_sec);
        return Json(new { ok = true });
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = e.Message });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  gpt_use_history — 실패 업데이트
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<JsonResult> FailGptUseHistory(int guh_seq, string error_msg)
    {
      try
      {
        var repo = new GptUseHistoryRepository();
        await repo.FailAsync(guh_seq, error_msg);
        return Json(new { ok = true });
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = e.Message });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  PDF 상단 로고 삽입 후 다운로드
    //  position : "right"(default) | "left"   — 상단 좌/우 위치
    //  pages    : "all"(default)   | "first"  — 모든 페이지 / 첫 페이지만
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    public ActionResult InsertLogoToPdf(HttpPostedFileBase file, string position = "right", string pages = "all")
    {
      try
      {
        if (file == null || file.ContentLength == 0)
          return Json(new { ok = false, message = "파일이 없습니다." });

        string ext = System.IO.Path.GetExtension(file.FileName).ToLower();
        if (ext != ".pdf")
          return Json(new { ok = false, message = "PDF 파일만 지원합니다." });

        string logoPath = Server.MapPath("~/Images/unico_logo.png");
        if (!System.IO.File.Exists(logoPath))
          return Json(new { ok = false, message = "로고 파일을 찾을 수 없습니다." });

        byte[] pdfBytes;
        using (var ms = new MemoryStream())
        {
          file.InputStream.CopyTo(ms);
          pdfBytes = ms.ToArray();
        }

        byte[] outputBytes;
        using (var outputStream = new MemoryStream())
        {
          var reader = new PdfReader(pdfBytes);
          var stamper = new PdfStamper(reader, outputStream);

          const float CM_TO_PT = 28.3465f;
          const float LOGO_W = 3.21f * CM_TO_PT;
          const float LOGO_H = 1.06f * CM_TO_PT;
          const float MARGIN = 18f;

          bool isLeft = position == "left";
          int lastPage = pages == "first" ? 1 : reader.NumberOfPages;

          for (int i = 1; i <= lastPage; i++)
          {
            var logo = ITextImage.GetInstance(logoPath);
            logo.ScaleAbsolute(LOGO_W, LOGO_H);

            var page = reader.GetPageSizeWithRotation(i);
            logo.SetAbsolutePosition(
              isLeft ? MARGIN : page.Width - LOGO_W - MARGIN,
              page.Height - LOGO_H - MARGIN
            );

            var over = stamper.GetOverContent(i);
            over.AddImage(logo);
          }

          stamper.Close();
          reader.Close();
          outputBytes = outputStream.ToArray();
        }

        string outName = System.IO.Path.GetFileNameWithoutExtension(file.FileName) + "_logo.pdf";
        // MVC Controller.File() 메서드 명시적 호출
        return new FileContentResult(outputBytes, "application/pdf")
        {
          FileDownloadName = outName
        };
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = "[오류] " + e.Message });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  PDF 연락처(이메일/전화번호) 추정 텍스트 제거 (흰색 박스 마스킹)
    // ─────────────────────────────────────────────────────────
    private static readonly System.Text.RegularExpressions.Regex EmailRegex =
      new System.Text.RegularExpressions.Regex(
        @"[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    private static readonly System.Text.RegularExpressions.Regex PhoneRegex =
      new System.Text.RegularExpressions.Regex(
        @"(\+?\d{1,3}[-.\s]?)?\(?0\d{1,2}\)?[-.\s]?\d{3,4}[-.\s]?\d{4}" +   // 국내(010-1234-5678, (02)123-4567 등)
        @"|\+\d{1,3}[-.\s]?\d{2,4}[-.\s]?\d{3,4}[-.\s]?\d{3,4}" +           // 국제(+82 10-1234-5678 등)
        @"|\b\d{10,11}\b",                                                  // 구분자 없는 10~11자리
        System.Text.RegularExpressions.RegexOptions.Compiled);

    [HttpPost]
    public ActionResult RemoveContactsFromPdf(HttpPostedFileBase file)
    {
      try
      {
        if (file == null || file.ContentLength == 0)
          return Json(new { ok = false, message = "파일이 없습니다." });

        string ext = System.IO.Path.GetExtension(file.FileName).ToLower();
        if (ext != ".pdf")
          return Json(new { ok = false, message = "PDF 파일만 지원합니다." });

        byte[] pdfBytes;
        using (var ms = new MemoryStream())
        {
          file.InputStream.CopyTo(ms);
          pdfBytes = ms.ToArray();
        }

        byte[] outputBytes;
        using (var outputStream = new MemoryStream())
        {
          var reader = new PdfReader(pdfBytes);
          var parser = new iTextSharp.text.pdf.parser.PdfReaderContentParser(reader);
          var stamper = new PdfStamper(reader, outputStream);

          for (int p = 1; p <= reader.NumberOfPages; p++)
          {
            var listener = new ContactRedactListener();
            parser.ProcessContent(p, listener);

            var rects = listener.GetRedactRects();
            if (rects.Count == 0) continue;

            var over = stamper.GetOverContent(p);
            over.SaveState();
            over.SetColorFill(iTextSharp.text.BaseColor.WHITE);
            foreach (var r in rects)
            {
              // 좌우 약간의 여백을 두어 글자가 완전히 가려지도록 함
              over.Rectangle(r.Llx - 1f, r.Lly - 1f, (r.Urx - r.Llx) + 2f, (r.Ury - r.Lly) + 2f);
            }
            over.Fill();
            over.RestoreState();
          }

          stamper.Close();
          reader.Close();
          outputBytes = outputStream.ToArray();
        }

        string outName = System.IO.Path.GetFileNameWithoutExtension(file.FileName) + "_redacted.pdf";
        return new FileContentResult(outputBytes, "application/pdf")
        {
          FileDownloadName = outName
        };
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = "[오류] " + e.Message });
      }
    }

    // 마스킹 영역 사각형
    private struct RedactRect
    {
      public float Llx, Lly, Urx, Ury;
    }

    // PDF 텍스트 위치를 수집해 이메일/전화번호 추정 구간의 사각형을 산출하는 리스너
    private class ContactRedactListener : iTextSharp.text.pdf.parser.IRenderListener
    {
      private class Chunk
      {
        public string Text;
        public float Llx, Lly, Urx, Ury;
      }

      private readonly List<Chunk> _chunks = new List<Chunk>();

      public void BeginTextBlock() { }
      public void EndTextBlock() { }
      public void RenderImage(iTextSharp.text.pdf.parser.ImageRenderInfo renderInfo) { }

      public void RenderText(iTextSharp.text.pdf.parser.TextRenderInfo renderInfo)
      {
        string text = renderInfo.GetText();
        if (string.IsNullOrEmpty(text)) return;

        var descent = renderInfo.GetDescentLine();
        var ascent = renderInfo.GetAscentLine();
        var sp = descent.GetStartPoint();
        var ep = ascent.GetEndPoint();

        float x1 = sp[iTextSharp.text.pdf.parser.Vector.I1];
        float y1 = sp[iTextSharp.text.pdf.parser.Vector.I2];
        float x2 = ep[iTextSharp.text.pdf.parser.Vector.I1];
        float y2 = ep[iTextSharp.text.pdf.parser.Vector.I2];

        _chunks.Add(new Chunk
        {
          Text = text,
          Llx = Math.Min(x1, x2),
          Lly = Math.Min(y1, y2),
          Urx = Math.Max(x1, x2),
          Ury = Math.Max(y1, y2)
        });
      }

      // 같은 줄로 묶고 이메일/전화 정규식을 적용해 마스킹 사각형 목록 반환
      public List<RedactRect> GetRedactRects()
      {
        var result = new List<RedactRect>();

        // 같은 줄(베이스라인 Y 근사)로 그룹핑
        var lines = _chunks
          .GroupBy(c => (int)Math.Round(c.Lly / 3.0))
          .ToList();

        foreach (var line in lines)
        {
          var ordered = line.OrderBy(c => c.Llx).ToList();

          // 줄 문자열을 만들면서 각 chunk의 문자열 범위를 기록
          var sb = new System.Text.StringBuilder();
          var ranges = new List<(int start, int end, Chunk chunk)>();
          foreach (var ch in ordered)
          {
            int start = sb.Length;
            sb.Append(ch.Text);
            ranges.Add((start, sb.Length, ch));
          }
          string lineText = sb.ToString();
          if (lineText.Length == 0) continue;

          foreach (System.Text.RegularExpressions.Match m in EmailRegex.Matches(lineText))
            AddMatchRect(m, ranges, result);
          foreach (System.Text.RegularExpressions.Match m in PhoneRegex.Matches(lineText))
            AddMatchRect(m, ranges, result);
        }

        return result;
      }

      private void AddMatchRect(System.Text.RegularExpressions.Match m,
        List<(int start, int end, Chunk chunk)> ranges, List<RedactRect> result)
      {
        int ms = m.Index;
        int me = m.Index + m.Length;

        float llx = float.MaxValue, lly = float.MaxValue, urx = float.MinValue, ury = float.MinValue;
        bool any = false;

        foreach (var r in ranges)
        {
          // 매칭 구간과 겹치는 chunk의 사각형을 합집합
          if (r.start < me && r.end > ms)
          {
            llx = Math.Min(llx, r.chunk.Llx);
            lly = Math.Min(lly, r.chunk.Lly);
            urx = Math.Max(urx, r.chunk.Urx);
            ury = Math.Max(ury, r.chunk.Ury);
            any = true;
          }
        }

        if (any)
          result.Add(new RedactRect { Llx = llx, Lly = lly, Urx = urx, Ury = ury });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  Spire.PDF 평가판 워터마크 문구 제거
    //  "Evaluation Warning : The document was created with Spire.PDF for .NET."
    //  변환된 DOCX의 본문/머리글/바닥글에서 해당 문구가 포함된 단락을 삭제
    // ─────────────────────────────────────────────────────────
    private void RemoveSpireEvaluationWarning(string docxPath)
    {
      const string WARN_TEXT = "Evaluation Warning";

      try
      {
        using (var doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(docxPath, true))
        {
          bool changed = false;
          var main = doc.MainDocumentPart;
          if (main == null) return;

          // 본문 + 머리글 + 바닥글의 모든 루트 요소 수집
          var roots = new List<DocumentFormat.OpenXml.OpenXmlElement>();
          if (main.Document?.Body != null) roots.Add(main.Document.Body);
          foreach (var hp in main.HeaderParts) if (hp.Header != null) roots.Add(hp.Header);
          foreach (var fp in main.FooterParts) if (fp.Footer != null) roots.Add(fp.Footer);

          foreach (var root in roots)
          {
            var targets = root
              .Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>()
              .Where(t => t.Text != null && t.Text.Contains(WARN_TEXT))
              .ToList();

            foreach (var t in targets)
            {
              var para = t.Ancestors<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().FirstOrDefault();
              if (para != null) para.Remove();
              else t.Remove();
              changed = true;
            }
          }

          if (changed) main.Document.Save();
        }
      }
      catch
      {
        // 워터마크 제거 실패해도 변환 파일 다운로드는 계속 진행
      }
    }

    // ─────────────────────────────────────────────────────────
    //  파일 읽기 헬퍼
    // ─────────────────────────────────────────────────────────
    private byte[] ReadAllBytes(string filePath)
    {
      using (var fs = System.IO.File.OpenRead(filePath))
      {
        var data = new byte[fs.Length];
        int br = fs.Read(data, 0, data.Length);
        if (br != fs.Length) throw new IOException(filePath);
        return data;
      }
    }

    // ─────────────────────────────────────────────────────────
    //  PDF 헬퍼 (iTextSharp.text.pdf.parser 완전 한정명 사용)
    // ─────────────────────────────────────────────────────────
    private string GetCreatorFromPdf(string path)
    {
      try
      {
        using (var reader = new PdfReader(path))
          return reader.Info["Creator"] ?? "";
      }
      catch { return ""; }
    }

    private string ExtractTextFromPdf(string path)
    {
      try
      {
        using (var reader = new PdfReader(path))
        {
          string text = "";
          iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy =
            new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
          for (int i = 1; i <= reader.NumberOfPages; i++)
            text += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i, strategy);
          return text;
        }
      }
      catch { return ""; }
    }

    // ─────────────────────────────────────────────────────────
    //  MakeupGPT 부분뷰 — 프로젝트/후보자 단위 이력서 메이크업
    // ─────────────────────────────────────────────────────────
    public async Task<PartialViewResult> MakeupGPT(int p_seq, List<int> pic_seq, int c_seq = 0)
    {
      ProjectRepository pr = new ProjectRepository();
      var model = new List<pjt_recandidate_history>();
      ViewBag.request_user = 0;

      if (pic_seq != null && pic_seq.Count > 0)
        model = pr.SelectProjectReCanMakeupList(p_seq, pic_seq);
      else if (p_seq > 0 && c_seq > 0)
        model = pr.SelectProjectReCanMakeupList(p_seq, c_seq);

      return PartialView(model);
    }

    public async Task<PartialViewResult> MakeupGPT_MR(int mr_idx)
    {
      ProjectRepository pr = new ProjectRepository();
      var model = new List<pjt_recandidate_history>();
      ViewBag.request_user = 0;

      if (mr_idx > 0)
      {
        var makeup = await pr.SelectMakeupRequestOneAsync(mr_idx);
        if (makeup != null)
        {
          model = pr.SelectProjectReCanMakeupList(makeup.p_seq, makeup.c_seq);
          ViewBag.request_user = makeup.req_user;
        }
      }

      return PartialView("MakeupGPT", model);
    }

    // ─────────────────────────────────────────────────────────
    //  서버측 이력서 파일 → OpenAI Files API 업로드
    //  - PDF / DOCX / HWP 등 모든 파일 형식: OpenAI에 직접 업로드
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<ActionResult> ResumeMakeupUploadToOpenAI(int cr_seq, string api_key)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        var cresume = await cer.SelectCanResumeOneAsync(cr_seq);

        if (cresume == null)
          return Json(new { ok = false, message = "이력서 정보가 없습니다." });

        string filePath = cresume.file_origin_path;
        if (!System.IO.File.Exists(filePath))
          return Json(new { ok = false, message = "이력서 파일을 찾을 수 없습니다." });

        string ext = System.IO.Path.GetExtension(filePath).ToLower();

        string mimeType;
        switch (ext)
        {
          case ".pdf": mimeType = "application/pdf"; break;
          case ".docx": mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
          case ".doc": mimeType = "application/msword"; break;
          case ".hwp": mimeType = "application/x-hwp"; break;
          case ".hwpx": mimeType = "application/hwp+zip"; break;
          default: mimeType = "application/octet-stream"; break;
        }

        using (var httpClient = new HttpClient())
        {
          httpClient.Timeout = TimeSpan.FromSeconds(120);
          httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", api_key);

          using (var formContent = new MultipartFormDataContent())
          {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            var byteContent = new ByteArrayContent(fileBytes);
            byteContent.Headers.ContentType =
              new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);

            string uploadName = System.IO.Path.GetFileName(filePath);
            formContent.Add(byteContent, "file", uploadName);
            formContent.Add(new StringContent("user_data"), "purpose");

            var response = await httpClient.PostAsync("https://api.openai.com/v1/files", formContent);
            string respStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
              return Json(new { ok = false, message = "OpenAI 파일 업로드 실패: " + respStr });

            dynamic resultObj = JsonConvert.DeserializeObject(respStr);
            string file_id = (string)resultObj.id;

            return Json(new { ok = true, method = "file", file_id });
          }
        }
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = e.Message });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  OpenAI Files API 파일 삭제 (처리 완료 후 임시파일 정리)
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<ActionResult> ResumeMakeupDeleteFromOpenAI(string file_id, string api_key)
    {
      try
      {
        using (var httpClient = new HttpClient())
        {
          httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", api_key);
          await httpClient.DeleteAsync($"https://api.openai.com/v1/files/{file_id}");
        }
        return Json(new { ok = true });
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = e.Message });
      }
    }

    // ─────────────────────────────────────────────────────────
    //  Makeup DOCX 파일 생성 — 후보자 폴더 저장 버전
    //  저장 경로: ~/UploadedFiles/gpt_makeup/{c_seq}/
    // ─────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<JsonResult> MakeMakeup2(
      string makeup_model,
      string kor_name,
      int c_seq,
      string file_type,
      int request_user = 0)
    {
      try
      {
        var model = JsonConvert.DeserializeObject<MakeupCreateModel>(makeup_model);

        if (string.IsNullOrWhiteSpace(model.candidate))
          return Json(new { ok = false, message = "후보자 이름을 확인 할 수 없습니다." });

        string req_name = "미확인";
        if (request_user > 0)
        {
          AccountEntityRepository aer = new AccountEntityRepository();
          var user = aer.SelectUser(request_user);
          req_name = user != null ? user.name : AppIdentity.name;
        }
        else
        {
          req_name = AppIdentity.name;
        }

        string file_type_str = (file_type == "E") ? "E" : "";
        string template_name = (file_type == "E") ? "EN_makeup_blank.docx" : "KR_makeup_blank.docx";

        string fileName = string.Format("{0}-{1}{2}(by GPT).docx", req_name, kor_name, file_type_str);
        string samplePath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/makeupSample/"), template_name);
        string saveDirPath = Server.MapPath("~/UploadedFiles/gpt_makeup/" + c_seq);
        string file_dir = System.IO.Path.Combine(Utils.GetRootUrl(Request), saveDirPath);
        System.IO.Directory.CreateDirectory(file_dir);

        MakeupMaker maker = new MakeupMaker();
        (bool, string) result;
        if (file_type == "E")
          result = await maker.MakeEng(model, samplePath, file_dir, fileName);
        else
          result = await maker.Make(model, samplePath, file_dir, fileName);

        if (!result.Item1)
          throw new Exception(result.Item2);

        return Json(new
        {
          ok = true,
          file_url = Request.Url.GetLeftPart(UriPartial.Authority) + "/UploadedFiles/gpt_makeup/" + c_seq + "/" + result.Item2,
          file_name = result.Item2,
          message = ""
        });
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = e.Message });
      }
    }
  }
}
