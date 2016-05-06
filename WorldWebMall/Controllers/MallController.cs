using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Spatial;
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
using BitCode.Owin.Security.LinkedIn;


namespace WorldWebMall.Controllers
{
    [Authorize] // testing repos
    [RoutePrefix("customer")]
    //[EnableCors(origins: "http://ec2-52-88-34-32.us-west-2.compute.amazonaws.com", headers: "*", methods: "*")]
    public class MallController : ApiController
    {
        private static MallContext db = new MallContext();
        private model_test dbt = new model_test();
       
        // GET: api/Mall
        [Route("get-user-details")]
        [ResponseType(typeof(CustomerDetails))]
        public IHttpActionResult GetUserDetailed(string id)
        {
            UserManager<ApplicationUser, string> manager = new UserManager<ApplicationUser, string>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            IList<string> roles = UserManagerExtensions.GetRoles(manager, id);
            
            
            if (roles == null)
            {
                return NotFound();
            }
            if (roles.Contains("customer"))
            {
                var customer = db.Customers.Where(c => c.CustomerId == id)
                                    .Project().To<CustomerDetails>();
                return Ok(customer);
            }
            else
            {
                var company = db.Companies.Where(c => c.CompanyId == id)
                                    .Project().To<CustomerDetails>();
                return Ok(company);
            }
            
        }
        
        [Route("get-string")]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetString()
        {
            
            Company c = new Company();
            //User.Identity.SetFacebookToken("zvaita");
            TwitterClient client = new TwitterClient();
            LinkedInAuthenticationProvider auth = new LinkedInAuthenticationProvider();
            return Ok(auth.Authorize());//Ok(User.Identity.GetFacebookToken());
            
        }
        /// <summary>
        /// returns status code 200
        /// </summary>
        /// <param name="page"></param>
        /// <param name="amount"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [Route("get-B")]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetStringB(string token)
        {

            TwitterClient client = new TwitterClient();
            LinkedInAuthenticationProvider auth = new LinkedInAuthenticationProvider();
            //client.Authorize(token);
            return Ok(auth.GetAccessToken(token));
        }

        [Route("test-table")]
        [ResponseType(typeof(Test))]
        public IHttpActionResult GetTest()
        {
            var advert = db.Adverts;

            return Ok(advert);
        }

        [Route("get-test-ad")]
        [ResponseType(typeof(Test))]
        public IHttpActionResult GetAdTest()
        {
            var t = db.Tests;
            return Ok(t);
        }

        [Route("get-testv-table")]
        [ResponseType(typeof(Test))]
        public IHttpActionResult GetTestV()
        {
            var t = db.TestVers;
            return Ok(t);
        }

        [Route("get-user")]
        public IHttpActionResult GetUser(string id)
        {
            UserManager<ApplicationUser, string> manager = new UserManager<ApplicationUser, string>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            
            IList<string> roles = UserManagerExtensions.GetRoles(manager, id);
           
            if (roles == null)
            {
                return NotFound();
            }
            if (roles.Contains("customer"))
            {
                var customer = db.Customers.Where(c => c.CustomerId == id)
                                    .Project().To<CustomerDTO>();
                return Ok(customer);
            }
            else
            {
                var company = db.Companies.Where(c => c.CompanyId == id)
                                    .Project().To<CustomerDTO>();
                return Ok(company);
            }

        }

        /**
         * Broadcasts Logic comes here
         */
          
        //Get Broadcasts and Refresh them as well
        //To refresh time should be null
        [Route("get-broadcasts")]
        [ResponseType(typeof(IQueryable<BroadcastDTO>))]
        public async Task<IHttpActionResult> GetBroadcast(int page, int amount, DateTime? time = null)
        {

            
            string UserId = User.Identity.GetUserId();

            //IQueryable<BroadcastDTO>
            var result = db.Broadcasts.Where(o => o.time <= (time != null ? time : DateTime.UtcNow) & o.Customers.Any(s => s.CustomerId.Equals(UserId)))
                    .OrderBy(a => a.time).Skip(amount * (page - 1)).Take(amount)
                    .Project().To<BroadcastDTO>().ToList<BroadcastDTO>();

            if (result == null)
            {
                return NotFound();
            }

            foreach (var p in result)
            {
                p.liked = db.BLikes.Find(p.ID, UserId) != null ? true : false;
            }
            //update number of views
                
            return Ok(result);
        }

