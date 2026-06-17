using System;
using System.IO;
using System.Web;
using Univision.Security;

namespace Univision.Main.Infrastructure
{
  /// <summary>
  /// UploadedFiles 폴더 직접 접근 차단 HTTP 모듈
  ///
  /// AppIdentity.user_seq = HttpContext.Current.User.Identity(ClaimsIdentity)에서 읽음
  /// OWIN 쿠키 인증이 ClaimsIdentity를 설정한 뒤, 핸들러 실행 직전인
  /// PreRequestHandlerExecute 시점에는 AppIdentity.user_seq가 정상 반환됨
  ///
  /// 로그 위치: D:\Log\upload_auth_yyyyMMdd.log (하루 1파일)
  /// </summary>
  public class UploadedFileAuthModule : IHttpModule
  {
    private static readonly object _logLock = new object();

    public void Init(HttpApplication context)
    {
      // PreRequestHandlerExecute:
      //   - OWIN 인증 완료 후 → ClaimsIdentity 정상 설정
      //   - StaticFile 핸들러 포함 모든 요청에서 발화 (runAllManagedModulesForAllRequests="true" 필요)
      //   - AppIdentity.user_seq 정상 작동
      context.PreRequestHandlerExecute += CheckUploadedFileAccess;
    }

    private void CheckUploadedFileAccess(object sender, EventArgs e)
    {
      var app = (HttpApplication)sender;
      var path = app.Request.AppRelativeCurrentExecutionFilePath;

      // UploadedFiles 하위 경로만 검사
      if (!path.StartsWith("~/UploadedFiles/", StringComparison.OrdinalIgnoreCase))
        return;

      // 로그인 여부 확인 — AppIdentity가 null일 경우 대비 try/catch
      bool isLoggedIn = false;
      int userSeq = 0;
      try
      {
        userSeq = AppIdentity.user_seq;
        isLoggedIn = (userSeq > 0);
      }
      catch
      {
        // ClaimsIdentity 없음 → 비로그인으로 처리
        isLoggedIn = false;
      }

      WriteLog(app, path, userSeq, isLoggedIn ? "ALLOW" : "BLOCK");

      if (!isLoggedIn)
      {
        app.Response.StatusCode = 403;
        app.Response.StatusDescription = "Forbidden";
        app.Response.SuppressContent = true;
        app.CompleteRequest();
      }
      else
      {
        // 브라우저 캐싱 방지: 로그아웃 후 재접근 시 반드시 서버 재요청하도록 강제
        app.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        app.Response.Cache.SetNoStore();
        app.Response.Cache.AppendCacheExtension("must-revalidate");
        app.Response.AddHeader("Pragma", "no-cache");
        app.Response.AddHeader("Expires", "0");
      }
    }

    // ─────────────────────────────────────────────
    //  로그: D:\Log\upload_auth_yyyyMMdd.log
    // ─────────────────────────────────────────────
    private static void WriteLog(HttpApplication app, string path, int userSeq, string result)
    {
      try
      {
        string logDir = @"D:\Log";
        string logFile = Path.Combine(logDir, "upload_auth_" + DateTime.Now.ToString("yyyyMMdd") + ".log");

        if (!Directory.Exists(logDir))
          Directory.CreateDirectory(logDir);

        string ip = app.Request.UserHostAddress ?? "-";
        string line = string.Format(
          "{0:yyyy-MM-dd HH:mm:ss} | {1,-5} | user_seq={2,6} | {3,-15} | {4}\r\n",
          DateTime.Now, result, userSeq, ip, path);

        lock (_logLock)
        {
          File.AppendAllText(logFile, line, System.Text.Encoding.UTF8);
        }
      }
      catch
      {
        // 로그 실패는 무시 (본 기능에 영향 없도록)
      }
    }

    public void Dispose() { }
  }
}
