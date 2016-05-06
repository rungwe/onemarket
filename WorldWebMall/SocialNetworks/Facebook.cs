using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.AspNet.Identity;
using WorldWebMall.Models;
using Facebook;
using Facebook.Reflection;
using System.Security.Principal;
using App.Extensions;
//using System.Dynamic;
using System.IO;

namespace WorldWebMall.SocialNetworks
{
    //[Authorize]
    public static class Facebook
    {
        private static MallContext db = new MallContext();
        private static string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        public static List<string> GetPageNames(this IIdentity identity)
        {
            List<string> names = new List<string>();
            var face = new FacebookClient(identity.GetFacebookToken());
            var page = (JsonObject)face.Get("me/accounts");
            foreach (var c in (JsonArray)page["data"])
            {
                names.Add( (string)(((JsonObject)c)["name"]) );
            }
            return names;
        }
        public static HttpResponseMessage TestRD(this IIdentity identity)
        {
            Company c = new Company();
            c.name = "Tariro";
            var response = new HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Content = new ObjectContent<Company>(c, new JsonMediaTypeFormatter(), "application/json");
            response.Headers.Location = new Uri(baseUrl + "/external/get-facebook");

            

            return response;
        }
        public static string GetPageAccessToken(this IIdentity identity, string pageName)
        {
            IList<string> names = new List<string>();
            var face = new FacebookClient(identity.GetFacebookToken());
            var page = (JsonObject)face.Get("me/accounts");
            foreach (var c in (JsonArray)page["data"])
            {
                if ( (string)(((JsonObject)c)["name"]) == pageName)
                {
                    string pageToken = (string)(((JsonObject)c)["access_token"]);
                    string Id = (string)(((JsonObject)c)["id"]);
                    identity.SetFacebookToken(pageToken);
                    identity.SetFBPageId(Id);
                    return (string)(((JsonObject)c)["access_token"]);
                }
                    
                
            }
            return null;
        }
        
        public static dynamic PostFacebookBroadcast(this IIdentity identity)
        {
            string attachPath = "C:/Users/tariro/Pictures/From Tariro/Saved pictures/130748761451106643.jpg";
            var fb = new FacebookClient(identity.GetFacebookToken());
            dynamic result = fb.Post(identity.GetFBPageId() + "/photos" , 
                new {
                    message = "my first photo upload using Facebook SDK for .NET",
                    file = new FacebookMediaObject { 
                        ContentType = "image/jpeg", 
                        FileName = Path.GetFileName(attachPath) 
                    }.SetValue(File.ReadAllBytes(attachPath)),
                    url = ""
            });

            return result;
        }

        public static string GetFacebookProfile(this IIdentity identity)
        {
            var face = new FacebookClient(identity.GetFacebookToken());//
            dynamic me = face.Get("me");
            string name = me.name;
            return name;
        }
        /**
        public async Task<IHttpActionResult> PostFacebookComment(this IIdentity identity, string comment, string id)
        {

        }

        public async Task<IHttpActionResult> PutFacebookLike(this IIdentity identity, string id)
        {

        }

        public async Task<IHttpActionResult> GetFacebookBroadcast(this IIdentity identity, string id)
        {

        }

        public async Task<IHttpActionResult> GetFacebookBroadcasts(this IIdentity identity, string id, int amount, int page)
        {

        }

        public async Task<IHttpActionResult> GetFacebookComments(this IIdentity identity, string id, int amount, int page)
        {

        }

        public async Task<IHttpActionResult> GetFacebookLikes(this IIdentity identity, string id, int amount, int page)
        {

        }

        public async Task<IHttpActionResult> GetFacebookNumberOfLikes(this IIdentity identity, string id)
        {

        }

        public async Task<IHttpActionResult> GetFacebookNumberOfComments(this IIdentity identity, string id)
        {

        }

        public async Task<IHttpActionResult> GetFacebookNumberOfShares(this IIdentity identity, string id)
        {

        }
        */
    }
}