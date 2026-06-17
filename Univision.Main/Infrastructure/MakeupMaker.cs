using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Univision.Main.Models.GPT;
using Newtonsoft.Json;
using System.Collections.Generic;
using Univision.Core.Repositories;
using Univision.Security;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

namespace Univision.Main.Infrastructure
{
  public class MakeupMaker
  {
    public async Task<(bool, string)> Make(MakeupCreateModel model, string samplePath, string savePathOnly, string saveFileName)
    {
      try
      {
        string fileNameOnly = Path.GetFileNameWithoutExtension(saveFileName);
        string extension = Path.GetExtension(saveFileName);
        string saveFilePath = savePathOnly + "\\" + saveFileName;

        int count = 1;
        while (File.Exists(saveFilePath))
        {
          saveFileName = $"{fileNameOnly}({count}){extension}";
          saveFilePath = Path.Combine(savePathOnly, saveFileName);
          count++;
        }
        File.Copy(samplePath, saveFilePath);


        using (WordprocessingDocument copyDocument = WordprocessingDocument.Open(saveFilePath, true))
        {


          Body body = copyDocument.MainDocumentPart.Document.Body;

          foreach (Table table in body.Elements<Table>())
          {
            // 테이블 내의 모든 행 순회
            foreach (TableRow row in table.Elements<TableRow>())
            {
              // 행 내의 모든 셀 순회
              foreach (TableCell cell in row.Elements<TableCell>())
              {
                ReplaceTextInTableCell(cell, "{{candidate}}", model.candidate);
              }
            }
          }

          Paragraph paragraph = new Paragraph();
          Run run = new Run();

          RunProperties NormalRunProperties = new RunProperties();
          NormalRunProperties.Append(new RunFonts() { Ascii = "맑은 고딕", HighAnsi = "맑은 고딕" });


          // Body에 단락 추가
          body.Append(AddTitle("인적사항"));

          // Body에 단락 추가
          body.Append(AddTextWithTabCenter("DOB:", 4, model.yob));
          body.Append(AddTextWithTabCenter("Gender:", 4, model.gender));
          body.Append(AddTextWithTabCenter("Address:", 4, model.addr));

          if (model.education.Count > 0)
          {
            // Body에 단락 추가
            body.Append(AddText());
            body.Append(AddTitle("학력사항"));

            foreach (var edu in model.education)
            {
              // Body에 단락 추가
              body.Append(AddEducation(edu));
            }
          }


          if (model.core.Count > 0)
          {
            body.Append(AddText());
            body.Append(AddTitle("핵심역량"));

            // 경력사항 업무상세와 동일한 최대 4depth 계층 구조 렌더링
            foreach (var core in model.core)
            {
              if (!string.IsNullOrEmpty(core.desc))
                body.Append(AddBullet(core.desc));

              foreach (var depth2 in core.depth)
              {
                if (!string.IsNullOrEmpty(depth2.desc))
                  body.Append(AddBullet(depth2.desc, left: "1.25cm", level: 1));

                foreach (var depth3 in depth2.depth)
                {
                  if (!string.IsNullOrEmpty(depth3.desc))
                    body.Append(AddBullet(depth3.desc, left: "1.75cm", level: 2));

                  foreach (var depth4 in depth3.depth)
                  {
                    if (!string.IsNullOrEmpty(depth4.desc))
                      body.Append(AddBullet(depth4.desc, left: "2.25cm", level: 3));
                  }
                }
              }
            }
          }

          body.Append(AddText());
          body.Append(AddTitle("경력사항"));
          if (model.career.Count > 0)
          {
            bool first = true;
            foreach (var career in model.career)
            {
              if (first)
              {
                first = false;
              }
              else
              {
                body.Append(AddText());
              }
              // Body에 단락 추가
              string text1 = "";
              string text2 = "";
              string dept_pos = "";
              if (!String.IsNullOrEmpty(career.company))
                text1 += career.company;
              //if (!String.IsNullOrEmpty(career.area))
              //  text1 += ", " + career.area;

              int byteLength = System.Text.Encoding.UTF8.GetByteCount(text1);
              int tabCount = 10;
              if (byteLength > 0)
              {
                byteLength = (int)(byteLength / 3.0 * 2);
                tabCount = tabCount - byteLength / 7;
              }

              if (!string.IsNullOrEmpty(career.j_yyyymm))
              {
                text2 += career.j_yyyymm;
                if (!string.IsNullOrEmpty(career.r_yyyymm))
                {
                  text2 += " – " + career.r_yyyymm;
                }
              }
              else
              {
                if (!string.IsNullOrEmpty(career.r_yyyymm))
                {
                  text2 += career.r_yyyymm;
                }
              }
              body.Append(AddTextWithTabCenter(text1, tabCount, text2, true));


              foreach (var info in career.info)
              {
                if (!string.IsNullOrEmpty(info))
                  body.Append(AddText(info));
              }


              if (!string.IsNullOrWhiteSpace(career.dept))
              {
                dept_pos += career.dept;
              }
              if (!string.IsNullOrEmpty(career.pos))
              {
                dept_pos += (!String.IsNullOrWhiteSpace(dept_pos) ? " / " : "") + career.pos;
              }
              if (!string.IsNullOrEmpty(career.is_leader) && career.is_leader == "Y")
              {
                dept_pos += "(팀장)";
              }
              body.Append(AddText(dept_pos, true));

              foreach (var depth1 in career.desc)
              {
                if (!string.IsNullOrEmpty(depth1.desc))
                  body.Append(AddBullet(depth1.desc));

                foreach (var depth2 in depth1.depth)
                {
                  if (!string.IsNullOrEmpty(depth2.desc))
                    body.Append(AddBullet(depth2.desc, left: "1.25cm", level: 1));

                  foreach (var depth3 in depth2.depth)
                  {
                    if (!string.IsNullOrEmpty(depth3.desc))
                      body.Append(AddBullet(depth3.desc, left: "1.75cm", level: 2));

                    foreach (var depth4 in depth3.depth)
                    {
                      body.Append(AddBullet(depth4.desc, left: "2.25cm", level: 3));
                    }
                  }
                }
              }

              if (!string.IsNullOrEmpty(career.r_reason))
                body.Append(AddText("[이직사유] " + career.r_reason));
            }
          }

          if (model.certifications.Count > 0)
          {
            body.Append(AddText());
            body.Append(AddTitle("자격사항"));

            foreach (var certifications in model.certifications)
            {
              // Body에 단락 추가
              body.Append(AddCert(certifications));
            }
          }

          if (model.awards.Count > 0)
          {
            body.Append(AddText());
            body.Append(AddTitle("수상사항"));

            foreach (var award in model.awards)
            {
              // Body에 단락 추가
              body.Append(AddCert(award));
            }
          }

          if (model.learns.Count > 0)
          {
            body.Append(AddText());
            body.Append(AddTitle("교육사항"));

            foreach (var learn in model.learns)
            {
              // Body에 단락 추가
              body.Append(AddLearn(learn));
            }
          }

          if (model.activities.Count > 0)
          {
            body.Append(AddText());
            body.Append(AddTitle("활동사항"));

            foreach (var activity in model.activities)
            {
              // Body에 단락 추가
              body.Append(AddLearn(activity));
            }
          }

          if (model.overseas.Count > 0)
          {
            body.Append(AddText());
            body.Append(AddTitle("해외경험"));

            foreach (var overseas in model.overseas)
            {
              // Body에 단락 추가
              body.Append(AddLearn(overseas));
            }
          }

          if (model.skills.Count > 0 || model.languages.Count > 0)
          {
            body.Append(AddText());
            body.Append(AddTitle("SKILLS", 13));

            foreach (var skill in model.skills)
            {
              // Body에 단락 추가
              body.Append(AddSkill(skill));
            }

            // 같은 언어끼리 묶어서 언어명 1줄 + level 하위 불릿으로 출력
            foreach (var langGroup in model.languages
              .GroupBy(l => (l.language ?? "").Trim()))
            {
              string text1 = langGroup.Key;
              var subLevels = new List<string>();

              foreach (var language in langGroup)
              {
                if (string.IsNullOrEmpty(language.level)) continue;

                if ("상중하 원활 보통 원어민 수준".Contains(language.level))
                  text1 += " (" + language.level + ")";
                else
                  subLevels.Add(language.level);
              }

              if (!string.IsNullOrEmpty(text1))
              {
                body.Append(AddBullet(text1));
              }
              foreach (var level in subLevels)
              {
                body.Append(AddBullet(level, left: "1.25cm", level: 1));
              }

            }
          }

          if (model.etcs.Count > 0)
          {
            body.Append(AddText());
            body.Append(AddTitle("기타사항"));

            foreach (var etc in model.etcs)
            {
              // Body에 단락 추가
              body.Append(AddBullet(etc));
            }
          }

          Paragraph pageBreakParagraph = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
          body.Append(pageBreakParagraph);
          body.Append(AddTitle("자기소개"));
          if (model.selfintro.Count > 0)
          {
            foreach (var self_intro in model.selfintro)
            {
              // \n 기준으로 줄 분리 후 각 줄을 별도 단락으로 추가
              var lines = self_intro.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
              foreach (var line in lines)
                body.Append(AddText(line));
            }
          }
          else
          {
            body.Append(AddText());
          }


          copyDocument.MainDocumentPart.Document.Save();
        }


        return (true, saveFileName);

      }
      catch (Exception e)
      {
        return (
          false
          ,
          e.Message);


      }
    }


