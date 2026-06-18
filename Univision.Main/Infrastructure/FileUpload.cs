using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;

namespace Univision.Main.Infrastructure
{
    public class FileUpload
    {
        private static string _baseUrl;

        public FileUpload()
        {
            _baseUrl = ConfigurationManager.AppSettings["_uploadPath"].ToString();
        }

        public List<ViewDataUploadFilesResult> Upload(HttpPostedFileBase[] files)
        {
            try
            {
                //업로드 루트 디렉토리 체크 후, 없으면 생성.
                Directory.CreateDirectory(_baseUrl);

                var resultList = new List<ViewDataUploadFilesResult>();

                foreach (var file in files)
                {
                    ////업로드 차단할 확장자 입력.
                    //if (!file.FullName.Contains(".exe")) 
                    //    continue;

                    if (file.ContentLength > 0)
                    {
                        String fullPath = Path.Combine(_baseUrl, file.FileName);

                        file.SaveAs(fullPath);

                        ViewDataUploadFilesResult result = new ViewDataUploadFilesResult()
                        {
                            name = file.FileName,
                            size = file.ContentLength,
                            type = file.ContentType,
                            url = fullPath,
                            deleteUrl = "",
                            thumbnailUrl = "",
                            deleteType = ""
                        };

                        resultList.Add(result);
                    }
                }

                return resultList;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        //최종 저장시 임시폴더 -> 본폴더로 이동
        public string UploadTempToFolder(string org_file, string target_dir)
        {
            try
            {
                string rst_file_name = "";
                if (File.Exists(org_file))
                {
                    //정식 폴더생성
                    Directory.CreateDirectory(target_dir);
                    string only_file_ext = Path.GetExtension(org_file);
                    string only_file_name = Path.GetFileNameWithoutExtension(org_file);

                    rst_file_name = only_file_name + only_file_ext;

                    string target_dir_file = Path.Combine(target_dir, rst_file_name);

                    int count = 1;

                    //파일명 중복확인 중복이면 넘버링
                    while (File.Exists(target_dir_file))
                    {
                        string tmp_only_file_name = string.Format("{0} ({1})", only_file_name, count++);
                        rst_file_name = tmp_only_file_name + only_file_ext;
                        target_dir_file = Path.Combine(target_dir, rst_file_name);
                    }
                    //파일 최종 이동
                    File.Move(org_file, target_dir_file);
                    
                }
                return rst_file_name;
            }
            catch //(Exception e)
            {
                return "";
            }
        }
        //텍스트 추출 모듈을 위한 파일 복사 (국문 파일명이 잘 안됨 ㅠㅠ)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="org_file_path">원본파일명 경로포함</param>
        /// <param name="new_file_name">복사될 파일명 확장자/경로 미포함</param>
        /// <returns></returns>
        public string CopyTempFileForKMPDL(string org_file_path, string new_file_name)
        {
            string ext = Path.GetExtension(org_file_path);
            string target_file_path = @"D:\UploadFolder\kmpdl\" + new_file_name + ext;
            File.Copy(org_file_path, target_file_path, true);
            string rtn_path = new_file_name + ext;
            return rtn_path;
        }

        public void DeleteTempFileForKMPDL(string file_path)
        {
            File.Delete(@"D:\UploadFolder\kmpdl\" + file_path);
        }

        //임시폴더에 파일 업로드(게시판류)
        public UploadFileResult UploadTemp(string path, string sub_path1, string sub_path2, HttpPostedFileBase file)
        {
            try
            {
                //업로드 루트 디렉토리 체크 후, 없으면 생성.

                string dir = Path.Combine(path, sub_path1, sub_path2);

                Directory.CreateDirectory(dir);

                ////업로드 차단할 확장자 입력.
                //if (!file.FullName.Contains(".exe")) 
                //    continue;

                UploadFileResult result = new UploadFileResult();

                if (file.ContentLength > 0)
                {
                    //var newFileName = new Guid();
                    //정식 폴더생성
                    string only_file_ext = Path.GetExtension(file.FileName);
                    string only_file_name = Path.GetFileNameWithoutExtension(file.FileName);
                    string rst_file_name = only_file_name + only_file_ext;
                    string target_dir_file = Path.Combine(dir, rst_file_name);

                    int count = 1;

                    //파일명 중복확인 중복이면 넘버링
                    while (File.Exists(target_dir_file))
                    {
                        string tmp_only_file_name = string.Format("{0} ({1})", only_file_name, count++);
                        rst_file_name = tmp_only_file_name + only_file_ext;
                        target_dir_file = Path.Combine(dir, rst_file_name);
                    }
                    //파일 최종 저장
                    file.SaveAs(target_dir_file);


                    result = new UploadFileResult()
                    {
                        status = true,
                        name = rst_file_name,
                        dbPath = "UploadedFiles/"+ sub_path1 + "/"+ sub_path2 + "/" + rst_file_name,
                        extension = only_file_ext,
                        filePath = target_dir_file,
                        originName = file.FileName
                    };
                }
                else
                {
                    result.status = false;
                    result.statusMessage = "빈 파일 입니다.\n파일내 작성된 내용이 없습니다.";
                }


                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    private static Image resizeImage(Image imgToResize, Size size)
    {
      //Get the image current width  
      int sourceWidth = imgToResize.Width;
      //Get the image current height  
      int sourceHeight = imgToResize.Height;
      float nPercent = 0;
      float nPercentW = 0;
      float nPercentH = 0;
      //Calulate  width with new desired size  
      nPercentW = ((float)size.Width / (float)sourceWidth);
      //Calculate height with new desired size  
      nPercentH = ((float)size.Height / (float)sourceHeight);
      if (nPercentH < nPercentW)
        nPercent = nPercentH;
      else
        nPercent = nPercentW;
      //New Width  
      int destWidth = (int)(sourceWidth * nPercent);
      //New Height  
      int destHeight = (int)(sourceHeight * nPercent);
      Bitmap b = new Bitmap(destWidth, destHeight);
      Graphics g = Graphics.FromImage((System.Drawing.Image)b);
      g.InterpolationMode = InterpolationMode.HighQualityBicubic;
      // Draw image with new width and height  
      g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
      g.Dispose();
      return (System.Drawing.Image)b;
    }

    public UploadFileResult UploadImageTemp(string path, string sub_path1, string sub_path2, HttpPostedFileBase file, bool resize = false)
    {
      try
      {
        //업로드 루트 디렉토리 체크 후, 없으면 생성.

        string dir = Path.Combine(path, sub_path1, sub_path2);

        Directory.CreateDirectory(dir);

        ////업로드 차단할 확장자 입력.
        //if (!file.FullName.Contains(".exe")) 
        //    continue;

        UploadFileResult result = new UploadFileResult();

        if (file.ContentLength > 0)
        {
          //var newFileName = new Guid();
          //정식 폴더생성
          string only_file_ext = Path.GetExtension(file.FileName);
          string only_file_name = Path.GetFileNameWithoutExtension(file.FileName);
          string rst_file_name = only_file_name + only_file_ext;
          string target_dir_file = Path.Combine(dir, rst_file_name);

          int count = 1;

          //파일명 중복확인 중복이면 넘버링
          while (File.Exists(target_dir_file))
          {
            string tmp_only_file_name = string.Format("{0} ({1})", only_file_name, count++);
            rst_file_name = tmp_only_file_name + only_file_ext;
            target_dir_file = Path.Combine(dir, rst_file_name);
          }
          //파일 최종 저장
          file.SaveAs(target_dir_file);
          if (resize)
          {
            Image img = Image.FromFile(target_dir_file);
            Image resize_img = resizeImage(img, new Size(200, 200));
            rst_file_name = "resize_" + rst_file_name;
            target_dir_file = Path.Combine(dir, rst_file_name);
            resize_img.Save(target_dir_file);
          }
          


          result = new UploadFileResult()
          {
            status = true,
            name = rst_file_name,
            dbPath = "UploadedFiles/" + sub_path1 + "/" + sub_path2 + "/" + rst_file_name,
            extension = only_file_ext,
            filePath = target_dir_file,
            originName = file.FileName
          };
        }
        else
        {
          result.status = false;
          result.statusMessage = "빈 파일 입니다.\n파일내 작성된 내용이 없습니다.";
        }


        return result;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    //일반 파일 업로드
    public UploadFileResult UploadCommon(string path, string sub_path, HttpPostedFileBase file)
        {
            try
            {
                //업로드 루트 디렉토리 체크 후, 없으면 생성.
                string dir = Path.Combine(path, sub_path);
                Directory.CreateDirectory(dir);

                ////업로드 차단할 확장자 입력.
                //if (!file.FullName.Contains(".exe")) 
                //    continue;

                UploadFileResult result = new UploadFileResult();

                if (file.ContentLength > 0)
                {
                    string only_file_ext = Path.GetExtension(file.FileName);
                    string only_file_name = Path.GetFileNameWithoutExtension(file.FileName);
                    string rst_file_name = file.FileName;
                    string target_dir_file = Path.Combine(dir, rst_file_name);

                    int count = 1;
                    //파일명 중복확인 중복이면 넘버링
                    while (File.Exists(target_dir_file))
                    {
                        string tmp_only_file_name = string.Format("{0} ({1})", only_file_name, count++);
                        rst_file_name = tmp_only_file_name + only_file_ext;
                        target_dir_file = Path.Combine(dir, rst_file_name);
                    }

                    file.SaveAs(target_dir_file);

                    result = new UploadFileResult()
                    {
                        status = true,
                        name = rst_file_name,
                        dbPath = "UploadedFiles/" + sub_path + "/" + rst_file_name,
                        extension = Path.GetExtension(rst_file_name),
                        filePath = target_dir_file,
                        originName = file.FileName
                    };
                }
                else
                {
                    result.status = false;
                    result.statusMessage = "빈 파일 입니다.\n파일내 작성된 내용이 없습니다.";
                }


                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    public UploadFileResult UploadReceipt(string path, string sub_path, string new_file_name, HttpPostedFileBase file)
    {
      try
      {
        //업로드 루트 디렉토리 체크 후, 없으면 생성.
        string dir = Path.Combine(path, sub_path);
        Directory.CreateDirectory(dir);

        ////업로드 차단할 확장자 입력.
        //if (!file.FullName.Contains(".exe")) 
        //    continue;

        UploadFileResult result = new UploadFileResult();

        if (file.ContentLength > 0)
        {
          string only_file_ext = Path.GetExtension(file.FileName);
          string only_file_name = new_file_name; // Path.GetFileNameWithoutExtension(file.FileName);
          string rst_file_name = new_file_name + only_file_ext; //file.FileName;
          string target_dir_file = Path.Combine(dir, rst_file_name);

          int count = 1;
          //파일명 중복확인 중복이면 넘버링
          while (File.Exists(target_dir_file))
          {
            string tmp_only_file_name = string.Format("{0} ({1})", only_file_name, count++);
            rst_file_name = tmp_only_file_name + only_file_ext;
            target_dir_file = Path.Combine(dir, rst_file_name);
          }

          file.SaveAs(target_dir_file);

          result = new UploadFileResult()
          {
            status = true,
            name = rst_file_name,
            dbPath = "UploadedFiles/" + sub_path + "/" + rst_file_name,
            extension = Path.GetExtension(rst_file_name),
            filePath = target_dir_file,
            originName = file.FileName
          };
        }
        else
        {
          result.status = false;
          result.statusMessage = "빈 파일 입니다.\n파일내 작성된 내용이 없습니다.";
        }


        return result;
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public UploadFileResult UploadResume(int c_seq, string path, HttpPostedFileBase file)
        {
            try
            {
                //업로드 루트 디렉토리 체크 후, 없으면 생성.
                string dir = Path.Combine(path, @"candidate\" + c_seq);

                Directory.CreateDirectory(dir);

                ////업로드 차단할 확장자 입력.
                //if (!file.FullName.Contains(".exe")) 
                //    continue;

                UploadFileResult result = new UploadFileResult();

                if (file.ContentLength > 0)
                {
                    //var newFileName = new Guid();

                    string fullPath = Path.Combine(dir, file.FileName).Normalize();

                    string newFullPath = fullPath;
                    string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
                    string newFileName = file.FileName.Normalize();

                    int count = 1;
                    while (File.Exists(newFullPath))
                    {
                        newFileName = string.Format("{0}({1})", fileNameOnly, count++) + Path.GetExtension(file.FileName);
                        newFullPath = Path.Combine(dir, newFileName);
                    }

                    file.SaveAs(newFullPath);

                    result = new UploadFileResult()
                    {
                        status = true,
                        name = newFileName,
                        dbPath = "/UploadedFiles/candidate/" + c_seq + "/" + newFileName, 
                        extension = Path.GetExtension(newFileName),
                        filePath = newFullPath,
                        originName = file.FileName
                    };
                }
                else
                {
                    result.status = false;
                    result.statusMessage = "빈 파일 입니다.\n파일내 작성된 내용이 없습니다.";
                }
                    

                return result;
            }
            catch (Exception e)
            {
                return new UploadFileResult()
                {
                    status = false,
                    statusMessage = e.Message
                };
            }
        }

    public UploadFileResult UploadDirectorActivity(string path, string sub_path1, string sub_path2, HttpPostedFileBase file, bool resize = false)
    {
      try
      {
        //업로드 루트 디렉토리 체크 후, 없으면 생성.

        string dir = Path.Combine(path, sub_path1, sub_path2);

        Directory.CreateDirectory(dir);

        ////업로드 차단할 확장자 입력.
        //if (!file.FullName.Contains(".exe")) 
        //    continue;

        UploadFileResult result = new UploadFileResult();

        if (file.ContentLength > 0)
        {
          //var newFileName = new Guid();
          //정식 폴더생성
          string only_file_ext = Path.GetExtension(file.FileName);
          string only_file_name = Path.GetFileNameWithoutExtension(file.FileName);
          string rst_file_name = only_file_name + only_file_ext;
          string target_dir_file = Path.Combine(dir, rst_file_name);

          int count = 1;

          //파일명 중복확인 중복이면 넘버링
          while (File.Exists(target_dir_file))
          {
            string tmp_only_file_name = string.Format("{0} ({1})", only_file_name, count++);
            rst_file_name = tmp_only_file_name + only_file_ext;
            target_dir_file = Path.Combine(dir, rst_file_name);
          }
          //파일 최종 저장
          file.SaveAs(target_dir_file);
          if (resize && isImageFile(target_dir_file))
          {
            Image img = Image.FromFile(target_dir_file);
            Image resize_img = resizeImage(img, new Size(800, 800));
            rst_file_name = "resize_" + rst_file_name;
            target_dir_file = Path.Combine(dir, rst_file_name);
            resize_img.Save(target_dir_file);
          }



          result = new UploadFileResult()
          {
            status = true,
            name = rst_file_name,
            dbPath = "UploadedFiles/" + sub_path1 + "/" + sub_path2 + "/" + rst_file_name,
            extension = only_file_ext,
            filePath = target_dir_file,
            originName = file.FileName
          };
        }
        else
        {
          result.status = false;
          result.statusMessage = "빈 파일 입니다.\n파일내 작성된 내용이 없습니다.";
        }


        return result;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public bool isImageFile(string filename)
    {
      try
      {
        using (Image newImage = Image.FromFile(filename))
        { }
      }
      catch (OutOfMemoryException ex)
      {
        //The file does not have a valid image format.
        //-or- GDI+ does not support the pixel format of the file

        return false;
      }
      return true;
    }

    public UploadFileResult UploadContract(int c_seq, string path, HttpPostedFileBase file)
        {
            try
            {
                //업로드 루트 디렉토리 체크 후, 없으면 생성.
                string dir = Path.Combine(path, "client/" + c_seq);

                Directory.CreateDirectory(dir);

                ////업로드 차단할 확장자 입력.
                //if (!file.FullName.Contains(".exe")) 
                //    continue;

                UploadFileResult result = new UploadFileResult();

                if (file.ContentLength > 0)
                {
                    //var newFileName = new Guid();

                    string fullPath = Path.Combine(dir, file.FileName);

                    string newFullPath = fullPath;
                    string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
                    string newFileName = file.FileName;

                    int count = 1;
                    while (File.Exists(newFullPath))
                    {
                        newFileName = string.Format("{0}({1})", fileNameOnly, count++) + Path.GetExtension(file.FileName);
                        newFullPath = Path.Combine(dir, newFileName);
                    }

                    file.SaveAs(newFullPath);

                    result = new UploadFileResult()
                    {
                        status = true,
                        name = newFileName,
                        dbPath = "UploadedFiles/client/" + c_seq + "/" + newFileName,
                        extension = Path.GetExtension(newFileName),
                        filePath = newFullPath,
                        originName = file.FileName
                    };
                }
                else
                {
                    result.status = false;
                    result.statusMessage = "빈 파일 입니다.\n파일내 작성된 내용이 없습니다.";
                }


                return result;
            }
            catch (Exception e)
            {
                return new UploadFileResult()
                {
                    status = false,
                    statusMessage = e.Message
                };
            }
        }

        /// <summary>
        /// 이력서 임시 업로드
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public UploadFileResult UploadResumeTemp(string path, HttpPostedFileBase file)
        {
            try
            {
                //업로드 루트 디렉토리 체크 후, 없으면 생성.
                string dir = Path.Combine(path, "candidateTemp");

                Directory.CreateDirectory(dir);

                ////업로드 차단할 확장자 입력.
                //if (!file.FullName.Contains(".exe")) 
                //    continue;

                UploadFileResult result = new UploadFileResult();

                if (file.ContentLength > 0)
                {
                    
                    string fullPath = Path.Combine(dir, file.FileName);

                    string newFullPath = fullPath;
                    string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);

                    int count = 1;
                    while (File.Exists(newFullPath))
                    {
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                        newFullPath = Path.Combine(dir, tempFileName + Path.GetExtension(file.FileName));
                    }

                    file.SaveAs(newFullPath);

                    result = new UploadFileResult()
                    {
                        status = true,
                        filePath = newFullPath,
                    };
                }
                else
                {
                    result.status = false;
                    result.statusMessage = "빈 파일 입니다.\n파일내 작성된 내용이 없습니다.";
                }


                return result;
            }
            catch (Exception e)
            {
                return new UploadFileResult()
                {
                    status = false,
                    statusMessage = e.Message
                };
            }
        }

        /// <summary>
        /// 파일 위치 이동
        /// </summary>
        /// <param name="originPath">파일 원본 위치</param>
        /// <param name="newPath">이동할 파일 위치</param>
        /// <returns></returns>
        public UploadFileResult MoveFile(string originPath, string newPath)
        {
            try
            {
                if (string.IsNullOrEmpty(originPath))
                    return new UploadFileResult() { status = false, statusMessage = "원본 파일 경로가 비어 있습니다." };
                if (!File.Exists(originPath))
                    return new UploadFileResult() { status = false, statusMessage = "원본 파일을 찾을 수 없습니다. (" + originPath + ")" };

                string newFullPath = Path.Combine(newPath, Path.GetFileName(originPath).Normalize());
                string fileNameOnly = Path.GetFileNameWithoutExtension(originPath).Normalize();
                string newFileName =  Path.GetFileName(originPath).Normalize();

                Directory.CreateDirectory(newPath);

                int count = 1;
                while (File.Exists(newFullPath))
                {
                    newFileName = string.Format("{0}({1})", fileNameOnly, count++) + Path.GetExtension(originPath);
                    newFullPath = Path.Combine(newPath, newFileName);
                }

                // 임시파일이 일시적으로 잠겨 있을 수 있어(바이러스 검사/인덱서 등) 전송 오류는 재시도
                // File.Move 는 성공 시 원본을 제거하므로 별도 File.Delete 불필요
                RetryIo(() => File.Move(originPath, newFullPath));

                UploadFileResult result = new UploadFileResult()
                {
                    status = true,
                    name = newFileName,
                    dbPath = Path.Combine(newPath, newFileName),
                    extension = Path.GetExtension(newFileName),
                    filePath = newFullPath,
                    originName = fileNameOnly + Path.GetExtension(originPath)
                };

                return result;
            }
            catch (Exception e)
            {
                return new UploadFileResult()
                {
                    status = false,
                    statusMessage = e.Message
                };
            }
        }

        // 일시적 IO 오류(파일 잠금 등) 발생 시 잠깐 대기 후 재시도
        private static void RetryIo(Action op, int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try { op(); return; }
                catch (IOException) { if (attempt >= maxAttempts) throw; System.Threading.Thread.Sleep(200 * attempt); }
            }
        }

        public UploadFileResult CopyFile(string originPath, string newPath)
        {
            try
            {
                string newFullPath = Path.Combine(newPath, Path.GetFileName(originPath).Normalize());
                string fileNameOnly = Path.GetFileNameWithoutExtension(originPath).Normalize();
                string newFileName = Path.GetFileName(originPath).Normalize();

                bool is_copied = false;

                Directory.CreateDirectory(newPath);

                int count = 1;
                while (File.Exists(newFullPath))
                {
                    newFileName = string.Format("{0}({1})", fileNameOnly, count++) + Path.GetExtension(originPath);
                    newFullPath = Path.Combine(newPath, newFileName);
                }

                RetryIo(() => File.Copy(originPath.Normalize(), newFullPath));

                //File.Delete(originPath);
                if (File.Exists(newFullPath))
                    is_copied = true;

                UploadFileResult result = new UploadFileResult()
                {
                    status = is_copied,
                    name = newFileName,
                    dbPath = Path.Combine(newPath, newFileName),
                    extension = Path.GetExtension(newFileName),
                    filePath = newFullPath,
                    originName = fileNameOnly + Path.GetExtension(originPath)
                };

                return result;
            }
            catch (Exception e)
            {
                return new UploadFileResult()
                {
                    status = false,
                    statusMessage = e.Message
                };
            }
        }

        public bool DeleteFile(string fullPath)
        {
            try
            {
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Rename(FileInfo fileInfo, string newName)
        {
            try
            {
                fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + newName);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }

    public class UploadFileResult
    {
        public bool status { get; set; }
        public string statusMessage { get; set; }
        public string name { get; set; }
        public string filePath { get; set; }
        public string extension { get; set; }
        public string originName { get; set; }
        public string dbPath { get; set; }        
    }

    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public long size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteType { get; set; }
    }

    public class JsonFiles
    {
        public ViewDataUploadFilesResult[] files;
        public string TempFolder { get; set; }
        public JsonFiles(List<ViewDataUploadFilesResult> filesList)
        {
            files = new ViewDataUploadFilesResult[filesList.Count];
            for (int i = 0; i < filesList.Count; i++)
            {
                files[i] = filesList.ElementAt(i);
            }

        }
    }

}
