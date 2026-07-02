using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Univision.Security
{
    public class AppIdentity
    {
        public static ClaimsIdentity ManualIdentityClaims;

        protected static ClaimsIdentity CurrentIdentity
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return ManualIdentityClaims;
                }
                return HttpContext.Current.User.Identity as ClaimsIdentity;
            }
        }
        /**
         * 
         *            identity.AddClaim(new Claim(UniClaimTypes.login_id, user.user_id));
            identity.AddClaim(new Claim(UniClaimTypes.email, user.email));
            identity.AddClaim(new Claim(UniClaimTypes.entry_date, entry_date));
            identity.AddClaim(new Claim(UniClaimTypes.img_dir, user.img_dir));
            identity.AddClaim(new Claim(UniClaimTypes.img_origin_path, user.img_origin_path));
            identity.AddClaim(new Claim(UniClaimTypes.img_path, user.img_path));
            identity.AddClaim(new Claim(UniClaimTypes.level, user.level.ToString()));
            identity.AddClaim(new Claim(UniClaimTypes.name, user.name));
            identity.AddClaim(new Claim(UniClaimTypes.ua_seq, user.ua_seq.ToString()));
            identity.AddClaim(new Claim(UniClaimTypes.ud_seq, user.ud_seq.ToString())); 
         */


        public static string LoginID
        {
            get
            {
                return CurrentIdentity.FindFirstValue(UniClaimTypes.login_id);
            }
        }

        public static string email
        {
            get
            {
                return CurrentIdentity.FindFirstValue(UniClaimTypes.email);
            }
        }

        public static string tel
        {
            get
            {
                return CurrentIdentity.FindFirstValue(UniClaimTypes.tel);
            }
        }

        public static string cellphone
        {
            get
            {
                return CurrentIdentity.FindFirstValue(UniClaimTypes.cellphone);
            }
        }

        public static string entry_date
        {
            get
            {
                return CurrentIdentity.FindFirstValue(UniClaimTypes.entry_date);
            }
        }

        public static string img_dir
        {
            get
            {
                return CurrentIdentity.FindFirstValue(UniClaimTypes.img_dir);
            }
        }
        public static string img_origin_path
        {
            get
            {
                return CurrentIdentity.FindFirstValue(UniClaimTypes.img_origin_path);
            }
        }

        public static string img_path
        {
            get
            {
                return CurrentIdentity.FindFirstValue(UniClaimTypes.img_path);
            }
        }

        public static int level
        {
            get
            {
                int seq = 0;
                int.TryParse(CurrentIdentity.FindFirstValue(UniClaimTypes.level), out seq);
                return seq;
            }
        }


        public static string name
        {
            get
            {
                return CurrentIdentity.FindFirstValue(UniClaimTypes.name);
            }
        }

        public static int ua_seq
        {
            get
            {
                int seq = 0;
                int.TryParse(CurrentIdentity.FindFirstValue(UniClaimTypes.ua_seq), out seq);
                return seq;
            }
        }

        public static int ud_seq
        {
            get
            {
                int seq = 0;
                int.TryParse(CurrentIdentity.FindFirstValue(UniClaimTypes.ud_seq), out seq);
                return seq;
            }
        }

        public static int user_seq
        {
            get
            {
                int seq = 0;
                int.TryParse(CurrentIdentity.FindFirstValue(UniClaimTypes.user_seq), out seq);
                return seq;
            }
        }
        /// <summary>
        /// 부서장
        /// </summary>
        public static int leader_seq
        {
            get
            {
                int seq = 0;
                int.TryParse(CurrentIdentity.FindFirstValue(UniClaimTypes.leader_seq), out seq);
                return seq;
            }
        }
        /// <summary>
        /// 경지담당
        /// </summary>
        public static int s_confirm_seq
        {
            get
            {
                int seq = 0;
                int.TryParse(CurrentIdentity.FindFirstValue(UniClaimTypes.s_confirm_seq), out seq);
                return seq;
            }
        }
        /// <summary>
        /// 경영팀장
        /// </summary>
        public static int s_leader_seq
        {
            get
            {
                int seq = 0;
                int.TryParse(CurrentIdentity.FindFirstValue(UniClaimTypes.s_leader_seq), out seq);
                return seq;
            }
        }


        public static bool isLogin
        {
            get
            {
                if (user_seq == 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// PMS 오픈(2026-07-13) 관련 고객사/후보자/프로젝트 등록·수정·삭제 허용 여부.
        /// 오픈 전에는 전체 허용, 오픈 이후에는 관리 권한(ua_seq 1~3)만 허용.
        /// </summary>
        public static bool CanPmsEdit
        {
            get
            {
                DateTime pmsOpen = new DateTime(2026, 7, 13);
                if (DateTime.Now < pmsOpen)
                    return true;
                return ua_seq > 0 && ua_seq <= 3;
            }
        }

        /// <summary>
        /// 외부접속 여부.
        /// </summary>
        public static int isExternal
        {
            get
            {
                int seq = 0;
                int.TryParse(CurrentIdentity.FindFirstValue(UniClaimTypes.isExternal), out seq);
                return seq;
            }
        }

        /// <summary>
        /// 외부접속 인증여부.
        /// </summary>
        public static int isAuth
        {
            get
            {
                int seq = 0;
                int.TryParse(CurrentIdentity.FindFirstValue(UniClaimTypes.isAuth), out seq);
                return seq;
            }
        }
    }
}
