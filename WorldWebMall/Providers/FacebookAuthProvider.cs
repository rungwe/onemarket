using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Facebook;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security;
using App.Extensions;
using Facebook;
using System.Dynamic;
using WorldWebMall.SocialNetworks;
//using WorldWebMall.Models;


namespace WorldWebMall.Providers
{
    
    public class FacebookAuthProvider : FacebookAuthenticationProvider
    {
        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            //context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            string extendedToken = GetExtendedAccessToken(context.AccessToken);
            context.Identity.SetFacebookToken(extendedToken);
            
            return Task.FromResult<object>(null);
        }

        public string GetExtendedAccessToken(string token) 
        { 
            dynamic result = new ExpandoObject(); 
            try 
            {
                var api = new FacebookClient(token); 
                dynamic parameters = new ExpandoObject(); 
                parameters.grant_type = "fb_exchange_token"; 
                parameters.fb_exchange_token = token;
                parameters.client_id = "442382459306288";
                parameters.client_secret = "d69c8e0ffd64d4ac57ccea0626830a41"; 
                result = api.Get("oauth/access_token", parameters); 
            } 
            catch (FacebookOAuthException err) 
            { 
                result.error = "Error"; 
                result.message = err.Message; 
            } 
            /**catch (Exception err) 
            { 
                result.error = "Error"; 
                result.message = err.Message; 
            }*/ 
            return result.access_token; 
        }

    }

}