using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Twitter;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security;
using App.Extensions;
using Facebook;
using System.Dynamic;
using WorldWebMall.SocialNetworks;

namespace WorldWebMall.Providers
{
    public class TwitterAuthProvider : TwitterAuthenticationProvider
    {
        public const string AccessToken = "TwitterAccessToken";
        public const string AccessTokenSecret = "TwitterAccessTokenSecret";

        public override Task Authenticated(TwitterAuthenticatedContext context)
        {
            /**context.Identity.AddClaims(
                new List<Claim>
            {
                new Claim(AccessToken, context.AccessToken),
                new Claim(AccessTokenSecret, context.AccessTokenSecret)
            });
            */
            context.Identity.SetTwitterToken(context.AccessToken);
            TwitterClient client = new TwitterClient();
            client.Authorize(context.AccessToken);
            //context.AccessTokenSecret;
            return null;//base.Authenticated(context);
        }
    }
}