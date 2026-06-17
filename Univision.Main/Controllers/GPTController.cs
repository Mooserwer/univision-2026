using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using Univision.Main.Infrastructure;
using Univision.Main.Models.GPT;
using Univision.Security;
using Xceed.Document.NET;
using Xceed.Words.NET;



namespace Univision.Main.Controllers
{
  public class GPTController : BaseController
  {

    public ActionResult MakeupAi()
    {
      
      return View();
    }


    public async Task<PartialViewResult> MakeupGPT(int p_seq, List<int> pic_seq, int c_seq = 0)
    {
      ProjectRepository pr = new ProjectRepository();
      var model = new List<pjt_recandidate_history>();
      ViewBag.request_user = 0;
      if (pic_seq.Count > 0)
      {
        model = pr.SelectProjectReCanMakeupList(p_seq, pic_seq);
      }
      else if (p_seq > 0 && c_seq > 0)
      {
        model = pr.SelectProjectReCanMakeupList(p_seq, c_seq);
      }


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

    [HttpPost]
    public async Task<ContentResult> MakeupGptCall(List<GptMessage> messages)
    {
      try
      {
        string contentType   = Request.Headers["Content-Type"]; // Content-Type 헤더 값 가져오기
        string authorization = Request.Headers["Authorization"]; // Authorization 헤더 값 가져오기
        string apiKey = "";
        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        {
          // "Bearer " 이후의 부분을 추출
          apiKey = authorization.Substring("Bearer ".Length);
        }
        else
        {
          // Authorization 헤더가 없거나 Bearer로 시작하지 않으면 처리할 로직
          apiKey = ConfigurationManager.AppSettings["OpenAIApiKey"];
        }
        //const makeupApiKey = "";
        GPTServer gpt = new GPTServer(apiKey);
        var result = await gpt.GetCompletionAsyncRaw(messages);
        var settings = new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver(),
          Formatting = Newtonsoft.Json.Formatting.Indented // JSON 예쁘게 출력
        };

        string jsonResponse = JsonConvert.SerializeObject(result);

        //return Json(jsonResponse, JsonRequestBehavior.AllowGet); ;
        return Content(jsonResponse, "application/json");
      } 
      catch (Exception e)
      {
        // 오류 처리 후 ContentResult로 반환
        var errorResponse = new
        {
          ok = false,
          message = e.Message
        };

        // JSON 문자열로 직렬화하여 반환
        return Content(JsonConvert.SerializeObject(errorResponse), "application/json");
      }
    }

    [HttpPost]
    public async Task<ActionResult> MakeupGptCallStream(List<GptMessage> messages)
    {
      
      Response.ContentType = "application/json";
      Response.BufferOutput = false;


      string contentType = Request.Headers["Content-Type"]; // Content-Type 헤더 값 가져오기
      string authorization = Request.Headers["Authorization"]; // Authorization 헤더 값 가져오기
      string apiKey = "";
      if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
      {
        // "Bearer " 이후의 부분을 추출
        apiKey = authorization.Substring("Bearer ".Length);
      }
      else
      {
        // Authorization 헤더가 없거나 Bearer로 시작하지 않으면 처리할 로직
        apiKey = ConfigurationManager.AppSettings["OpenAIApiKey"];
      }
      //const makeupApiKey = "";
      GPTServer gpt = new GPTServer(apiKey);

      try
      {
        // API 스트리밍 데이터 받기
        using (var stream = await gpt.GetCompletionAsyncStream(messages))
        using (var reader = new StreamReader(stream))
        {
          using (var writer = new StreamWriter(Response.OutputStream, System.Text.Encoding.UTF8))
          {
            writer.AutoFlush = true; // 데이터를 실시간으로 전송하기 위해 AutoFlush 활성화
            char[] buffer = new char[1024];
            int bytesRead;
            while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
              // 클라이언트로 스트리밍 데이터 전송
              await writer.WriteAsync(new string(buffer, 0, bytesRead));
            }
          }
        }
      }
      catch (Exception ex)
      {
        // 오류 처리
        Response.StatusCode = 500;
        return Content($"Error: {ex.Message}");
      }

      return new EmptyResult();
      
    }