        //Put a like on broadcast
        [Route("like-broadcast")]
        public async Task<IHttpActionResult> PutLikeB(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Broadcast broad = await db.Broadcasts.FindAsync(id);
                string UserId = User.Identity.GetUserId();
                //check for advert not found
                if (broad == null || id != broad.BroadcastId)
                {
                    return BadRequest();
                }

                BLike duplicate = await db.BLikes.FindAsync(id, UserId);
                if (duplicate != null)
                {
                    return StatusCode(HttpStatusCode.Conflict);
                }
                broad.BLikes.Add(new BLike
                {
                    UserId = UserId,
                    CustomerId = UserId,
                    BroadcastId = id,
                    time = DateTime.UtcNow
                });

                CompanyNotification notification = new CompanyNotification()
                {
                    BroadcastId = id,
                    time = DateTime.UtcNow,
                    type = "like",
                    seen = false,
                    CustomerId = UserId,
                    CompanyId = broad.CompanyId
                };

                db.CompanyNotifications.Add(notification);
                db.Entry(notification).State = EntityState.Added;
                db.Entry(broad).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //read on what to put there
                    /**if (!(id))
                    {
                        return NotFound();
                    }
                    else*/
                    {
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(e);
            }

            return StatusCode(HttpStatusCode.Created);
        }

