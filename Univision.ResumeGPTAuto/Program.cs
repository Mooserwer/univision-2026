using GPT.Core;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using Univision.Core.Repositories.MailResume;
using Univision.Main.Models.Candidate;

namespace Univision.ResumeGPTAuto
{
  class Program
  {

    private static string isResumeSysPrompt = getPrompt("./Prompt/is_resume.txt");
    private static string resumeToJsonPrompt = getPrompt("./Prompt/resume_organize.txt");
    private static string BusiToJsonPrompt = getPrompt("./Prompt/resume_organize_busi.txt");
    private static string JobToJsonPrompt = getPrompt("./Prompt/resume_organize_job.txt");
    private static bool stop = false;
    private const int sleep_ms = 60000;

    static async Task Main(string[] args)
    {
      Console.WriteLine("시작");
#if DEBUG
      Console.WriteLine(isResumeSysPrompt);
      Console.WriteLine(resumeToJsonPrompt);
#endif
      try
      {
        if (isResumeSysPrompt == null || resumeToJsonPrompt == null)
        {
          throw new Exception("프롬프트 없음");
        }
        //var program = new Program();
        Logger.EnableConsoleOutput = true;
        Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);

        while (true)

        {

          Logger.WriteLine("Start : 이력서 자동저장");
          await Work();
          Logger.WriteLine("End : 이력서 자동저장 ");



          if (stop)

          {

            break;

          }



          Thread.Sleep(sleep_ms);

        }


      }
      catch (Exception e)
      {
        Console.WriteLine("Exception: " + e.Message);
      }
#if DEBUG
      //Console.ReadLine();
#endif
    }

    protected static void myHandler(object sender, ConsoleCancelEventArgs args)

    {

      args.Cancel = true;

      stop = true;

    }

    public static string getPrompt(string path)
    {
      try
      {
        //Pass the file path and file name to the StreamReader constructor
        StreamReader sr = new StreamReader(path);
        //Read the first line of text
        string line = sr.ReadLine();
        string txt = String.Empty;
        txt = line;
        //Continue to read until you reach end of file
        while (line != null)
        {
          //write the line to console window
          //Console.WriteLine(txt);
          //Read the next line
          line = sr.ReadLine();
          txt += "\n" + line;
        }
        //close the file
        sr.Close();

        return txt;
      }
      catch (Exception e)
      {
        Console.WriteLine("Exception: " + e.Message);
        return null;
      }
    }

