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
using System.Dynamic;
using System.IO;
using System.Net.Http.Headers;


namespace WorldWebMall.SocialNetworks
{
    public static class Twitter
    {
        private static string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        

        
        /*
        public static dynamic PostTwitterBroadcast(this IIdentity identity)
        {
           
            return result;
        }
        */
        public static string GetTwitterProfile(this IIdentity identity)
        {
            var face = new TwitterClient(identity.GetTwitterToken());//
            Company c = new Company();
            HttpResponseMessage response = new HttpResponseMessage();
            
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Content = new ObjectContent<Company>(c, new JsonMediaTypeFormatter(), "application/json");
            response.Headers.Location = new Uri(baseUrl + "/external/get-facebook");
            return "";
        }
        /**
        public async Task<IHttpActionResult> PostTwitterComment(this IIdentity identity, string comment, string id)
        {

        }

        public async Task<IHttpActionResult> PutTwitterLike(this IIdentity identity, string id)
        {

        }

        public async Task<IHttpActionResult> GetTwitterBroadcast(this IIdentity identity, string id)
        {

        }

        public async Task<IHttpActionResult> GetTwitterBroadcasts(this IIdentity identity, string id, int amount, int page)
        {

        }

        public async Task<IHttpActionResult> GetTwitterComments(this IIdentity identity, string id, int amount, int page)
        {

        }

        public async Task<IHttpActionResult> GetTwitterLikes(this IIdentity identity, string id, int amount, int page)
        {

        }

        public async Task<IHttpActionResult> GetTwitterNumberOfLikes(this IIdentity identity, string id)
        {

        }

        public async Task<IHttpActionResult> GetTwitterNumberOfComments(this IIdentity identity, string id)
        {

        }

        public async Task<IHttpActionResult> GetTwitterNumberOfShares(this IIdentity identity, string id)
        {

        }
        */
    }
}