using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Univision.Security;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using System.Configuration;
using Univision.Main.Infrastructure.SMS;
using System.Security.Cryptography;
using System.Text;
using Univision.Main.Infrastructure.CustomFilters;

namespace Univision.Main.Controllers
{
  [Authorize]
  public class AccountController : Controller
  {


    // GET: Account
    IAuthenticationManager Authentication
    {
      get { return HttpContext.GetOwinContext().Authentication; }
    }
    private int GetExternal()
    {
      string allowedIps = ConfigurationManager.AppSettings["allowedIPs"].Replace(" ", "").Trim();

      //, 구분으로 허용 ip string에서 자름.
      string[] ips = allowedIps.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
      string currentIp = HttpContext.Request.UserHostAddress;

      int isExternal = 1;

      foreach (var ip in ips)
      {
        //허용된 IP의 경우 통과
        string[] addr = ip.Split('/');

        if (addr.Length != 2)
        {
          if (ip.Equals(currentIp))
          {
            isExternal = 0;
            break;
          }
        }
        else
        {
          if (new IPSubnet(ip).Contains(currentIp))
          {
            isExternal = 0;
            break;
          }
        }
      }

      return isExternal;
    }

    [AllowAnonymous]
    public ActionResult Index(string returnUrl)
    {
      if (AppIdentity.user_seq > 0)
      {
        if (!String.IsNullOrEmpty(returnUrl))
          return Redirect(returnUrl);
        else
          return RedirectToAction("Index", "Dashboard");
      }

      

      ViewBag.returnUrl = returnUrl;
      ViewBag.isExternal = GetExternal();
      ;
      return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<JsonResult> Login(string id, string pwd, string isExternal, int a_seq = 0, string code = "", int isRemember = 0)
    {
      AccountRepository re = new AccountRepository();

      uv_user user = await re.SelectUser(id, pwd, 1);

      if (user == null)
        return Json(new { isLogin = false });
      else
          if (user.is_out_login != 1 && GetExternal() == 1)
        return Json(new { isLogin = false, message = "외부접속이 허용되지 않는 계정입니다." });

      int isAuth = 0;
      if (isExternal == "1")
      {
        AccountEntityRepository acr = new AccountEntityRepository();
        var authCode = await acr.SelectAuthCodeOneAsync(a_seq, id);

        if (authCode == null)
          return Json(new
          {
            isLogin = false,
            message = "외부 접속시, SMS 인증을 받으셔야 합니다."
          });

        if (authCode.auth_code != code)
          return Json(new
          {
            isLogin = false,
            message = "인증코드가 일치하지 않습니다.<br />올바른 본인확인 인증번호를 입력하세요."
          });

        isAuth = 1;
      }


      userAuth auth = new userAuth();
      ClaimsIdentity identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.NameIdentifier, ClaimTypes.Role);
      auth.SetIdentity(id, identity);

      string entry_date = string.Empty;

      if (user.entry_date.HasValue)
        entry_date = user.entry_date.Value.ToString("yyyy-MM-dd");

      string img_dir = string.Empty;
      string img_origin_path = string.Empty;
      string img_path = string.Empty;
      string name = string.Empty;


      if (!string.IsNullOrWhiteSpace(user.img_dir))
        img_dir = user.img_dir;
      if (!string.IsNullOrWhiteSpace(user.img_origin_path))
        img_origin_path = user.img_origin_path;
      if (!string.IsNullOrWhiteSpace(user.img_path))
        img_path = user.img_path;
      if (!string.IsNullOrWhiteSpace(user.name))
        name = user.name;


      identity.AddClaim(new Claim(UniClaimTypes.login_id, user.user_id));
      identity.AddClaim(new Claim(UniClaimTypes.rank_name, user.rank_name));
      identity.AddClaim(new Claim(UniClaimTypes.email, user.email));
      identity.AddClaim(new Claim(UniClaimTypes.tel, user.tel));
      identity.AddClaim(new Claim(UniClaimTypes.cellphone, user.hp));
      identity.AddClaim(new Claim(UniClaimTypes.entry_date, entry_date));
      identity.AddClaim(new Claim(UniClaimTypes.img_dir, img_dir));
      identity.AddClaim(new Claim(UniClaimTypes.img_origin_path, img_origin_path));
      identity.AddClaim(new Claim(UniClaimTypes.img_path, img_path));
      //identity.AddClaim(new Claim(UniClaimTypes.level, user.level.ToString()));
      identity.AddClaim(new Claim(UniClaimTypes.name, name));
      identity.AddClaim(new Claim(UniClaimTypes.ua_seq, user.ua_seq.ToString()));
      identity.AddClaim(new Claim(UniClaimTypes.ud_seq, user.ud_seq.ToString()));
      identity.AddClaim(new Claim(UniClaimTypes.user_seq, user.uv_seq.ToString()));
      identity.AddClaim(new Claim(UniClaimTypes.uv_seq, user.uv_seq.ToString()));

      
      if (user.ud_seq == 4 || user.ud_seq == 10 || user.ud_seq == 47)
      {
        identity.AddClaim(new Claim(UniClaimTypes.leader_seq, user.uv_seq.ToString()));
      }
      else
      {
        identity.AddClaim(new Claim(UniClaimTypes.leader_seq, user.leader_seq.ToString()));
      }


      identity.AddClaim(new Claim(UniClaimTypes.s_confirm_seq, user.s_confirm_seq.ToString()));
      identity.AddClaim(new Claim(UniClaimTypes.s_leader_seq, user.s_leader_seq.ToString()));

      identity.AddClaim(new Claim(UniClaimTypes.isExternal, isExternal));
      identity.AddClaim(new Claim(UniClaimTypes.isAuth, isAuth.ToString()));

      Response.Cookies["txt_user_name"].Value = HttpUtility.UrlEncode(name);
      Response.Cookies["txt_user_name"].Expires = DateTime.Now.AddDays(1);
      if (isRemember == 1)
      {
        Response.Cookies["remember"].Value = isRemember.ToString();
        Response.Cookies["remember"].Expires = DateTime.Now.AddDays(30);
        Response.Cookies["remember_id"].Value = id;
        Response.Cookies["remember_id"].Expires = DateTime.Now.AddDays(30);
      }
      else
      {
        Response.Cookies["remember"].Value = "0";
        Response.Cookies["remember_id"].Value = String.Empty;
      }

      var authenticationContext = await Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie);

