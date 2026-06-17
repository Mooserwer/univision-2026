using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;


namespace Univision.Main
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Index"),
                CookieName = "Univision.Auth",
                ExpireTimeSpan = TimeSpan.FromHours(8)
            });

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ExternalBearer,
                AuthenticationMode = AuthenticationMode.Passive
            });

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                Provider = new OAuthBearerWithQueryAuthenticationProvider()
            });
        }
    }
    public class OAuthBearerWithQueryAuthenticationProvider
 : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (String.IsNullOrWhiteSpace(context.Token))
            {
                var tokenQuery = context.OwinContext.Request.Query.Get("Bearer");
                if (!string.IsNullOrEmpty(tokenQuery))
                    context.Token = tokenQuery;
            }
            return Task.FromResult<object>(null);
        }

    }
}