        //Delete a like, Unlike Broadcasts
        [Route("unlike-broadcast")]
        public async Task<IHttpActionResult> DeleteUnlike(int id)
        {
            string UserId = User.Identity.GetUserId();
            BLike like = await db.BLikes.FindAsync(id, UserId);
            if (like == null)
            {
                return NotFound();
            }

            //Ensure that the person deleting is either the owner of the broadcast or of the comment
            
            if (like.CustomerId != UserId )
            {
                return StatusCode(HttpStatusCode.Unauthorized); 
            }

            //ensure cascade delete

            db.BLikes.Remove(like);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        //Delete a comment
        [Route("delete-comment-on-broadcast")]
        public async Task<IHttpActionResult> DeleteComment(int id)
        {
            BComment comment = await db.BComments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            //Ensure that the person deleting is either the owner of the comment
            string UserId = User.Identity.GetUserId();
            if (comment.CustomerId != UserId )
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            //ensure cascade delete

            db.BComments.Remove(comment);
            db.Entry(comment).State = EntityState.Deleted;

            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        //Get More Broadcast comments
        [Route("get-comments-on-broadcast")]
        [ResponseType(typeof(IQueryable<BroadcastComment>))]
        public IHttpActionResult GetBComments(int id , int page, int amount)
        {
            var result = db.BComments.Where(b => b.BroadcastId == id)
                        .OrderBy(a => a.time).Skip(amount * (page - 1)).Take(amount)
                        .Project().To<BroadcastComment>();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //Get More Broadcasts likes
        [Route("get-broadcast-likes")]
        [ResponseType(typeof(IQueryable<BroadcastLike>))]
        public async Task<IHttpActionResult> GetBLikes(int id, int page, int amount)
        {
            var result = db.BLikes.Where(b => b.BroadcastId == id)
                        .OrderBy(a => a.time).Skip(amount * (page - 1)).Take(amount)
                        .Project().To<BroadcastLike>();

            if (result == null)
            {
                return NotFound();
            }
            
            return Ok(result);
        }

        //Put a comment on broadcasts
        [Route("comment-on-broadcast")]
        public async Task<IHttpActionResult> PutCommentB(int id , CommentDTO comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Broadcast broad = await db.Broadcasts.FindAsync(id);
            //check for advert not found
            if (broad == null || id != broad.BroadcastId)
            {
                return BadRequest();
            }
            string UserId = User.Identity.GetUserId();
            broad.BComments.Add(new BComment
            { 
                CustomerId = UserId, 
                BroadcastId = id, 
                comment = comment.comment,
                time = DateTime.UtcNow
            });
            
            CompanyNotification notification = new CompanyNotification()
            {
                BroadcastId = id,
                time = DateTime.UtcNow,
                type = "comment",
                seen = false,
                CustomerId = UserId,
                CompanyId = broad.CompanyId
            };

            db.CompanyNotifications.Add(notification);
            db.Entry(broad).State = EntityState.Modified;
            db.Entry(notification).State = EntityState.Added;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //read on what to put there
                /**if (!(id))
                {
                    return NotFound();
                }
                else*/
                {
                    throw;
                }
            }

            return Ok(comment.comment);
        }

        [Route("get-comments-on-company")]
        [ResponseType(typeof(IQueryable<CompanyComment>))]
        public IHttpActionResult GetCComments(string id, int page, int amount)
        {
            var result = db.CComments.Where(c => c.CompanyId == id).OrderBy(a => a.time)
                            .Skip(amount * (page - 1)).Take(amount)
                            .Project().To<CompanyComment>();
            //add number of pages
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //Search for broadcasts

        /**
         * Categories Logic
         */

        //Insert Category
        [Route("create-category")]
        public async Task<IHttpActionResult> PostCreateCategory(string category)
        {
            var c = await db.Categorys.FindAsync(category);
            if (c == null)
                c = new Category() { category = category };
            else
                return StatusCode(HttpStatusCode.Conflict);

            db.Entry(c).State = EntityState.Added;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            
            return StatusCode(HttpStatusCode.Created);
        }

        //Delete Category
        public async Task<IHttpActionResult> DeleteCategory(string category)
        {
            Category c = await db.Categorys.FindAsync(category);
            if (c == null)
                return NotFound();

            db.Categorys.Remove(c);
            db.Entry(c).State = EntityState.Deleted;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        //Get Categories
        public IHttpActionResult GetCategories()
        {
            var result = db.Categorys.Project().To<CategoryDTO>();
            return Ok(result);
        }
        /**
         * Company suggestions come here
         */

        //Get Shops of a certain category
        [Route("get-companies-in-category")]
        [ResponseType(typeof(IQueryable<CompanyDTO>))]
        public async Task<IHttpActionResult> GetSuggestions(string Category, int page, int amount)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Category cate = await db.Categorys.FindAsync(Category);

            if (cate == null)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }
            var result = db.Companies.Where(a => a.Categorys.Where(c => c.category.Equals(Category)).Count() != 0)
                            .OrderBy(o => o.Followers).Skip(amount * (page - 1)).Take(amount)
                            .Project().To<CompanyDTO>();   
                            

            if (result.Count() == 0)
            {
                return NotFound();
            }

            return Ok(result);
            //http since might not be found
        }

        //Get shop suggestions grouped by category
        [Route("get-company-suggestions-by-category")]
        [ResponseType(typeof(IQueryable<CategoryCompanies>))]
        public IHttpActionResult GetSuggestions(int num_categories, int page, int num_companies)
        {
            //To get more of each category individual calls are made for the specific category
            /*var result = db.Categorys.OrderBy(c => c.category).Skip(num_categories * (page - 1)).Take(num_categories)
                            .Include(c => c.companys.OrderBy(a => a.Followers.Count()).Take(num_companies))
                            
                            .Project().To<CategoryCompanies>();
            */
            ICollection<CategoryCompanies> result = new List<CategoryCompanies>();

            ICollection<string> categories = db.Categorys.OrderBy(a => a.category).Skip(num_categories * (page - 1)).Take(num_categories)
                                .Select(c => c.category).ToList();

            foreach (string cat in categories)
            {
                var companies = db.Companies.OrderBy(a => a.Followers.Count()).Take(num_companies)
                .Where(a => a.Categorys.Where(c => c.category.Equals(cat)).Count() != 0)
                    .Project().To<CompanyDTO>().ToList();

                result.Add(new CategoryCompanies()
                {
                    category = cat,
                    Companies = companies
                });
            }
            
            
            //catch when an error occurs

            return Ok(result);
        }

        [Route("get-company-suggestions")]
        [ResponseType(typeof(IQueryable<CompanyDTO>))]
        public async Task<IHttpActionResult> GetCompanySuggestions(int page, int amount)
        {
            string UserId = User.Identity.GetUserId();
            Customer customer = await db.Customers.FindAsync(UserId);

            IQueryable<CompanyDTO> result = db.Companies.Where(c => c.Followers.Where(a => a.CustomerId == UserId).Count() == 0)
                                                .OrderBy(o => o.name).Skip(amount * (page - 1)).Take(amount)
                                                .Project().To<CompanyDTO>();


            //catch when an error occurs

            return Ok(result);
        }

        /**
         * Adverts operations
         */
        [Route("get-featured-adverts")]
        [ResponseType(typeof(ICollection<Advertisement>))]
        public IHttpActionResult GetAdverts(int page, int amount , double longitude , double latitude)
        {
            var location = DbGeography.FromText(string.Format("POINT({0} {1})", longitude , latitude));
            
            try
            {
                var result = db.Adverts.OrderBy(c => c.Location.Distance(location))
                            .Skip(amount * (page - 1))
                            .Take(amount)
                            .Project().To<Advertisement>().ToList<Advertisement>();

               

                if (result == null)
                {
                    return NotFound();
                }

                string UserId = User.Identity.GetUserId();

                foreach (var p in result)
                {
                    p.liked = db.ALikes.Find(p.ID , UserId) != null ? true : false;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {

               return Ok(ex.InnerException);
            }
            
            
        }

        //Post an ad, create
        [Route("create-advert")]
        public async Task<IHttpActionResult> PostAdvert(CreateAd advert, double longitude, double latitude)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string UserId = User.Identity.GetUserId();
            //Advert ad = Mapper.Map<Advert>(advert);
            Advert ad = new Advert()
            {
                title = advert.title,
                info = advert.details,
                price = advert.price,
                time = DateTime.UtcNow,
                exp = DateTime.UtcNow.AddDays(20)
            };
            ad.CustomerId = UserId;
            ad.Location = DbGeography.FromText(string.Format("POINT({0} {1})", longitude , latitude));
            Customer customer = await db.Customers.FindAsync(UserId);
            if (customer == null)
                return NotFound();

            if (advert.categories != null)
            {
                foreach (string c in advert.categories)
                {
                    var obj = db.Categorys.Find(c);
                    if (obj == null)
                        obj = new Category() { category = c };


                    ad.Categories.Add(obj);
                }
            }
            ad.Customer = customer;
            customer.active_ads += 1;
            db.Adverts.Add(ad);
            db.Entry(ad).State = EntityState.Added;
            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (db.Adverts.Any(a => a.Id == ad.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(ad.Id);
        }

        //Ads Search FTS

        //Get Ads by category
        [Route("get-adverts-in-category")]
        [ResponseType(typeof(IQueryable<Advertisement>))]
        public async Task<IHttpActionResult> GetCateAds(string category, int page, int amount, double longitude, double latitude)
        {
            /*
            var result = db.Categorys.Where(c => c.category == category)
                        .Select(a => a.Adverts.OrderBy( o => o.time).Skip(amount*(page - 1)).Take(amount))
                        .Project().To<Advertisement>();
            */
            var location = DbGeography.FromText(string.Format("POINT({0} {1})", longitude, latitude));
            var result = await db.Adverts.Where(c => c.Categories.Where(a => a.category.Equals(category)).Count() != 0)
                            .OrderBy(c => c.Location.Distance(location)).Skip(amount * (page - 1)).Take(amount)
                            .Project().To<Advertisement>().ToListAsync<Advertisement>();

            if (result == null)
            {
                return NotFound();
            }

            String UserId = User.Identity.GetUserId();
            foreach (var p in result)
            {
                p.liked = db.ALikes.Find(p.ID, UserId) != null ? true : false;
            }

            return Ok(result);
            
        }

        [Route("get-personal-adverts")]
        public async Task<IHttpActionResult> GetMyAds(int page, int amount)
        {
            String UserId = User.Identity.GetUserId();
            
            /*
            var result = db.Categorys.Where(c => c.category == category)
                        .Select(a => a.Adverts.OrderBy( o => o.time).Skip(amount*(page - 1)).Take(amount))
                        .Project().To<Advertisement>();
            */
            var result = await db.Adverts.Where(c => c.CustomerId == UserId)
                            .OrderBy(o => o.time).Skip(amount * (page - 1)).Take(amount)
                            .Project().To<Advertisement>().ToListAsync<Advertisement>();

            if (result == null)
            {
                return NotFound();
            }

            
            foreach (var p in result)
            {
                p.liked = db.ALikes.Find(p.ID, UserId) != null ? true : false;
            }

            return Ok(result);

        }
        //Edit Ads, Put

        //Like Ad
        [Route("like-advert")]
        public async Task<IHttpActionResult> PutLikeA(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Advert advert = await db.Adverts.FindAsync(id);
            string UserId = User.Identity.GetUserId();
            //check for advert not found
            if (advert == null || id != advert.Id)
            {
                return BadRequest();
            }
             
            ALike duplicate = await db.ALikes.FindAsync(id, UserId);

            if (duplicate != null)
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

            advert.ALikes.Add(new ALike
            {
                CustomerId = UserId,
                AdvertId = id,
                time = DateTime.UtcNow
            });

            CustomerNotification notification = new CustomerNotification()
            {
                AdvertId = id,
                time = DateTime.UtcNow,
                type = "like",
                seen = false,
                UserId = UserId,
                CustomerId = advert.CustomerId
            };

            db.CustomerNotifications.Add(notification);
            db.Entry(notification).State = EntityState.Added;
            db.Entry(advert).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //read on what to put there
                /**if (!(id))
                {
                    return NotFound();
                }
                else*/
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.Created);
        }

        //Put a comment on Adverts
        [Route("comment-on-advert")]
        public async Task<IHttpActionResult> PutCommentA(int id, CommentDTO comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Advert advert = await db.Adverts.FindAsync(id);
            //check for advert not found
            if (advert == null || id != advert.Id)
            {
                return BadRequest();
            }
            string UserId = User.Identity.GetUserId();
            advert.AComments.Add(new AComment
            {
                CustomerId = UserId,
                AdvertId = id,
                comment = comment.comment,
                time = DateTime.UtcNow
            });

            CustomerNotification notification = new CustomerNotification()
            {
                AdvertId = id,
                type = "comment",
                seen = false,
                UserId = UserId,
                time = DateTime.Now,
                CustomerId = advert.CustomerId
            };

            db.CustomerNotifications.Add(notification);
            db.Entry(advert).State = EntityState.Modified;
            db.Entry(notification).State = EntityState.Added;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //read on what to put there
                /**if (!(id))
                {
                    return NotFound();
                }
                else*/
                {
                    throw;
                }
            }

            return Ok(comment.comment);
        }

        //Delete a like, Unlike Adverts
        [Route("unlike-advert")]
        public async Task<IHttpActionResult> DeleteUnlikeA(int id)
        {
            string UserId = User.Identity.GetUserId();
            ALike like = await db.ALikes.FindAsync(id, UserId);
            if (like == null)
            {
                return NotFound();
            }

            //Ensure that the person deleting is either the owner of the broadcast or of the comment
            
            if (like.CustomerId != UserId)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            //ensure cascade delete

            db.ALikes.Remove(like);
            db.Entry(like).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        //Delete a comment
        [Route("delete-comment-on-advert")]
        public async Task<IHttpActionResult> DeleteCommentA(int id)
        {
            AComment comment = await db.AComments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            //Ensure that the person deleting is either the owner of the comment
            string UserId = User.Identity.GetUserId();
            if (comment.CustomerId != UserId)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            //ensure cascade delete

            db.AComments.Remove(comment);
            db.Entry(comment).State = EntityState.Deleted;

            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        //Get More Broadcast comments
        [Route("get-advert-comments")]
        [ResponseType(typeof(IQueryable<AdComment>))]
        public IHttpActionResult GetAComments(int id, int page, int amount)
        {
            /*
            var result = db.Adverts.Where(b => b.Id == adId)
                        .Select(c => c.AComments.OrderBy(a => a.time).Skip(amount * (page - 1)).Take(amount))
                            .Project().To<AdComment>();
             */
            var result = db.AComments.Where(c => c.AdvertId == id)
                            .OrderBy(a => a.time).Skip(amount * (page - 1)).Take(amount)
                            .Project().To<AdComment>();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //Get More Adverts likes
        [Route("get-advert-likes")]
        [ResponseType(typeof(IQueryable<AdLike>))]
        public IHttpActionResult GetALikes(int id, int page, int amount)
        {
            /*
            var result = db.Adverts.Where(b => b.Id == adId)
                        .Select(c => c.ALikes.OrderBy(a => a.time).Skip(amount * (page - 1)).Take(amount))
                            .Project().To<AdLike>();
             */
            var result = db.ALikes.Where(c => c.AdvertId == id)
                            .OrderBy(a => a.time).Skip(amount * (page - 1)).Take(amount)
                            .Project().To<AdLike>();
            
            if (result == null)
            {
                return NotFound();
            }
            
            return Ok(result);
        }

        /**
         * Buddies operations
         */

        //Get friends
        [Route("get-buddies")]
        [ResponseType(typeof(IQueryable<CustomerDTO>))]
        public IHttpActionResult GetBuddies(int amount, int page)
        {
            string UserId = User.Identity.GetUserId();
            var result = db.Buddies.Where(c => c.Id == UserId)
                        .Select(a => a.Customers.OrderBy(o => o.fname).Skip(amount * (page - 1)).Take(amount))
                            .Project().To<CustomerDTO>();

            if (result == null)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            //catch when an error occurs

            return Ok(result);
        }

        //Get friend suggestion

        //Friend request, Get
        [Route("get-friend-requests")]
        [ResponseType(typeof(IQueryable<FriendRequest>))]
        public IHttpActionResult GetFriendRequest()
        {
            string UserId = User.Identity.GetUserId();
            var result = db.Requests.Where(c => c.receiverId == UserId && c.accepted == false)
                            .Project().To<FriendRequest>();
            
            if (result == null)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Ok(result);
        }

        //Accepted Request
        [Route("get-accepted-requests")]
        [ResponseType(typeof(IQueryable<AcceptedRequest>))]
        public IHttpActionResult GetAcceptedRequest()
        {
            string UserId = User.Identity.GetUserId();
            var result = db.Requests.Where(c => c.senderId == UserId && c.accepted == true)
                            .Project().To<AcceptedRequest>();
            
            if (result == null)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Ok(result);
        }

        //Send friend request
        [Route("send-friend-request")]
        public async Task<IHttpActionResult> PutFriendRequest(string id)
        {
            Customer customer = await db.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }
            string UserId = User.Identity.GetUserId();
            Request request = new Request()
            {
                senderId = UserId,
                receiverId = id,
                seen = false,
                accepted = false
            };

            db.Requests.Add(request);
            db.Entry(request).State = EntityState.Added;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.Created);

        }
        
        //Accept friend request, Put
        [Route("accept-friend-request")]
        public async Task<IHttpActionResult> PutAcceptRequest(int id)
        {
            Request request = await db.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            request.accepted = true;
            db.Entry(request).State = EntityState.Modified;
            string UserId = User.Identity.GetUserId();
            Buddy buddy = await db.Buddies.FindAsync(UserId);
            Customer customer = await db.Customers.FindAsync(UserId);

            customer.num_friends += 1;
            buddy.Customers.Add(request.Sender);

            db.Entry(customer).State = EntityState.Modified;
            db.Entry(buddy).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        //Seen Friend Request
        [Route("friend-request-seen")]
        public async Task<IHttpActionResult> PutRequestSeen(int id)
        {
            Request request = await db.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            request.seen = true;
            db.Entry(request).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return StatusCode(HttpStatusCode.NoContent);

        }

        //Unfriend, Delete
        [Route("unfriend")]
        public async Task<IHttpActionResult> DeleteFriend(string id)
        {
            string UserId = User.Identity.GetUserId();
            Buddy buddy = await db.Buddies.FindAsync(UserId);
            Customer customer = await db.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            buddy.Customers.Remove(customer);
            db.Entry(customer).State = EntityState.Detached;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return StatusCode(HttpStatusCode.NoContent);
        }
        
        //Get followed companies
        [Route("get-following-companies")]
        [ResponseType(typeof(IQueryable<CompanyDTO>))]
        public IHttpActionResult GetFollowed(int page, int amount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            string UserId = User.Identity.GetUserId();
            /*
            IQueryable<CompanyDTO> result =  db.Customers.Where(a => a.CustomerId == UserId)
                                    .Select(c => c.Followings.OrderBy(o => o.Followers.Count())
                                    .Skip(amount * (page - 1)).Take(amount))
                                    .Project().To<CompanyDTO>();
            */

            IQueryable<CompanyDTO> result = db.Companies.Where(a => a.Followers.Where(c => c.CustomerId == UserId).Count() != 0)
                                        .OrderBy(o => o.Followers.Count()).Skip(amount * (page - 1)).Take(amount)
                                        .Project().To<CompanyDTO>();

            if (result == null)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Ok(result);
        }

        [Route("follow-company")]
        public async Task<IHttpActionResult> PutFollow(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            string UserId = User.Identity.GetUserId();

            Company company = await db.Companies.FindAsync(id);
            Customer customer = await db.Customers.FindAsync(UserId);
            var duplicate = db.Companies.Where(a => a.CompanyId == id)
                            .Select(c => c.Followers.Any(a => a.CustomerId == UserId)).FirstOrDefault();
         
    
            if(duplicate)
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

            await db.Broadcasts.Where(a => a.CompanyId == id).Take(10).ForEachAsync(c => c.Customers.Add(customer));
            company.Followers.Add(customer);
            db.Entry(company).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError); 
            }
            
            return StatusCode(HttpStatusCode.Created);
        }
        

        //Get shop's private page
        [Route("get-company-profile")]
        [ResponseType(typeof(CompanyDetails))]
        public IHttpActionResult GetCompanyDetails(string id)
        {
            IQueryable<CompanyDetails> result =  db.Companies.Where(c => c.CompanyId == id)
                                            .Project().To<CompanyDetails>();

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //Share broadcast, Put
        [Route("share-broadcast")]
        public async Task<IHttpActionResult> PutShare(List<string> userIds, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Broadcast broad = await db.Broadcasts.FindAsync(id);
            
            if (broad == null)
            {
                return NotFound();
            }
            //broad.Customers.Concat(userIds);
            

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //read on what to put there
                /**if (!(id))
                {
                    return NotFound();
                }
                else*/
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        
        /**
         * Profile operations
         */
        //Edit/Update/Create profile, Put
        [Route("edit-profile")]
        public async Task<IHttpActionResult> PutProfile(CustomerData cust)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string UserId = User.Identity.GetUserId();
            Customer customer = await db.Customers.FindAsync(UserId);

            //check for advert not found
            if (customer == null)
            {
                return NotFound();
            }

            Mapper.Map(cust, customer);
            
            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //read on what to put there
                /**if (!(id))
                {
                    return NotFound();
                }
                else*/
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        //Change Address, Put
        [Route("change-address")]
        public async Task<IHttpActionResult> PutAddress(Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string UserId = User.Identity.GetUserId();
            CustomerAddress add = await db.CustomerAddresses.FindAsync(UserId);
            
            //check for advert not found
            if (add == null)
            {
                return NotFound();
            }
            Mapper.Map(address, add);
            db.Entry(add).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //read on what to put there
                /**if (!(id))
                {
                    return NotFound();
                }
                else*/
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        //Change profile picture

        //Delete picture

        //Get user profile
        [ResponseType(typeof(CustomerDetails))]
        [Route("get-user-profile")]
        public IHttpActionResult GetUserProfile()
        {
            string UserId = User.Identity.GetUserId();
            var customer = db.Customers.Where(c => c.CustomerId == UserId)
                               .Project().To<CustomerDetails>();
            return Ok(customer);
        }
        
        /**
         * Notifications
         */
        //Notifications
        [Route("get-notifications")]
        [ResponseType(typeof(IQueryable<Notification>))]
        public IHttpActionResult GetNotifications(int amount, int page)
        {
            string UserId = User.Identity.GetUserId();
            var result = db.CustomerNotifications.Where(c => c.CustomerId == UserId)
                            .Skip(amount * (page - 1)).Take(amount)
                            .Project().To<Notification>();

            if (result == null)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return Ok(result);
        }

        //Notifications seen, Put
        [Route("notification-seen")]
        public async Task<IHttpActionResult> PutSeen(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CustomerNotification notification = await db.CustomerNotifications.FindAsync(id);
            
            //Ensure that the person modifying is either the owner of the comment
            string UserId = User.Identity.GetUserId();
            if (notification.CustomerId != UserId )
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            //check for not found
            if (notification == null)
            {
                return BadRequest();
            }

            notification.seen = true;
            db.Entry(notification).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //read on what to put there
                /**if (!(id))
                {
                    return NotFound();
                }
                else*/
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        //get the notified object
        [Route("go-to-notification")]
        [ResponseType(typeof(Advertisement))]
        public IHttpActionResult GetNotifications(int id)
        {
            var result = db.CustomerNotifications.Where(c => c.Id == id)
                            .Select(a => a.Advert).Project().To<Advertisement>();

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        
        
        private bool CustomerExists(string id)
        {
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }

        private bool AdvertExists(int id)
        {
            return db.Adverts.Count(e => e.Id == id) > 0;
        }
    }
}