    public async Task<(bool, string)> MakeEng(MakeupCreateModel model, string samplePath, string savePathOnly, string saveFileName)
    {
      try
      {
        string fileNameOnly = Path.GetFileNameWithoutExtension(saveFileName);
        string extension = Path.GetExtension(saveFileName);
        string saveFilePath = savePathOnly + "\\" + saveFileName;

        int count = 1;
        while (File.Exists(saveFilePath))
        {
          saveFileName = $"{fileNameOnly}({count}){extension}";
          saveFilePath = Path.Combine(savePathOnly, saveFileName);
          count++;
        }
        File.Copy(samplePath, saveFilePath);


        using (WordprocessingDocument copyDocument = WordprocessingDocument.Open(saveFilePath, true))
        {


          Body body = copyDocument.MainDocumentPart.Document.Body;

          foreach (Table table in body.Elements<Table>())
          {
            // 테이블 내의 모든 행 순회
            foreach (TableRow row in table.Elements<TableRow>())
            {
              // 행 내의 모든 셀 순회
              foreach (TableCell cell in row.Elements<TableCell>())
              {
                ReplaceTextInTableCell(cell, "{{candidate}}", model.candidate);
              }
            }
          }

          Paragraph paragraph = new Paragraph();
          Run run = new Run();

          string font_str = "Arial";
          // Body에 단락 추가
          body.Append(AddTitle("PERSONAL DETAILS", 10, font: font_str));

          // Body에 단락 추가
          body.Append(AddTextWithTabCenter("YOB:", 3, model.yob, font: font_str));
          body.Append(AddTextWithTabCenter("Gender:", 2, model.gender, font: font_str));
          body.Append(AddTextWithTabCenter("Address:", 2, model.addr, font: font_str));

          if (model.education.Count > 0)
          {
            // Body에 단락 추가
            body.Append(AddText(font: font_str));
            body.Append(AddTitle("EDUCATION", font: font_str));

            foreach (var edu in model.education)
            {
              // Body에 단락 추가
              body.Append(AddEducation(edu, font: font_str, engDate: true));
            }
          }


          if (model.core.Count > 0)
          {
            body.Append(AddText(font: font_str));
            body.Append(AddTitle("CORE COMPETENCIES", 10, font: font_str));

            // 경력사항 업무상세와 동일한 최대 4depth 계층 구조 렌더링
            foreach (var core in model.core)
            {
              if (!string.IsNullOrEmpty(core.desc))
                body.Append(AddBullet(core.desc, font: font_str));

              foreach (var depth2 in core.depth)
              {
                if (!string.IsNullOrEmpty(depth2.desc))
                  body.Append(AddBullet(depth2.desc, font: font_str, left: "1.25cm", level: 1));

                foreach (var depth3 in depth2.depth)
                {
                  if (!string.IsNullOrEmpty(depth3.desc))
                    body.Append(AddBullet(depth3.desc, font: font_str, left: "1.75cm", level: 2));

                  foreach (var depth4 in depth3.depth)
                  {
                    if (!string.IsNullOrEmpty(depth4.desc))
                      body.Append(AddBullet(depth4.desc, font: font_str, left: "2.25cm", level: 3));
                  }
                }
              }
            }
          }

          body.Append(AddText(font: font_str));
          body.Append(AddTitle("WORK EXPERIENCE", 11, font: font_str));
          if (model.career.Count > 0)
          {
            bool first = true;
            foreach (var career in model.career)
            {
              if (first)
              {
                first = false;
              }
              else
              {
                body.Append(AddText(font: font_str));
              }
              // Body에 단락 추가
              string text1 = "";
              string text2 = "";
              string dept_pos = "";
              if (!String.IsNullOrEmpty(career.company))
                text1 += career.company;
              //if (!String.IsNullOrEmpty(career.area))
              //  text1 += ", " + career.area;

              int byteLength = System.Text.Encoding.UTF8.GetByteCount(text1);
              int tabCount = 9;
              if (byteLength > 0)
              {
                byteLength = (int)(byteLength / 3.0 * 2);
                tabCount = tabCount - byteLength / 7;
              }

              if (!string.IsNullOrEmpty(career.j_yyyymm))
              {
                text2 += ToEngDate(career.j_yyyymm);

                if (!string.IsNullOrEmpty(career.r_yyyymm))
                {
                  if (career.r_yyyymm == "현재")
                  {
                    text2 += " – " + "Present";
                  }
                  else
                  {
                    text2 += " – " + ToEngDate(career.r_yyyymm);
                  }
                }
              }
              else
              {
                if (!string.IsNullOrEmpty(career.r_yyyymm))
                {
                  if (career.r_yyyymm == "현재")
                  {
                    text2 += "Present";
                  }
                  else
                  {
                    text2 += ToEngDate(career.r_yyyymm);
                  }

                }
              }
              body.Append(AddTextWithTabCenter(text1, tabCount, text2, true, font: font_str));


              foreach (var info in career.info)
              {
                if (!string.IsNullOrEmpty(info))
                  body.Append(AddText(info, font: font_str));
              }


              if (!string.IsNullOrWhiteSpace(career.dept))
              {
                dept_pos += career.dept;
              }
              if (!string.IsNullOrEmpty(career.pos))
              {
                dept_pos += (!String.IsNullOrWhiteSpace(dept_pos) ? " / " : "") + career.pos;
              }
              /*
              if (!string.IsNullOrEmpty(career.is_leader) && career.is_leader == "Y")
              {
                dept_pos += "(팀장)";
              }
              */
              body.Append(AddText(dept_pos, true, font: font_str));

              foreach (var depth1 in career.desc)
              {
                if (!string.IsNullOrEmpty(depth1.desc))
                  body.Append(AddBullet(depth1.desc, font: font_str));

                foreach (var depth2 in depth1.depth)
                {
                  if (!string.IsNullOrEmpty(depth2.desc))
                    body.Append(AddBullet(depth2.desc, font: font_str, left: "1.25cm", level: 1));

                  foreach (var depth3 in depth2.depth)
                  {
                    if (!string.IsNullOrEmpty(depth3.desc))
                      body.Append(AddBullet(depth3.desc, font: font_str, left: "1.75cm", level: 2));

                    foreach (var depth4 in depth3.depth)
                    {
                      body.Append(AddBullet(depth4.desc, font: font_str, left: "2.25cm", level: 3));
                    }
                  }
                }
              }

              if (!string.IsNullOrEmpty(career.r_reason))
                body.Append(AddText("[Reason for resignation] " + career.r_reason, font: font_str));
            }
          }

          if (model.certifications.Count > 0)
          {
            body.Append(AddText(font: font_str));
            body.Append(AddTitle("CERTIFICATION", 11, font: font_str));

            foreach (var certifications in model.certifications)
            {
              // Body에 단락 추가
              body.Append(AddCert(certifications, font: font_str, engDate: true));
            }
          }

          if (model.awards.Count > 0)
          {
            body.Append(AddText(font: font_str));
            body.Append(AddTitle("AWARDS", font: font_str));

            foreach (var award in model.awards)
            {
              // Body에 단락 추가
              body.Append(AddCert(award, font: font_str, engDate: true));
            }
          }

          if (model.learns.Count > 0)
          {
            body.Append(AddText(font: font_str));
            body.Append(AddTitle("LEARN", font: font_str));

            foreach (var learn in model.learns)
            {
              // Body에 단락 추가
              body.Append(AddLearn(learn, font: font_str, engDate: true));
            }
          }

          if (model.activities.Count > 0)
          {
            body.Append(AddText(font: font_str));
            body.Append(AddTitle("ACTIVITIES", font: font_str));

            foreach (var activity in model.activities)
            {
              // Body에 단락 추가
              body.Append(AddLearn(activity, font: font_str, engDate: true));
            }
          }

          if (model.overseas.Count > 0)
          {
            body.Append(AddText(font: font_str));
            body.Append(AddTitle("OVERSEAS", font: font_str));

            foreach (var overseas in model.overseas)
            {
              // Body에 단락 추가
              body.Append(AddLearn(overseas, font: font_str, engDate: true));
            }
          }

          if (model.skills.Count > 0 || model.languages.Count > 0)
          {
            body.Append(AddText(font: font_str));
            body.Append(AddTitle("SKILLS", 13, font: font_str));

            foreach (var skill in model.skills)
            {
              // Body에 단락 추가
              body.Append(AddSkill(skill, font: font_str));
            }

            // 같은 언어끼리 묶어서 언어명 1줄 + level 하위 불릿으로 출력
            foreach (var langGroup in model.languages
              .GroupBy(l => (l.language ?? "").Trim()))
            {
              string text1 = langGroup.Key;
              var subLevels = new List<string>();

              foreach (var language in langGroup)
              {
                if (string.IsNullOrEmpty(language.level)) continue;

                if ("상중하 원활 보통 원어민 수준".Contains(language.level))
                  text1 += " (" + language.level + ")";
                else
                  subLevels.Add(language.level);
              }

              if (!string.IsNullOrEmpty(text1))
              {
                body.Append(AddBullet(text1, font: font_str));
              }
              foreach (var level in subLevels)
              {
                body.Append(AddBullet(level, font: font_str, left: "1.25cm", level: 1));
              }

            }
          }

          if (model.etcs.Count > 0)
          {
            body.Append(AddText());
            body.Append(AddTitle("OTHERS", font: font_str));

            foreach (var etc in model.etcs)
            {
              // Body에 단락 추가
              body.Append(AddBullet(etc, font: font_str));
            }
          }

          Paragraph pageBreakParagraph = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
          body.Append(pageBreakParagraph);
          body.Append(AddTitle("PERSONAL INTRODUCTION", 9, font: font_str));
          if (model.selfintro.Count > 0)
          {
            foreach (var self_intro in model.selfintro)
            {
              // \n 기준으로 줄 분리 후 각 줄을 별도 단락으로 추가
              var lines = self_intro.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
              foreach (var line in lines)
                body.Append(AddText(text: line, font: font_str));
            }
          }
          else
          {
            body.Append(AddText(font: font_str));
          }


          copyDocument.MainDocumentPart.Document.Save();
        }


        return (true, saveFileName);

      }
      catch (Exception e)
      {
        return (
          false
          ,
          e.Message);


      }
    }

