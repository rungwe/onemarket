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


namespace WorldWebMall.SocialNetworks
{
    [Authorize]
    public class LinkedIn
    {
        /**
        private static string baseUrl = "https://api.linkedin.com/v1/companies/";
        
        public async Task<IHttpActionResult> PostBroadcast(this IIdentity identity , CreateB obj)
        {
            string url = baseUrl + identity.GetLIPageId() + "/shares?format=json";

            return null;
        } 

        public async Task<IHttpActionResult> PostComment(this IIdentity identity, string key, string comment)
        {
            string url = baseUrl + identity.GetLIPageId() + "/updates/key=" + key "/update-comments-as-company/";
        }

        public async Task<IHttpActionResult> GetNumFolloers(this IIdentity identity)
        {
            string url = baseUrl + identity.GetLIPageId() + "/num-followers?format=json";
        }

        public async Task<IHttpActionResult> GetBroadcast(this IIdentity identity , string key)
        {
            string url = baseUrl + identity.GetLIPageId() + "updates/key=" + key + "?format=json";
        }

        public async Task<IHttpActionResult> GetBroadcasts(this IIdentity identity, int amount, int page)
        {
            string url = identity.GetLIPageId() + "updates?format=json";
        }

        public async Task<IHttpActionResult> GetComments(this IIdentity identity, string key, int amount, int page)
        {
            string url = baseUrl + identity.GetLIPageId() + "/updates/key=" + key + "/update-comments?format=json";
        }

        public async Task<IHttpActionResult> GetLikes(this IIdentity identity, string key, int amount, int page)
        {
            string url = baseUrl + identity.GetLIPageId() + "/updates/key=" + key + "/likes?format=json";
        }
        */
    }
}