    // 작업을 수행하는 메서드
    static async Task Work()
    {
      var gpt = new GPTControl();
      Console.WriteLine(gpt.GetAPIKey());

      var mailResumeRepo = new MailResumeRepository();
      var mailResumeEnRepo = new MailResumeEntityRepository();
      try
      {

        // 처리할 준비가 된 메일 이력서 목록을 가져옴
        var mails = await mailResumeRepo.SelectReadyMailResumeListAsync(10, "DESC");
        //var mails = await mailResumeRepo.SelectReadyMailResumeListAsyncDebug(100, "ASC");

        // 처리할 메일이 있는 경우
        if (mails.Count > 0)
        {
          int i = 0;
          foreach (var mail in mails)
          {
            await Logger.WriteLineAsync("\t메일: (" + (++i) + ")" + mail.dv_snd_name + ", " + mail.dv_snd_addr + ", " + mail.dd_rcv.ToString());

            // 메일에서 이력서 파일 목록을 가져옴
            var resumes = await mailResumeRepo.SelectReadyMailResumeFileListAsync(mail.dv_timestamp);

            bool is_organize = false;
            // 각 이력서 파일을 처리
            if (resumes.Count > 0)
            {

              foreach (var resume in resumes)
              {
                await Logger.WriteLineAsync("\t\t파일: " + resume.dv_file_name);
                string ext = Path.GetExtension(resume.file_path2).ToLower();
                if (ext == null || ext == ".xls" || ext == ".xlsx")
                {
                  await Logger.WriteLineAsync("\t\t엑셀파일은 읽지 않습니다.");
                  continue;
                }
                // 파일 내용을 읽음
                string fileContents = await ResumeGetContents(resume.file_path2);

                if (!string.IsNullOrEmpty(fileContents))
                {
                  fileContents = @"메일주소 : " + mail.dv_snd_addr + "\n" + fileContents;
                  var messages = new List<Message>
                                    {
                                        new Message
                                        {
                                            Role = "system",
                                            Content = isResumeSysPrompt
                                        },
                                        new Message
                                        {
                                            Role = "user",
                                            Content = fileContents
                                        }
                                    };
                  try
                  {
                    // GPT를 사용하여 파일이 이력서인지 판단
                    string isResumeResult = await gpt.GetCompletionAsync(messages);
                    await Logger.WriteLineAsync("\t\t이력서 여부: " + isResumeResult);

                    if (isResumeResult.Trim().Equals("Y", StringComparison.OrdinalIgnoreCase))
                    {
                      //국영문 여부 저장하기
                      try
                      {
                        var res = await mailResumeEnRepo.SelectMailResumeFileOneAsync(resume.dv_timestamp, (int)resume.dn_seq);
                        if (res != null)
                        {
                          if (IsMostlyHangul(fileContents))
                          {
                            res.resume_type = "K";
                          }
                          else
                          {
                            res.resume_type = "E";
                          }
                          await mailResumeEnRepo.CreateOrUpdateMailResumeFileOneAsync(res, "U");
                          await Logger.WriteLineAsync("\t\t이력서 언어 : " + res.resume_type + " 저장 완료");
                        }
                      }
                      catch (Exception e)
                      {
                        Logger.WriteLine("\t\t이력서 언어 저장 실패" + e.Message, LogLevel.Error);
                      }

                      if (is_organize)
                      {
                        //이미 내용분석 작업이 끝났으면 다음 파일로 이동
                        continue;
                      }

                      var messages2 = new List<Message>
                                    {
                                        new Message
                                        {
                                            Role = "system",
                                            Content = resumeToJsonPrompt
                                        },
                                        new Message
                                        {
                                            Role = "user",
                                            Content = fileContents
                                        }
                                    };
                      Logger.WriteLine("\t\t내용분석중..");
                      //GPT를 사용하여 이력서 데이터 분류 작업 시작
                      string resumeJsonResult = await gpt.GetCompletionAsync(messages2);
                      Logger.WriteLine(resumeJsonResult);
                      //분류된 이력서 데이터 검증
                      try
                      {
                        mail_resume_gpt gpt_result = new mail_resume_gpt();
                        gpt_result.gpt_result = resumeJsonResult.Replace("`json", "").Replace("`", "");

                        var model = JsonConvert.DeserializeObject<CandidateCreateModel>(gpt_result.gpt_result, new JsonSerializerSettings
                        {
                          Error = HandleDeserializationError
                        });

                        gpt_result.gpt_result = JsonConvert.SerializeObject(model);

                        //추출된 메일 주소가 없으면 수신메일로 지정
                        if (String.IsNullOrEmpty(model.email1))
                        {
                          model.email1 = mail.dv_snd_addr;
                        }
                        if (!String.IsNullOrEmpty(model.cell_phone) || !String.IsNullOrEmpty(model.email1))
                        {
                          var dup_c_seq = await FindDuplicateCandidate(model.cell_phone, model.email1);
                          if (dup_c_seq > 0)
                          {
                            Logger.WriteLine("\t\t\t중복 후보자 있음 : " + dup_c_seq.ToString());
                          }
                          else
                          {
                            if (mail.dv_cd_no.HasValue)
                            {
                              dup_c_seq = mail.dv_cd_no.Value;
                              //Logger.WriteLine("\t\t\t중복 후보자 있음 : " + dup_c_seq.ToString());
                            }
                          }
                          gpt_result.c_seq = dup_c_seq;
                        }
                        if ((!gpt_result.c_seq.HasValue || gpt_result.c_seq.Value == 0) && model.companyList.Count > 0)
                        {
                          try
                          {
                            string company_name = model.companyList[0].company_name;
                            string dept_pos = model.companyList[0].division_name;
                            var messages3 = new List<Message>
                                    {
                                        new Message
                                        {
                                            Role = "system",
                                            Content = BusiToJsonPrompt
                                        },
                                        new Message
                                        {
                                            Role = "user",
                                            Content = company_name
                                        }
                                    };

                            Logger.WriteLine("\t\t" + company_name + "회사를 기준으로 산업제안 추출..");
                            string BusiJsonResult = await gpt.GetCompletionAsync(messages3);
                            Logger.WriteLine(BusiJsonResult);
                            gpt_result.gpt_result_busi = BusiJsonResult.Replace("`json", "").Replace("`", "");
                            var busi_model = JsonConvert.DeserializeObject<can_business>(gpt_result.gpt_result_busi);

                            var messages4 = new List<Message>
                                    {
                                        new Message
                                        {
                                            Role = "system",
                                            Content = JobToJsonPrompt
                                        },
                                        new Message
                                        {
                                            Role = "user",
                                            Content = company_name + "/" +dept_pos
                                        }
                                    };

                            Logger.WriteLine("\t\t" + company_name + "/" + dept_pos + "회사와 직무 기준으로 직무제안 추출..");
                            string JobJsonResult = await gpt.GetCompletionAsync(messages4);
                            Logger.WriteLine(JobJsonResult);
                            gpt_result.gpt_result_job = JobJsonResult.Replace("`json", "").Replace("`", "");
                            var job_model = JsonConvert.DeserializeObject<can_job>(gpt_result.gpt_result_busi);


                          }
                          catch (Exception e)
                          {
                            Logger.WriteLine("산업 / 직무 분석 중 오류 : " + e.Message, LogLevel.Warning);
                          }
                        }

                        gpt_result.dv_timestamp = mail.dv_timestamp;
                        gpt_result.reg_date = DateTime.UtcNow.AddHours(9);

                        var exist_gpt = await mailResumeEnRepo.SelectMailResumeGptOneAsync(mail.dv_timestamp);
                        if (exist_gpt != null)
                        {
                          await mailResumeEnRepo.DeleteGptResult(exist_gpt);
                        }

                        await mailResumeEnRepo.CreateOrUpdateMailResumeGPTOneAsync(gpt_result, "C");

                        is_organize = true;
                        // TODO: 데이터가 정상이면 임시로 후보자 테이블에 저장

                        continue;
                      }
                      catch (Exception e)
                      {
                        // TODO: 오류 처리 및 필요한 경우 다음 메일로 진행
                        Logger.WriteLine("내용분석 데이터 오류 : " + e.Message, LogLevel.Error);
                        continue;
                      }
                    }
                    else
                    {
                      // 이력서가 아니면 다음 파일로 이동
                      Logger.WriteLine("\t\t이력서 아님");

                      try
                      {
                        var res = await mailResumeEnRepo.SelectMailResumeFileOneAsync(resume.dv_timestamp, (int)resume.dn_seq);
                        if (res != null)
                        {
                          res.resume_type = "O";
                          await mailResumeEnRepo.CreateOrUpdateMailResumeFileOneAsync(res, "U");
                          Logger.WriteLine("\t\t이력서 언어 : " + res.resume_type + " 저장 완료");
                        }
                      }
                      catch (Exception e)
                      {
                        Logger.WriteLine("\t\t이력서 언어 저장 실패" + e.Message, LogLevel.Error);
                      }

                      continue;
                    }
                  }
                  catch
                  {
                    Logger.WriteLine("\t\t파일 내용을 읽을 수 없습니다.", LogLevel.Warning);
                  }

                }
                else
                {
                  Logger.WriteLine("\t\t파일 내용을 읽을 수 없습니다.", LogLevel.Warning);
                }
              }
            }

            if (is_organize == false)
            {
              var mail_update = await mailResumeEnRepo.SelectMailResumeOneAsync(mail.dv_timestamp);
              if (mail_update != null)
              {
                mail_update.no_resume = "Y";

                await mailResumeEnRepo.CreateOrUpdateMailResumeOneAsync(mail_update, "U");
              }
              Logger.WriteLine("\t메일: " + mail.dv_snd_name + " -> 이력서 아님 설정");
            }
            else
            {
              Logger.WriteLine("\t메일: " + mail.dv_snd_name + "저장완료");
            }
          }
        }
      }
      catch (Exception e)
      {
        Logger.WriteLine(e.Message, LogLevel.Error);
      }
    }