    private Paragraph AddTitle(string title, int tabCount = 12, string font = "맑은 고딕", string left = "0", string hanging = "0", string after = "1.75pt", string before = "1.75pt")
    {
      RunProperties sectionRunProperties = new RunProperties();
      sectionRunProperties.Append(new Bold());
      sectionRunProperties.Append(new Underline() { Val = UnderlineValues.Single });
      sectionRunProperties.Append(new Color() { Val = "#008080" });
      sectionRunProperties.Append(new FontSize() { Val = "24" });
      sectionRunProperties.Append(new RunFonts() { Ascii = font, HighAnsi = font });

      var spacingBetweenLines1 = new SpacingBetweenLines() { Before = before, After = after };  // Get rid of space between bullets
      var indentation = new Indentation() { Left = left, Hanging = hanging };  // correct indentation
      Justification justification = new Justification() { Val = JustificationValues.Left };

      ParagraphProperties para = new ParagraphProperties(
          spacingBetweenLines1, indentation, justification
      );
      Paragraph paragraph = new Paragraph();
      paragraph.PrependChild(para);

      Run run = new Run();
      run.PrependChild(sectionRunProperties);

      run.Append(new Text(title));
      for (int i = 0; i < tabCount; i++)
      {
        run.AppendChild(new TabChar());
      }
      paragraph.Append(run);
      // Body에 단락 추가

      return paragraph;
    }