      if (authenticationContext != null)
      {
        Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(identity, authenticationContext.Properties);
      }

      Authentication.SignIn(new AuthenticationProperties
      {
        IsPersistent = true
      },
      identity);

      //로그인 로그 남김.
      uv_login_history history = new uv_login_history()
      {
        uv_seq = user.uv_seq,
        login_dt = Utils.NowKorea(),
        login_ip = HttpContext.Request.UserHostAddress.ToString()
      };

      ApiEntityRepository aer = new ApiEntityRepository();
      await aer.CreateLoginHistory(history);

      return Json(new { isLogin = true });
    }

    public ActionResult LogOut()
    {
      Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
      return RedirectToAction("Index", "Account");
    }


    public async Task<JsonResult> FindDivisionUserList(int ud_seq)
    {
      try
      {
        AccountRepository ar = new AccountRepository();

        var list = await ar.SelectUserListByDivision(ud_seq);

        return Json(new
        {
          ok = true,
          list = list.Select(x => new
          {
            uv_seq = x.uv_seq,
            name = x.name
          }).ToList()
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "부서에 속한 사용자 정보를 불러오는데 실패 했습니다." + e.Message
        });
      }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<JsonResult> SendSmsAuthCode(string id, string pwd)
    {
      try
      {
        AccountRepository re = new AccountRepository();
        uv_user user = await re.SelectUser(id, pwd, 1);

        if (user == null)
        {
          return Json(new
          {
            ok = false,
            message = "아이디 또는 패스워드가 틀립니다."
          });
        }
        else
        {
          if (user.is_out_login != 1 && GetExternal() == 1)
          {
            return Json(new { isLogin = false, message = "외부접속이 허용되지 않는 계정입니다." });
          }

        }


        //정규식으로 휴대폰번호 검사
        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("01[016789]\\-\\d{3,4}\\-\\d{3,4}");

        if (!r.IsMatch(user.hp)) 
          return Json(new
          {
            ok = false,
            message = "등록된 휴대폰 번호가 올바르지 않습니다.<br />관리자에게 문의 하세요."
          });

        Random random = new Random();
        string authCode = random.Next(123456, 987654).ToString();

        SmsService service = new SmsService();
        var result = await service.SendSmsAsyncNew(new SmsSingleSendDto()
        {
          PhoneSender = ConfigurationManager.AppSettings["companyPhoneNumber"].ToString(),
          PhoneReceiver = user.hp,
          smsType = 0,
          Subject = "[(주)유니코써치] 본인확인 인증문자",
          Message = string.Format("[(주)유니코써치] 본인확인 인증번호는 [{0}] 입니다. 정확하게 입력해주세요.", authCode),
        });

        //발송 성공
        if (result.response_code == "R000")
        {
          AccountEntityRepository aer = new AccountEntityRepository();
          var auth = new auth_code_temp
          {
            user_id = id,
            auth_code = authCode,
            reg_dt = Utils.NowKorea()
          };

          await aer.CreateAuthCodeOneAsync(auth);

          return Json(new
          {
            ok = true,
            message = "인증번호가 발송 되었습니다.",
            a_seq = auth.a_seq
          });
        }
        //발송 실패
        else
          return Json(new
          {
            ok = false,
            message = string.Format("인증문자 발송에 실패 했습니다.<br />오류코드:{0} 오류메세지:{1}", result.response_code, result.response_desc)
          });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = string.Format("인증문자 발송 도중 서버 오류가 발생 했습니다.<br />{0}", e.Message)
        });
      }

    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<JsonResult> CheckAuthCode(int a_seq, string id, string code)
    {
      try
      {
        AccountEntityRepository aer = new AccountEntityRepository();
        var auth = await aer.SelectAuthCodeOneAsync(a_seq, id);

        if (auth == null)
          return Json(new
          {
            ok = false,
            message = "외부 접속시, SMS 인증을 받으셔야 합니다."
          });

        if (auth.auth_code != code)
          return Json(new
          {
            ok = false,
            message = "인증코드가 일치하지 않습니다.<br />올바른 본인확인 인증번호를 입력하세요."
          });

        return Json(new
        {
          ok = true,
          message = "인증 되었습니다."
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = string.Format("인증코드 확인 중, 서버 오류가 발생 했습니다.<br />{0}", e.Message)
        });
      }

    }

    /// <summary>
    /// 암호화
    /// </summary>
    /// <param name="phrase"></param>
    /// <returns></returns>

    string ComputeSha256Hash(string rawData)
    {
      // Create a SHA256   
      using (SHA256 sha256Hash = SHA256.Create())
      {
        // ComputeHash - returns byte array  
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // Convert byte array to a string   
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
          builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
      }
    }
  }
}