    // 이력서 파일 내용을 읽는 메서드
    static async Task<string> ResumeGetContents(string filePath)
    {
      try
      {
        var searchEngineRepo = new SearchEngineRepository();

        string newFileName = CopyTempFileForKMPDL(filePath, ReturnUniqueValue(1, "O"));
        string source = "/media/uploadfiles/kmpdl/" + newFileName;

        string fileContent = searchEngineRepo.TempResumeCheck(source);

        // 처리 후 임시 파일 삭제 (필요한 경우)
        // File.Delete(source);

        return fileContent;
      }
      catch (Exception e)
      {
        Logger.WriteLine(e.Message, LogLevel.Warning);
        return string.Empty;
      }
    }

    // KMPDL용 임시 파일을 복사하는 메서드
    static string CopyTempFileForKMPDL(string originalFilePath, string newFileName)
    {
      string kmpdlPath = @"D:\UploadFolder\kmpdl";
#if DEBUG
      originalFilePath = originalFilePath.Replace(@"D:\", @"\\10.1.2.150\");
      kmpdlPath = kmpdlPath.Replace(@"D:\", @"\\10.1.2.150\");
#endif

      string extension = Path.GetExtension(originalFilePath);
      string targetFilePath = Path.Combine(kmpdlPath, newFileName + extension);
      File.Copy(originalFilePath, targetFilePath, true);
      return newFileName + extension;
    }

    // 고유한 값을 반환하는 메서드
    public static string ReturnUniqueValue(int uvSeq, string head = "T")
    {
      byte[] result;

      using (var stream = new MemoryStream())
      {
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
        {
          writer.Write(DateTime.Now.Ticks);
          writer.Write(uvSeq);
        }

        stream.Position = 0;

        using (var hash = SHA256.Create())
        {
          result = hash.ComputeHash(stream);
        }
      }

      var text = new string[10];
      text[0] = head; // 최초 문자 고정

      for (var i = 1; i < text.Length; i++)
      {
        text[i] = result[i].ToString("x2");
      }

      return string.Concat(text);
    }

    static bool IsHangul(char c)
    {
      return c >= '\uAC00' && c <= '\uD7A3';
    }

    static bool IsMostlyHangul(string text)
    {
      string input = Regex.Replace(text, @"[^가-힣A-z\s]", "");

      int hangulCount = input.Count(c => IsHangul(c));
      int totalCount = input.Length;

      return (double)hangulCount / totalCount > 0.3;
    }

    public static async Task<int> FindDuplicateCandidate(string phone, string email, int except_c = 0)
    {
      try
      {
        string number_only_phone = new String(phone.Where(Char.IsDigit).ToArray());

        if (!string.IsNullOrEmpty(number_only_phone) && phone != "000-0000-0000" && phone != "0000" && number_only_phone.Length > 4)
        {
          if (number_only_phone.Substring(0, 4) == "8210")
            number_only_phone = "010" + number_only_phone.Substring(4);
        }

        if (email != "noemail@email" && email != "no@email.address" && email != "no@no" && email != "no@email")
        {
          if (!string.IsNullOrEmpty(email))
          {
            email = email.Replace("_", "[_]");
          }
        }

        CandidateRepository cr = new CandidateRepository();
        var candidate = await cr.SelectFindDuplicateCandidate(number_only_phone, email, except_c);

        if (candidate != null)
          return candidate.c_seq;
        else
          return 0;


      }
      catch (Exception e)
      {
        return 0;
      }
    }

    public static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
    {
      var currentError = errorArgs.ErrorContext.Error.Message;
      errorArgs.ErrorContext.Handled = true;
    }
  }
}