    private Paragraph AddText(string text = null, bool bold = false, string font = "맑은 고딕", string left = "0", string hanging = "0", string after = "1.75pt", string before = "1.75pt")
    {
      var spacingBetweenLines1 = new SpacingBetweenLines() { Before = before, After = after };  // Get rid of space between bullets
      var indentation = new Indentation() { Left = left, Hanging = hanging };  // correct indentation 
      Justification justification = new Justification() { Val = JustificationValues.Left };
      ParagraphProperties para = new ParagraphProperties(
          spacingBetweenLines1, indentation, justification
      );
      Paragraph paragraph = new Paragraph();
      paragraph.PrependChild(para);

      Run run = new Run();

      RunProperties runProperties = new RunProperties();
      runProperties.Append(new RunFonts() { Ascii = font, HighAnsi = font });
      if (bold)
      {
        runProperties.Append(new Bold());
      }
      run.PrependChild(runProperties);

      if (!String.IsNullOrEmpty(text))
        run.Append(new Text(text));


      paragraph.Append(run);

      return paragraph;
    }

    private Paragraph AddTextWithTabCenter(string text1 = "", int tabCount = 4, string text2 = "", bool bold = false, string font = "맑은 고딕", string left = "0", string hanging = "0", string after = "1.75pt", string before = "1.75pt")
    {



      var spacingBetweenLines1 = new SpacingBetweenLines() { Before = before, After = after };  // Get rid of space between bullets
      var indentation = new Indentation() { Left = left, Hanging = hanging };  // correct indentation 
      Justification justification = new Justification() { Val = JustificationValues.Left };
      ParagraphProperties para = new ParagraphProperties(
          spacingBetweenLines1, indentation, justification
      );
      Paragraph paragraph = new Paragraph();
      paragraph.PrependChild(para);

      Run run = new Run();
      RunProperties runProperties = new RunProperties();
      runProperties.Append(new RunFonts() { Ascii = font, HighAnsi = font });
      if (bold)
      {
        runProperties.Append(new Bold());
      }
      run.PrependChild(runProperties);

      run.Append(new Text(text1));
      for (int i = 0; i < tabCount; i++)
      {
        run.AppendChild(new TabChar());
      }
      if (bold && tabCount > 0)
      {
        for (int i = 0; i < 7; i++)
        {
          text2 = " " + text2;
        }
      }
      var lastText = new Text(text2);
      lastText.Space = SpaceProcessingModeValues.Preserve;
      run.AppendChild(lastText);
      paragraph.Append(run);

      return paragraph;
    }

