using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Univision.Core.Models;

namespace Univision.Security
{
    public class userAuth : Controller
    {
        public void SetIdentity(string user, ClaimsIdentity identity)
        {
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.login_id));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.user_seq));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.email));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.entry_date));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.img_dir));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.img_origin_path));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.img_path));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.level));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.login_id));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.name));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.rank_name));

            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.ua_seq));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.ud_seq));
            identity.TryRemoveClaim(identity.FindFirst(UniClaimTypes.name));
        }
    }
}
