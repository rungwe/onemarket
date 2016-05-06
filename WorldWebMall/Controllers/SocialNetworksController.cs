using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WorldWebMall.Models;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Http.Cors;
using App.Extensions;
using WorldWebMall.SocialNetworks;
using Facebook;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace WorldWebMall.Controllers
{
    [RoutePrefix("external")]
    public class SocialNetworksController : ApiController
    {
        private MallContext db = new MallContext();
        static ApplicationUserManager manager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

        [Route("get-facebook-pages")]
        public IHttpActionResult GetFBPages()
        {
            try
            {
                var result = User.Identity.GetPageNames();
                return Ok(result);
            }
            catch
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            
        }

        [Route("get-facebook")]
        public HttpResponseMessage GetFBPages(HttpResponseMessage response)
        {
            /**
            try
            {
                var result = User.Identity.GetPageNames();
                return Ok(result);
            }
            catch
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            */
            //var response = new HttpResponseMessage();
            //response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        [Route("get-facebook-page-access-token")]
        public async Task<IHttpActionResult> GetPageAccessToken(string pageName)
        {
            /**
            try
            {
                string token = User.Identity.GetPageAccessToken(pageName);
                if (token == null)
                    return BadRequest("Page name not found");
                var fb = new FacebookPageTokens()
                {
                    CompanyId = User.Identity.GetUserId(),
                    PageName = pageName,
                    Token = token,
                    exp = DateTime.UtcNow
                };
                db.Entry(fb).State = EntityState.Added;
                
                await db.SaveChangesAsync();
                
            }
            catch
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
             */
            return Ok(User.Identity.GetPageAccessToken(pageName));
        }

        [Route("linkedin_authorization")]
        public void GetLinkedInAuth(string code, string state , string error = null , string error_description = null)
        {
            //manager.FindByIdAsync();
        }

    }
}