    private Paragraph AddEducation(MakeupEducationModel edu, string font = "맑은 고딕", string left = "0", string hanging = "0", string after = "1.75pt", string before = "1.75pt", bool engDate = false)
    {
      var spacingBetweenLines1 = new SpacingBetweenLines() { Before = before, After = after };  // Get rid of space between bullets
      var indentation = new Indentation() { Left = left, Hanging = hanging };  // correct indentation 
      Justification justification = new Justification() { Val = JustificationValues.Left };

      ParagraphProperties para = new ParagraphProperties(
          spacingBetweenLines1, indentation, justification
      );
      Paragraph paragraph = new Paragraph();
      paragraph.PrependChild(para);

      Run run = new Run();
      RunProperties runProperties = new RunProperties();
      runProperties.Append(new RunFonts() { Ascii = font, HighAnsi = font });
      run.PrependChild(runProperties);

      run.Append(new Text(edu.school));
      /*
      if (!String.IsNullOrEmpty(edu.area))
        run.AppendChild(new Text("(" + edu.area + ")"));
      */
      if (!String.IsNullOrEmpty(edu.major))
      {
        var text = new Text(" " + edu.major);
        text.Space = SpaceProcessingModeValues.Preserve;
        run.AppendChild(text);
      }
      /*
      if (!String.IsNullOrEmpty(edu.degree))
      {
        var text = new Text(" " + edu.degree);
        text.Space = SpaceProcessingModeValues.Preserve;
        run.AppendChild(text);
      }
      */
      if (!engDate && !String.IsNullOrEmpty(edu.is_grdt))
      {
        string text1 = "";
        if ("졸업".Contains(edu.is_grdt))
        {
          text1 = "졸업";
        }
        else if ("수료".Contains(edu.is_grdt))
        {
          text1 = "수료";
        }
        else
        {
          text1 = edu.is_grdt;
        }

        var text = new Text(" " + text1);
        text.Space = SpaceProcessingModeValues.Preserve;
        run.AppendChild(text);
      }
      if (!String.IsNullOrEmpty(edu.grade))
      {
        var text = new Text(" (" + edu.grade + ")");
        text.Space = SpaceProcessingModeValues.Preserve;
        run.AppendChild(text);
      }


      int byteLength = System.Text.Encoding.UTF8.GetByteCount(run.InnerText);
      int tabCount = 11;
      if (byteLength > 0)
      {
        byteLength = (int)(byteLength / 3.0 * 2);
        tabCount = tabCount - byteLength / 7;
      }

      for (int i = 0; i < tabCount; i++)
      {
        run.AppendChild(new TabChar());
      }

      if (!string.IsNullOrEmpty(edu.ad_yyyymm))
      {
        string adStr = engDate ? ToEngDate(edu.ad_yyyymm) : edu.ad_yyyymm;
        run.AppendChild(new Text(adStr));

        if (!string.IsNullOrEmpty(edu.gdt_yyyymm))
        {
          string gdtStr = engDate ? ToEngDate(edu.gdt_yyyymm) : edu.gdt_yyyymm;
          var text = new Text(" – " + gdtStr);
          text.Space = SpaceProcessingModeValues.Preserve;
          run.AppendChild(text);
        }
      }
      else
      {
        if (!string.IsNullOrEmpty(edu.gdt_yyyymm))
        {
          string gdtStr = engDate ? ToEngDate(edu.gdt_yyyymm) : edu.gdt_yyyymm;
          run.AppendChild(new Text(gdtStr));
        }
      }

      paragraph.Append(run);

      return paragraph;
    }
    private Paragraph AddBullet(string core, string font = "맑은 고딕", string left = "0.75cm", string hanging = "0.5cm", string after = "1.75pt", string before = "1.75pt", int level = 0, int num_id = 1)
    {
      Paragraph paragraph = new Paragraph();
      Run run = new Run();
      RunProperties runProperties = new RunProperties();
      runProperties.Append(new RunFonts() { Ascii = font, HighAnsi = font });
      run.PrependChild(runProperties);

      var numberingProperties = new NumberingProperties(new NumberingLevelReference() { Val = level }, new NumberingId() { Val = num_id });
      var spacingBetweenLines1 = new SpacingBetweenLines() { Before = before, After = after };  // Get rid of space between bullets
      var indentation = new Indentation() { Left = left, Hanging = hanging };  // correct indentation 

      Justification justification = new Justification() { Val = JustificationValues.Left };
      ParagraphProperties para = new ParagraphProperties(
          numberingProperties, spacingBetweenLines1, indentation, justification
      );
      paragraph.PrependChild(para);
      //run.PrependChild(NormalRunProperties);
      run.Append(new Text(core));
      paragraph.Append(run);

      return paragraph;
    }