    /*
    [HttpPost]
    public async Task<JsonResult> MakeMakeup(string makeup_model, int c_seq, string file_type, int request_user = 0)
    {
      try
      {
        var model = JsonConvert.DeserializeObject<MakeupCreateModel>(makeup_model);

        if (string.IsNullOrWhiteSpace(model.candidate))
          return Json(new
          {
            ok = false
          ,
            message = "후보자 이름을 확인 할 수 없습니다."
          });
        string req_name = "미확인";
        if (request_user > 0)
        {
          AccountEntityRepository aer = new AccountEntityRepository();
          var user = aer.SelectUser(request_user);
          if (user == null)
          {
            req_name = AppIdentity.name;
          }
          else
          {
            req_name = user.name;
          }

        }
        else
        {
          req_name = AppIdentity.name;
        }
        string file_type_str = "";
        if (file_type == "E")
        {
          file_type_str = "ENG_";
        }
        else
        {
          file_type_str = "국문_";
        }

        string fileName = string.Format("{0}{1}-{2}(by GPT).docx", file_type_str, req_name, model.candidate.Replace(" ", ""));
        string renameFileName = fileName;
        string fileNameOnly = System.IO.Path.GetFileNameWithoutExtension(fileName);
        string extension = System.IO.Path.GetExtension(fileName);
        string samplePath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/makeupSample/"), "KR_makeup.docx");
        string path = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/Candidate/" + c_seq));
        string file_dir = System.IO.Path.Combine(Utils.GetRootUrl(Request), path);
        string saveFilePath = System.IO.Path.Combine(path, fileName);

        using (var document = DocX.Load(samplePath))
        {
          var newDoc = document.Copy();


          foreach (var table in newDoc.Tables)
          {
            foreach (var row in table.Rows)
            {
              foreach (var cell in row.Cells)
              {
                foreach (var paragraph in cell.Paragraphs)
                {
                  if (paragraph.Text.Contains("{{candidate}}"))
                    paragraph.ReplaceText("{{candidate}}", model.candidate);

                }
              }
            }
          }

          //foreach (var paragraph in newDoc.Paragraphs)
          for (int i = 0; i < newDoc.Paragraphs.Count; i++)
          {
            if (newDoc.Paragraphs[i].Text.Contains("{{yob}}"))
            {
              newDoc.Paragraphs[i].ReplaceText("{{yob}}", model.yob);
            }
            else if (newDoc.Paragraphs[i].Text.Contains("{{gender}}"))
            {
              newDoc.Paragraphs[i].ReplaceText("{{gender}}", model.gender);
            }
            else if (newDoc.Paragraphs[i].Text.Contains("{{addr}}"))
            {
              newDoc.Paragraphs[i].ReplaceText("{{addr}}", model.addr);
            }
            else if (newDoc.Paragraphs[i].Text.Contains("{{education}}"))
            {
              if (model.education.Count > 0)
              {
                Paragraph prevParagraph = null;
                foreach (var edu in model.education)
                {
                  Paragraph p = null;
                  if (prevParagraph == null)
                  {
                    p = newDoc.Paragraphs[i].InsertParagraphAfterSelf("");
                  }
                  else
                  {
                    p = prevParagraph.InsertParagraphAfterSelf("");
                  }

                  if (!String.IsNullOrEmpty(edu.school))
                    p.Append(edu.school);
                  if (!String.IsNullOrEmpty(edu.area))
                    p.Append("(" + edu.area + ")");
                  if (!String.IsNullOrEmpty(edu.major))
                    p.Append(" " + edu.major);
                  if (!String.IsNullOrEmpty(edu.is_grdt))
                    p.Append(" " + edu.is_grdt);
                  if (!String.IsNullOrEmpty(edu.grade))
                    p.Append("(" + edu.grade + ")");


                  int byteLength = System.Text.Encoding.UTF8.GetByteCount(p.Text.Replace("\t", ""));
                  int tabCount = 11;
                  if (byteLength > 0)
                  {
                    byteLength = (int)(byteLength / 3.0 * 2);
                    tabCount = tabCount - byteLength / 7;
                  }

                  if (!string.IsNullOrEmpty(edu.ad_yyyymm))
                  {
                    p.Append(new string('\t', tabCount) + edu.ad_yyyymm);
                    if (!string.IsNullOrEmpty(edu.gdt_yyyymm))
                    {
                      p.Append(" – " + edu.gdt_yyyymm);
                    }
                  }
                  else
                  {
                    if (!string.IsNullOrEmpty(edu.gdt_yyyymm))
                    {
                      p.Append(new string('\t', tabCount) + edu.gdt_yyyymm);
                    }
                  }

                  prevParagraph = p;
                }
              }
              newDoc.Paragraphs[i].Remove(false);
            }
            else if (newDoc.Paragraphs[i].Text.Contains("{{core}}"))
            {
              if (model.core.Count > 0)
              {
                Paragraph prevParagraph = null;
                foreach (var core in model.core)
                {
                  if (!String.IsNullOrEmpty(core))
                  {
                    Paragraph p = null;
                    if (prevParagraph == null)
                    {
                      p = newDoc.Paragraphs[i].InsertParagraphAfterSelf(core);
                    }
                    else
                    {
                      p = prevParagraph.InsertParagraphAfterSelf(core);
                    }
                    p.StyleName = "Blt1";

                    prevParagraph = p;
                  }
                }
              }
              newDoc.Paragraphs[i].Remove(false);
            }
            else if (newDoc.Paragraphs[i].Text.Contains("{{career}}"))
            {
              if (model.career.Count > 0)
              {
                Paragraph prevParagraph = null;
                foreach (var career in model.career)
                {
                  Paragraph p = null;
                  if (prevParagraph == null)
                  {
                    p = newDoc.Paragraphs[i].InsertParagraphAfterSelf("");
                  }
                  else
                  {
                    p = prevParagraph.InsertParagraphAfterSelf("");
                  }

                  if (!String.IsNullOrEmpty(career.company))
                    p.Append(career.company);
                  if (!String.IsNullOrEmpty(career.area))
                    p.Append(", " + career.area);

                  int byteLength = System.Text.Encoding.UTF8.GetByteCount(p.Text.Replace("\t", ""));
                  int tabCount = 11;
                  if (byteLength > 0)
                  {
                    byteLength = (int)(byteLength / 3.0 * 2);
                    tabCount = tabCount - byteLength / 7;
                  }

                  if (!string.IsNullOrEmpty(career.j_yyyymm))
                  {
                    p.Append(new string('\t', tabCount) + career.j_yyyymm);
                    if (!string.IsNullOrEmpty(career.r_yyyymm))
                    {
                      p.Append(" – " + career.r_yyyymm);
                    }
                  }
                  else
                  {
                    if (!string.IsNullOrEmpty(career.r_yyyymm))
                    {
                      p.Append(new string('\t', tabCount) + career.r_yyyymm);
                    }
                  }
                  p.StyleName = "Bold1";

                  if (!string.IsNullOrEmpty(career.info))
                  {
                    p = p.InsertParagraphAfterSelf(career.info);
                  }

                  p = p.InsertParagraphAfterSelf("");
                  if (!string.IsNullOrEmpty(career.dept))
                  {
                    p.Append(career.dept);
                  }
                  if (!string.IsNullOrEmpty(career.pos))
                  {
                    p.Append(" / " + career.pos);
                  }
                  if (!string.IsNullOrEmpty(career.is_leader) && career.is_leader == "Y")
                  {
                    p.Append("(팀장)");
                  }
                  p.StyleName = "Bold1";

                  foreach (var depth1 in career.desc)
                  {
                    p = p.InsertParagraphAfterSelf("");
                    if (!string.IsNullOrEmpty(depth1.desc))
                    {
                      p.Append(depth1.desc);
                    }
                    p.StyleName = "Blt1";

                    foreach (var depth2 in depth1.depth)
                    {
                      p = p.InsertParagraphAfterSelf("");
                      if (!string.IsNullOrEmpty(depth2.desc))
                      {
                        p.Append(depth2.desc);
                      }
                      p.StyleName = "Blt2";

                      foreach (var depth3 in depth2.depth)
                      {
                        p = p.InsertParagraphAfterSelf("");
                        if (!string.IsNullOrEmpty(depth3.desc))
                        {
                          p.Append(depth3.desc);
                        }
                        p.StyleName = "Blt3";

                        foreach (var depth4 in depth3.depth)
                        {
                          p = p.InsertParagraphAfterSelf("");
                          if (!string.IsNullOrEmpty(depth4.desc))
                          {
                            p.Append(depth4.desc);
                          }
                          p.StyleName = "Blt4";
                        }
                      }
                    }

                  }


                  if (!string.IsNullOrEmpty(career.r_reason))
                  {
                    p = p.InsertParagraphAfterSelf("");
                    p.Append("[이직사유] " + career.r_reason);
                  }
                  p = p.InsertParagraphAfterSelf("");
                  prevParagraph = p;
                }
              }
              newDoc.Paragraphs[i].Remove(false);
            }
            else if (newDoc.Paragraphs[i].Text.Contains("{{certifications}}"))
            {
              if (model.certifications.Count > 0)
              {
                Paragraph prevParagraph = null;
                foreach (var certification in model.certifications)
                {
                  if (!String.IsNullOrEmpty(certification.name))
                  {
                    var p = newDoc.Paragraphs[i].InsertParagraphAfterSelf("");
                    if (!string.IsNullOrEmpty(certification.name))
                    {
                      p.Append(certification.name);
                    }
                    if (!string.IsNullOrEmpty(certification.gov))
                    {
                      p.Append(" – " + certification.gov);
                    }
                    if (!string.IsNullOrEmpty(certification.year))
                    {
                      p.Append("(" + certification.year + ")");
                    }

                    p.StyleName = "Blt1";

                    prevParagraph = p;
                  }
                }
              }
              newDoc.Paragraphs[i].Remove(false);
            }
            else if (newDoc.Paragraphs[i].Text.Contains("{{learns}}"))
            {
              if (model.learns.Count > 0)
              {
                Paragraph prevParagraph = null;
                foreach (var learn in model.learns)
                {
                  if (!String.IsNullOrEmpty(learn.name))
                  {
                    var p = newDoc.Paragraphs[i].InsertParagraphAfterSelf("");
                    if (!string.IsNullOrEmpty(learn.name))
                    {
                      p.Append(learn.name);
                    }
                    if (!string.IsNullOrEmpty(learn.gov))
                    {
                      p.Append(" – " + learn.gov);
                    }
                    if (!string.IsNullOrEmpty(learn.year1))
                    {
                      p.Append("(" + learn.year1);
                      if (!string.IsNullOrEmpty(learn.year2))
                      {
                        p.Append(" – " + learn.year2);
                      }
                      p.Append(")");
                    }

                    p.StyleName = "Blt1";

                    prevParagraph = p;
                  }
                }
              }
              newDoc.Paragraphs[i].Remove(false);
            }
            else if (newDoc.Paragraphs[i].Text.Contains("{{skills}}"))
            {
              if (model.skills.Count > 0)
              {
                Paragraph prevParagraph = null;
                foreach (var skill in model.skills)
                {
                  if (!String.IsNullOrEmpty(skill.name))
                  {
                    var p = newDoc.Paragraphs[i].InsertParagraphAfterSelf("");
                    p.StyleName = "Blt1";
                    if (!string.IsNullOrEmpty(skill.name))
                    {
                      p.Append(skill.name);
                    }
                    if (!string.IsNullOrEmpty(skill.desc))
                    {
                      if ("상중하".Contains(skill.desc))
                      {
                        p.Append("(" + skill.desc + ")");
                      }
                      else
                      {
                        p = p.InsertParagraphAfterSelf(skill.desc);
                        p.StyleName = "Blt2";
                      }
                    }
                    else
                    {
                      p.StyleName = "Blt1";
                    }

                    prevParagraph = p;
                  }
                }
              }
              newDoc.Paragraphs[i].Remove(false);
            }
            else if (newDoc.Paragraphs[i].Text.Contains("{{languages}}"))
            {
              if (model.languages.Count > 0)
              {
                Paragraph prevParagraph = null;
                foreach (var language in model.languages)
                {
                  if (!String.IsNullOrEmpty(language.language))
                  {
                    var p = newDoc.Paragraphs[i].InsertParagraphAfterSelf("");
                    if (!string.IsNullOrEmpty(language.language))
                    {
                      p.Append(language.language);
                    }
                    if (!string.IsNullOrEmpty(language.level))
                    {
                      if ("상중하".Contains(language.level))
                      {
                        p.Append("(" + language.level + ")");
                        p.StyleName = "Blt1";
                      }
                      else
                      {
                        p = p.InsertParagraphAfterSelf(language.level);
                        p.StyleName = "Blt2";
                      }
                    }
                    else
                    {
                      p.StyleName = "Blt1";
                    }

                    prevParagraph = p;
                  }
                }
              }
              newDoc.Paragraphs[i].Remove(false);
            }
            else if (newDoc.Paragraphs[i].Text.Contains("{{etcs}}"))
            {
              if (model.etcs.Count > 0)
              {
                Paragraph prevParagraph = null;
                foreach (var etc in model.etcs)
                {
                  if (!String.IsNullOrEmpty(etc))
                  {
                    var p = newDoc.Paragraphs[i].InsertParagraphAfterSelf(etc);
                    p.StyleName = "Blt1";

                    prevParagraph = p;
                  }
                }
              }
              newDoc.Paragraphs[i].Remove(false);
            }

          }






          int count = 1;
          while (System.IO.File.Exists(saveFilePath))
          {
            renameFileName = $"{fileNameOnly}({count}){extension}";
            saveFilePath = System.IO.Path.Combine(path, renameFileName);
            count++;
          }

          newDoc.SaveAs(saveFilePath);
        }



        return Json(new
        {
          ok = true
          ,
          file_url = Request.Url.GetLeftPart(UriPartial.Authority) + "/UploadedFiles/candidate/" + c_seq + "/" + renameFileName
          ,
          file_name = renameFileName
          ,
          message = ""
        }); ;

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
          ,
          message = e.Message
        }); ;
      }
    }
    */
    [HttpPost]
    public async Task<JsonResult> MakeMakeup2026(string makeup_model, string file_name, string file_type)
    {
      try
      {
        var model = JsonConvert.DeserializeObject<MakeupCreateModel>(makeup_model);

        if (string.IsNullOrWhiteSpace(model.candidate))
          return Json(new
          {
            ok = false
          ,
            message = "후보자 이름을 확인 할 수 없습니다."
          });

        string req_name = "미확인";
        req_name = AppIdentity.name;
        
        string file_type_str = "";
        string template_filename = "KR_makeup_blank.docx";
        
        if (file_type == "E")
        {
          file_type_str = "E";
          template_filename = "EN_makeup_blank.docx";
        }
        else
        {
          file_type_str = "";
          //can_name = model.candidate.Replace(" ", "");
        }

        string fileName = string.Format("{0}_{1}_{2}(by GPT).docx", file_name, file_type_str, req_name);
        string renameFileName = fileName;

        string samplePath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/makeupSample/"), template_filename);
        string path = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/gpt_makeup_2026/"));
        string file_dir = System.IO.Path.Combine(Utils.GetRootUrl(Request), path);
        Directory.CreateDirectory(file_dir);
        //string saveFilePath = Path.Combine(path, fileName);

        MakeupMaker maker = new MakeupMaker();
        (bool, string) result;
        if (file_type == "E")
          result = await maker.MakeEng(model, samplePath, file_dir, fileName);
        else
          result = await maker.Make(model, samplePath, file_dir, fileName);

        if (result.Item1 == false)
        {
          throw new Exception(result.Item2);
        }

        return Json(new
        {
          ok = true
          ,
          file_url = Request.Url.GetLeftPart(UriPartial.Authority) + "/UploadedFiles/gpt_makeup_2026/" + result.Item2
          ,
          file_name = result.Item2
          ,
          message = ""
        }); ;
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = e.Message
        });
      }
    }
    [HttpPost]
    public async Task<JsonResult> MakeMakeup2(string makeup_model, string kor_name, int c_seq, string file_type, int request_user = 0)
    {
      try
      {
        var model = JsonConvert.DeserializeObject<MakeupCreateModel>(makeup_model);

        if (string.IsNullOrWhiteSpace(model.candidate))
          return Json(new
          {
            ok = false
          ,
            message = "후보자 이름을 확인 할 수 없습니다."
          });

        string req_name = "미확인";
        if (request_user > 0)
        {
          AccountEntityRepository aer = new AccountEntityRepository();
          var user = aer.SelectUser(request_user);
          if (user == null)
          {
            req_name = AppIdentity.name;
          }
          else
          {
            req_name = user.name;
          }

        }
        else
        {
          req_name = AppIdentity.name;
        }
        string file_type_str = "";
        string template_filename = "KR_makeup_blank.docx";
        string can_name = kor_name;
        if (file_type == "E")
        {
          file_type_str = "E";
          template_filename = "EN_makeup_blank.docx";
        }
        else
        {
          file_type_str = "";
          //can_name = model.candidate.Replace(" ", "");
        }

        string fileName = string.Format("{0}-{1}{2}(by GPT).docx", req_name, kor_name, file_type_str);
        string renameFileName = fileName;

        string samplePath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/makeupSample/"), template_filename);
        string path = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/gpt_makeup/" + c_seq));
        string file_dir = System.IO.Path.Combine(Utils.GetRootUrl(Request), path);
        Directory.CreateDirectory(file_dir);
        //string saveFilePath = Path.Combine(path, fileName);

        MakeupMaker maker = new MakeupMaker();
        (bool, string) result;
        if (file_type == "E")
          result = await maker.MakeEng(model, samplePath, file_dir, fileName);
        else
          result = await maker.Make(model, samplePath, file_dir, fileName);

        if (result.Item1 == false)
        {
          throw new Exception(result.Item2);
        }

        return Json(new
        {
          ok = true
          ,
          file_url = Request.Url.GetLeftPart(UriPartial.Authority) + "/UploadedFiles/gpt_makeup/" + c_seq + "/" + result.Item2
          ,
          file_name = result.Item2
          ,
          message = ""
        }); ;
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = e.Message
        });
      }
    }

    //AI분석을 위한 메이크업 파일 내용 추출
    [HttpPost]
    public async Task<ActionResult> ResumeMakeupGetContents(int cr_seq)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        CandidateEntityRepository cer = new CandidateEntityRepository();

        var cresume = await cer.SelectCanResumeOneAsync(cr_seq);

        SearchEngineRepository ser = new SearchEngineRepository();


        if (cresume == null)
        {
          throw (new Exception("이력서 정보가 없습니다."));
        }
        string file_content = "";

        var creator = GetCreatorFromPdf(cresume.file_origin_path);

        if (cresume.file_extension == ".pdf" && creator != "react-pdf")
        {
          file_content = ExtractTextFromPdf(cresume.file_origin_path);
        }
        else
        {
          //FileUpload fiUpload = new FileUpload();
#if DEBUG
          string source = "/media/uploadfiles/kmpdl/20250313.docx";
#else
          string new_file_name = fiUpload.CopyTempFileForKMPDL(cresume.file_origin_path, Utils.ReturnUniqueValue(AppIdentity.user_seq, "F"));
          string source = "/media/uploadfiles/kmpdl/" + new_file_name;
#endif
          file_content = ser.TempResumeCheck(source);
        }

        //fiUpload.DeleteTempFileForKMPDL(new_file_name);

        return Json(new
        {
          ok = true,
          message = "Check Complete.",
          result = cresume,
          file_content = file_content
        });

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
    public string GetCreatorFromPdf(string path)
    {
      try
      {
        using (PdfReader reader = new PdfReader(path))
        {
          string creator = reader.Info["Creator"];

          return creator;
        }

      }
      catch
      {
        return "";
      }
    }
    public string ExtractTextFromPdf(string path)
    {
      try
      {
        using (PdfReader reader = new PdfReader(path))
        {
          string text = "";
          ITextExtractionStrategy Strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();

          for (int i = 1; i <= reader.NumberOfPages; i++)
          {
            text += (PdfTextExtractor.GetTextFromPage(reader, i, Strategy));
          }
          return text;
        }
        
      } 
      catch
      {
        return "";
      }
    }
  }
}