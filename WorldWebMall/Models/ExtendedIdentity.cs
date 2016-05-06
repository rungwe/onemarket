using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Principal;
using WorldWebMall.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WorldWebMall;

namespace App.Extensions
{
    public static class ExtendedIdentity
    {
        static ApplicationUserManager manager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

        public static string GetFacebookToken(this IIdentity identity) 
        {
            return manager.FindByName(identity.GetUserName()).FacebookToken;
        }

        public static string GetTwitterToken(this IIdentity identity)
        {
            return manager.FindByName(identity.GetUserName()).TwitterToken;
        }

        public static string GetLinkedInToken(this IIdentity identity)
        {
            return manager.FindByName(identity.GetUserName()).LinkedInToken;
        }

        public static string GetFBPageId(this IIdentity identity)
        {
            return manager.FindByName(identity.GetUserName()).FBPageId;
        }

        public static string GetLIPageId(this IIdentity identity)
        {
            return manager.FindByName(identity.GetUserName()).FBPageId;
        }

        public static void SetFacebookToken(this IIdentity identity , string token)
        {
            var user = manager.FindByName("customer30@yahoo.com");
            //var user = manager.FindByName(identity.GetUserName());
            user.FacebookToken = token;
            manager.Update(user);
        }

        public static void SetLinkedInToken(this IIdentity identity, string token)
        {
            var user = manager.FindByName(identity.GetUserName());
            user.LinkedInToken = token;
            manager.Update(user);
        }

        public static void SetTwitterToken(this IIdentity identity, string token)
        {
            var user = manager.FindByName("customer30@yahoo.com");
            //var user = manager.FindByName(identity.GetUserName());
            user.TwitterToken = token;
            manager.Update(user);
        }

        public static void SetFBPageId(this IIdentity identity, string Id)
        {
            var user = manager.FindByName(identity.GetUserName());
            user.FBPageId = Id;
            manager.Update(user);
        }
        
    }
}