    private Paragraph AddCert(MakeupCertModel certification, string font = "맑은 고딕", bool engDate = false)
    {
      string text = "";
      if (!String.IsNullOrEmpty(certification.name))
      {
        if (!string.IsNullOrEmpty(certification.name))
        {
          text += certification.name;
        }
        if (!string.IsNullOrEmpty(certification.gov))
        {
          text += " – " + certification.gov;
        }
        if (!string.IsNullOrEmpty(certification.year))
        {
          string yearStr = engDate ? ToEngDate(certification.year) : certification.year;
          text += " (" + yearStr + ")";
        }
      }

      var paragraph = AddBullet(text, font: font);

      return paragraph;
    }

    private Paragraph AddLearn(MakeupLearnModel learn, string font = "맑은 고딕", bool engDate = false)
    {
      string text = "";
      if (!String.IsNullOrEmpty(learn.name))
      {
        if (!string.IsNullOrEmpty(learn.name))
        {
          text += learn.name;
        }
        if (!string.IsNullOrEmpty(learn.gov))
        {
          text += " – " + learn.gov;
        }
        if (!string.IsNullOrEmpty(learn.year1))
        {
          string y1 = engDate ? ToEngDate(learn.year1) : learn.year1;
          text += " (" + y1;
          if (!string.IsNullOrEmpty(learn.year2))
          {
            string y2 = engDate ? ToEngDate(learn.year2) : learn.year2;
            text += " – " + y2;
          }
          text += ")";
        }
      }

      var paragraph = AddBullet(text, font: font);

      return paragraph;
    }

