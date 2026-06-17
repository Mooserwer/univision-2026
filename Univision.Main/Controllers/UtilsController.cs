using Newtonsoft.Json;
using Spire.Pdf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Univision.Core.Repositories;
using Univision.Main.Infrastructure;
using Univision.Main.Models.GPT;
using Univision.Security;
using Xceed.Document.NET;
using Xceed.Words.NET;


namespace Univision.Main.Controllers
{
  public class UtilsController : BaseController
  {
    public async Task<PartialViewResult> PDFtoWord()
    {      
      return PartialView();
    }

    //AI분석을 위한 파일 업로드
    [HttpPost]
    public ActionResult PDFtoWord(HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        SearchEngineRepository ser = new SearchEngineRepository();
        string path = Server.MapPath("~/UploadedFiles");
        var rst = fiUpload.UploadTemp(path, "PDFCONVERT", "UPLOAD", file);

        if (!rst.status)
        {
          throw (new Exception(rst.statusMessage));
        }

        Spire.Pdf.License.LicenseProvider.SetLicenseKey("XbgnAQDxg0XYHgvSJ+3odG7syXqSPH2GsCw6CFr6JsqgIshAMr5pysViL41k+G7HkwIwcQvvv7/UFQUTf1jaA1la218ELXw5aZbRMgqdxfq6Mbsgkmn6N4mey6e/CACovGKq/b0IWPRr5Kt/eYzzsJZL0f315E+4fM3zV4evd+lWj9K+5rWPJwgH3mUgOuEGA6pMROO/p6mUtQ6i3Zy3FxUno6xsYVgskuP6XRVLAEx0ZOUlBgfh5J08/BYuPaoT5S1Mej3yv3nJCsrgcYi3q9/XsNH74ZuSEBY79AkrtJIihpL4wxHcJkzXA/giipWOe/MsTI3go7WOPtrwirVGJ0n22hJTa4jVf70au5jhtggJlnVEoDWpfUyCGbkDKKL2PYG0Aq5ogQRgrvLiAbjNW2iqmOzAZnROxFeJnTGscbGTr4bErw/HUroqmXTErqu86YPr6PMQB7xg/lfAtXif/IpovLvYeEjm+VqjcFaDlgq+T9pGH4kkloK8R0lXqHuVO6j96FBExSxRBnO8N5Ioq/cIpzc73pwcGAOwK740NTU0707Fl3PWXAB03o7rkh2ur+aq+ruJVfvOV0HrKGuM7uymrXEaxcg7jdkedOnD7SBxfOai9vRXPf+1bI6hih+miAVkimWCVrEG+jmbAPaKiDfbunr5L1/0EdJnSu4LaQeadi8eseJPN1ihReVWeXcet+j6fxCy/tqBLKEALabXGhPokq793nHQilnRJLna6YMMVCyX0TIjaLJ0MqOGNWlWOnNDfFq/XYqHAZ1fzsFVb7h0P3bbfRKXgPRo6TOr1iSK/mGlwuGeV7hzKxATeF3ZmHW7LBDCsqKJqLqQSqMY2ijoq9iNecQZ0YKqjXjKkJ6wT85hZz4QyYxwxy1X4QwRn8w6tavfBMwzl6a36yIavimAsBfJMlomk49kbbiQJkuZVlbzP10fzM1QsxVjTCQkMI9W4/mYwTnvWnHwWZOBixLHNOaor3/TTRt8Jq6d+hMetxj8hYhDI35pLJgHHZoKQ1wKcumTqYKn90swAjQSru8T5OWA5tZsoaN9qNII7IgHWdmzJssACqq2Pf3Ei+P5o/+mh27kLIjlZhlzW41/61tfvBWAIC+NrvBtGHPHpD/S0mvwY1QilCh6x7ZP3qvuGNvCcX/W/fB5SEhBXqMQhufMlXgk58i4LQrd9n7AnSwtsmTGino2HfIiTRh8eACz4zyr082Ir6lyTy4C9+eY5dE0JI6tcb5ke+59ceMgDYzb5mcrFGtcY5s4RgS9hgGBwLV0/dtIy+Hvcg+E1qRgnfuJYkEKzoxaTBmGXJLK+pE9kqpZSk6KOuzJLj82Rh/GbsQPueLJHm+Yw3CcEH9SRe/eHgNZrfJSE9uL4DMtxd5Vg8ppKfD2Cc6TzLyZEmAYLkjH5o5RqI9/wgSSnIfmZ0rwoJMaONnbIZeq6NJUWjnlNFYrxOC2mY9ikbLAMhEYj80fY7ueoiJijhqFblFWns6Y02fRjVjrMS5gtd0EEQ0m4gzAbTygMwdxWbVCJ1DpOWsMa25eiWTYCN9QlhYkL4qeLwU0g9u8tHh7kTCrkBM=");

        string fileName = string.Empty;

        string url = string.Empty;


        //string fullName = Server.MapPath("~" + filePath);
        PdfDocument doc = new PdfDocument();

        FileInfo info = new FileInfo(rst.filePath);

        if (!info.Exists)
          return new HttpNotFoundResult("파일을 찾을 수 없습니다.");
        if (String.IsNullOrEmpty(fileName))
          fileName = info.Name;

        string converted_path = Path.GetDirectoryName(rst.filePath) + @"\CONVERT\";
        string converted_name = Path.GetFileNameWithoutExtension(rst.filePath);
        string converted_full = converted_path + converted_name + ".docx";

        doc.LoadFromFile(rst.filePath);
        doc.ConvertOptions.SetPdfToDocOptions(true, true);
        FileInfo info2 = new FileInfo(converted_full);
        if (!info2.Exists)
        {
          doc.SaveToFile(converted_full, FileFormat.DOCX);

          System.IO.File.Delete(rst.filePath);
          

          doc.Dispose();
        }

        byte[] fileBytes = GetFile(converted_full);
        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, converted_name + ".docx");

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Error]알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }

    }


    byte[] GetFile(string s)
    {
      try
      {
        System.IO.FileStream fs = System.IO.File.OpenRead(s);
        byte[] data = new byte[fs.Length];
        int br = fs.Read(data, 0, data.Length);
        if (br != fs.Length)
          throw new System.IO.IOException(s);
        return data;
      }
      catch (Exception)
      {

        throw;
      }

    }


  }
}