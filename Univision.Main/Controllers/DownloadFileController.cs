using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Request.Board;
using Univision.Core.Models.DTO.Request.Candidate;
using Univision.Core.Models.DTO.Response;
using Univision.Core.Models.DTO.Response.Board;
using Univision.Core.Models.DTO.Response.Candidate;
using Univision.Core.Models.DTO.Response.Project;
using Univision.Core.Repositories;
using Univision.Main.Infrastructure;
using Univision.Main.Models.Api;
using Univision.Main.Models.Candidate;
using Univision.Main.Models.Project;
using Univision.Security;

namespace Univision.Main.Controllers
{
  public class DownloadFileController : BaseController
  {
    /// <summary>
    /// 경로에 파일이 있는지 찾기.
    /// </summary>
    /// <param name="element_seq"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpPost]
    public JsonResult ExistsFile(int element_seq = 0, string type = "")
    {
      string filePath = string.Empty;
      //string fileName = string.Empty;
      DownloadFileModel file_model = new DownloadFileModel();
      string url = string.Empty;

      if (type == "canResume")
      {
        CandidateRepository cr = new CandidateRepository();
        file_model = cr.SelectCanResumeFileInfo(element_seq);

        if (file_model != null)
        {
          filePath = file_model.file_origin_path;
          //fileName = file_model.file_name;
        }
      }
      else if (type == "client")
      {
        ClientRepository cr = new ClientRepository();
        filePath = cr.SelectClientFileInfo(element_seq);
      }
      else if (type == "contract")
      {
        ClientRepository cr = new ClientRepository();
        filePath = cr.SelectContractFileInfo(element_seq);
      }
      else if (type == "project")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectProjectFileInfo(element_seq);
      }
      else if (type == "board")
      {
        BoardRepository br = new BoardRepository();
        filePath = br.SelectBoardFileInfo(element_seq);
      }
      else if (type == "pjtRecHistory")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectProjectRecHisFileInfo(element_seq);
      }
      else if (type == "inorder")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectInOrderFileInfo(element_seq);
      }
      else if (type == "receipt")
      {
        MyListRepository mlr = new MyListRepository();
        filePath = mlr.SelectMobileReceiptFileInfo(element_seq);        
      }
      //string fullName = Server.MapPath("~" + filePath);

      FileInfo info = new FileInfo(filePath);
      if (!info.Exists)
        return Json(new { result = false });
      else
        return Json(new { result = true, url = "/DownloadFile/GetDownloadFile?element_seq=" + element_seq + "&type=" + type }); ;
    }

    /// <summary>
    /// 파일 다운로드
    /// </summary>
    /// <param name="element_seq"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public ActionResult GetDownloadFile(int element_seq = 0, string type = "")
    {
      string filePath = string.Empty;
      string fileName = string.Empty;
      DownloadFileModel file_model = new DownloadFileModel();
      string url = string.Empty;

      if (type == "canResume")
      {
        CandidateRepository cr = new CandidateRepository();
        file_model = cr.SelectCanResumeFileInfo(element_seq);

        if (file_model != null)
        {
          filePath = file_model.file_origin_path;
          fileName = file_model.file_name;
        }
      }
      else if (type == "client")
      {
        ClientRepository cr = new ClientRepository();
        filePath = cr.SelectClientFileInfo(element_seq);
      }
      else if (type == "contract")
      {
        ClientRepository cr = new ClientRepository();
        filePath = cr.SelectContractFileInfo(element_seq);
      }
      else if (type == "project")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectProjectFileInfo(element_seq);
      }
      else if (type == "board")
      {
        BoardRepository br = new BoardRepository();
        filePath = br.SelectBoardFileInfo(element_seq);
      }
      else if (type == "pjtRecHistory")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectProjectRecHisFileInfo(element_seq);
      }
      else if (type == "inorder")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectInOrderFileInfo(element_seq);
      }
      else if (type == "receipt")
      {
        MyListRepository mlr = new MyListRepository();
        filePath = mlr.SelectMobileReceiptFileInfo(element_seq);
        
      }

      //string fullName = Server.MapPath("~" + filePath);

      FileInfo info = new FileInfo(filePath);

      if (!info.Exists)
        return new HttpNotFoundResult(HttpUtility.UrlEncode("경로에서 파일을 찾을 수 없습니다.", System.Text.Encoding.UTF8).Replace("+", "%20"));

      if (String.IsNullOrEmpty(fileName))
        fileName = info.Name;

      byte[] fileBytes = GetFile(filePath);
      return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
    }


    public ActionResult Download(int element_seq = 0, string type = "")
    {
      string filePath = string.Empty;
      string fileName = string.Empty;
      DownloadFileModel file_model = new DownloadFileModel();
      string url = string.Empty;

      if (type == "canResume")
      {
        CandidateRepository cr = new CandidateRepository();
        file_model = cr.SelectCanResumeFileInfo(element_seq);

        if (file_model != null)
        {
          filePath = file_model.file_origin_path;
          fileName = file_model.file_name;
        }
      }
      else if (type == "client")
      {
        ClientRepository cr = new ClientRepository();
        filePath = cr.SelectClientFileInfo(element_seq);
      }
      else if (type == "contract")
      {
        ClientRepository cr = new ClientRepository();
        filePath = cr.SelectContractFileInfo(element_seq);
      }
      else if (type == "project")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectProjectFileInfo(element_seq);
      }
      else if (type == "board")
      {
        BoardRepository br = new BoardRepository();
        filePath = br.SelectBoardFileInfo(element_seq);
      }
      else if (type == "pjtRecHistory")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectProjectRecHisFileInfo(element_seq);
      }
      else if (type == "inorder")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectInOrderFileInfo(element_seq);
      }
      else if (type == "receipt")
      {
        MyListRepository mlr = new MyListRepository();
        filePath = mlr.SelectMobileReceiptFileInfo(element_seq);        
        
      }

      //string fullName = Server.MapPath("~" + filePath);

      FileInfo info = new FileInfo(filePath);

      if (!info.Exists)
        return new HttpNotFoundResult("파일을 찾을 수 없습니다.");
      if (String.IsNullOrEmpty(fileName))
        fileName = info.Name;

      byte[] fileBytes = GetFile(filePath);
      return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

    }


    public ActionResult DownloadFileToWord(int element_seq = 0, string type = "")
    {
      Spire.Pdf.License.LicenseProvider.SetLicenseKey("XbgnAQDxg0XYHgvSJ+3odG7syXqSPH2GsCw6CFr6JsqgIshAMr5pysViL41k+G7HkwIwcQvvv7/UFQUTf1jaA1la218ELXw5aZbRMgqdxfq6Mbsgkmn6N4mey6e/CACovGKq/b0IWPRr5Kt/eYzzsJZL0f315E+4fM3zV4evd+lWj9K+5rWPJwgH3mUgOuEGA6pMROO/p6mUtQ6i3Zy3FxUno6xsYVgskuP6XRVLAEx0ZOUlBgfh5J08/BYuPaoT5S1Mej3yv3nJCsrgcYi3q9/XsNH74ZuSEBY79AkrtJIihpL4wxHcJkzXA/giipWOe/MsTI3go7WOPtrwirVGJ0n22hJTa4jVf70au5jhtggJlnVEoDWpfUyCGbkDKKL2PYG0Aq5ogQRgrvLiAbjNW2iqmOzAZnROxFeJnTGscbGTr4bErw/HUroqmXTErqu86YPr6PMQB7xg/lfAtXif/IpovLvYeEjm+VqjcFaDlgq+T9pGH4kkloK8R0lXqHuVO6j96FBExSxRBnO8N5Ioq/cIpzc73pwcGAOwK740NTU0707Fl3PWXAB03o7rkh2ur+aq+ruJVfvOV0HrKGuM7uymrXEaxcg7jdkedOnD7SBxfOai9vRXPf+1bI6hih+miAVkimWCVrEG+jmbAPaKiDfbunr5L1/0EdJnSu4LaQeadi8eseJPN1ihReVWeXcet+j6fxCy/tqBLKEALabXGhPokq793nHQilnRJLna6YMMVCyX0TIjaLJ0MqOGNWlWOnNDfFq/XYqHAZ1fzsFVb7h0P3bbfRKXgPRo6TOr1iSK/mGlwuGeV7hzKxATeF3ZmHW7LBDCsqKJqLqQSqMY2ijoq9iNecQZ0YKqjXjKkJ6wT85hZz4QyYxwxy1X4QwRn8w6tavfBMwzl6a36yIavimAsBfJMlomk49kbbiQJkuZVlbzP10fzM1QsxVjTCQkMI9W4/mYwTnvWnHwWZOBixLHNOaor3/TTRt8Jq6d+hMetxj8hYhDI35pLJgHHZoKQ1wKcumTqYKn90swAjQSru8T5OWA5tZsoaN9qNII7IgHWdmzJssACqq2Pf3Ei+P5o/+mh27kLIjlZhlzW41/61tfvBWAIC+NrvBtGHPHpD/S0mvwY1QilCh6x7ZP3qvuGNvCcX/W/fB5SEhBXqMQhufMlXgk58i4LQrd9n7AnSwtsmTGino2HfIiTRh8eACz4zyr082Ir6lyTy4C9+eY5dE0JI6tcb5ke+59ceMgDYzb5mcrFGtcY5s4RgS9hgGBwLV0/dtIy+Hvcg+E1qRgnfuJYkEKzoxaTBmGXJLK+pE9kqpZSk6KOuzJLj82Rh/GbsQPueLJHm+Yw3CcEH9SRe/eHgNZrfJSE9uL4DMtxd5Vg8ppKfD2Cc6TzLyZEmAYLkjH5o5RqI9/wgSSnIfmZ0rwoJMaONnbIZeq6NJUWjnlNFYrxOC2mY9ikbLAMhEYj80fY7ueoiJijhqFblFWns6Y02fRjVjrMS5gtd0EEQ0m4gzAbTygMwdxWbVCJ1DpOWsMa25eiWTYCN9QlhYkL4qeLwU0g9u8tHh7kTCrkBM=");
      
      string filePath = string.Empty;
      string fileName = string.Empty;
      DownloadFileModel file_model = new DownloadFileModel();
      string url = string.Empty;

      if (type == "canResume")
      {
        CandidateRepository cr = new CandidateRepository();
        file_model = cr.SelectCanResumeFileInfo(element_seq);

        if (file_model != null)
        {
          filePath = file_model.file_origin_path;
          fileName = file_model.file_name;
        }
      }
      else if (type == "client")
      {
        ClientRepository cr = new ClientRepository();
        filePath = cr.SelectClientFileInfo(element_seq);
      }
      else if (type == "contract")
      {
        ClientRepository cr = new ClientRepository();
        filePath = cr.SelectContractFileInfo(element_seq);
      }
      else if (type == "project")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectProjectFileInfo(element_seq);
      }
      else if (type == "board")
      {
        BoardRepository br = new BoardRepository();
        filePath = br.SelectBoardFileInfo(element_seq);
      }
      else if (type == "pjtRecHistory")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectProjectRecHisFileInfo(element_seq);
      }
      else if (type == "inorder")
      {
        ProjectRepository pr = new ProjectRepository();
        filePath = pr.SelectInOrderFileInfo(element_seq);
      }
      else if (type == "receipt")
      {
        MyListRepository mlr = new MyListRepository();
        filePath = mlr.SelectMobileReceiptFileInfo(element_seq);

      }

      //string fullName = Server.MapPath("~" + filePath);
      PdfDocument doc = new PdfDocument();

      

      FileInfo info = new FileInfo(filePath);

      if (!info.Exists)
        return new HttpNotFoundResult("파일을 찾을 수 없습니다.");
      if (String.IsNullOrEmpty(fileName))
        fileName = info.Name;

      string converted_path = Path.GetDirectoryName(filePath) + @"\CONVERT\";
      string converted_name = Path.GetFileNameWithoutExtension(filePath);
      string converted_full = converted_path + converted_name + ".docx";
      
      doc.LoadFromFile(filePath);
      doc.ConvertOptions.SetPdfToDocOptions(true, true);
      FileInfo info2 = new FileInfo(converted_full);
      if (!info2.Exists)
      {
        doc.SaveToFile(converted_full, FileFormat.DOCX);
        doc.Dispose();
      }


      byte[] fileBytes = GetFile(converted_full);
      return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, converted_name + ".docx");

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