    private string ToEngDate(string yyyymm)
    {
      if (string.IsNullOrEmpty(yyyymm)) return yyyymm;
      var parts = yyyymm.Split('.');
      if (parts.Length < 2) return yyyymm;
      if (!int.TryParse(parts[0].Trim(), out int year)) return yyyymm;
      if (!int.TryParse(parts[1].Trim(), out int month) || month < 1 || month > 12) return yyyymm;
      string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                          "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
      return months[month - 1] + ". " + year;
    }

    private Paragraph AddSkill(MakeupSkillModel skill, string font = "맑은 고딕")
    {
      string text = "";
      if (!String.IsNullOrEmpty(skill.name))
      {
        if (!string.IsNullOrEmpty(skill.name))
        {
          text += skill.name;
        }
        if (!string.IsNullOrEmpty(skill.desc))
        {
          text += " (" + skill.desc + ")";
        }
      }

      var paragraph = AddBullet(text, font: font);

      return paragraph;
    }

    private static void ReplaceTextInParagraph(Paragraph paragraph, string searchText, string replaceText)
    {
      // 단락 내의 모든 Run 요소 순회
      foreach (Run run in paragraph.Elements<Run>())
      {
        // Run 요소 내의 모든 Text 요소 순회
        foreach (Text text in run.Elements<Text>())
        {
          // 텍스트 교체
          if (text.Text.Contains(searchText))
          {
            text.Text = text.Text.Replace(searchText, replaceText);
          }
        }
      }
    }

    private static void ReplaceTextInTableCell(TableCell cell, string searchText, string replaceText)
    {
      // 셀 내의 모든 단락 순회
      foreach (Paragraph paragraph in cell.Elements<Paragraph>())
      {
        // 단락 내의 모든 Run 요소 순회
        foreach (Run run in paragraph.Elements<Run>())
        {
          // Run 요소 내의 모든 Text 요소 순회
          foreach (Text text in run.Elements<Text>())
          {
            // 텍스트 교체
            if (text.Text.Contains(searchText))
            {
              text.Text = text.Text.Replace(searchText, replaceText);
            }
          }
        }
      }
    }
  }
}
