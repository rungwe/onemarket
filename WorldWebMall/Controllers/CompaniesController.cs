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
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.AspNet.Identity;
using WorldWebMall.Models;
using System.Web.Http.Cors;
using WorldWebMall.SocialNetworks;

namespace WorldWebMall.Controllers
{
    [Authorize(Roles="company")]
    [RoutePrefix("company")]
    //[EnableCors(origins: "http://ec2-52-88-34-32.us-west-2.compute.amazonaws.com", headers: "*", methods: "*")]
    public class CompaniesController : ApiController
    {
        private MallContext db = new MallContext();

        // GET: api/Companies
        public IQueryable<Company> GetCompanies()
        {
            return db.Companies;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> Status Code 200 if OK</returns>
        [ResponseType(typeof(CompanyDetails))]
        [Route("get-user-profile")]
        public IHttpActionResult GetUserProfile()
        {
            string UserId = User.Identity.GetUserId();
            
            var company = db.Companies.Where(c => c.CompanyId == UserId)
                                .Project().To<CompanyDetails>();

            return Ok(company);
        }


        // GET: broadcast
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="page"></param>
        /// <param name="time"></param>
        /// <returns>Status Code:   204 if no broadcasts 
        ///                         200 and the broadcasts</returns>
        [ResponseType(typeof(BroadcastDTO))]
        [Route("get-broadcasts")]
        public IHttpActionResult GetBroadcasts(int amount , int page , DateTime? time = null)
        {
            string UserId = User.Identity.GetUserId();
               
            var result = db.Broadcasts.Where(c => c.CompanyId == UserId).OrderBy(a => a.time)
                        .Skip(amount * (page - 1)).Take(amount).Project().To<BroadcastDTO>().ToList<BroadcastDTO>();
            if (result == null)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            foreach (var p in result)
            {
                p.liked = db.BLikes.Find(p.ID, UserId) != null ? true : false;
            }

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <returns>Status Code:   201 if successful</returns>
        [ResponseType(typeof(ContactDTO))]
        [Route("create-contacts")]
        public async Task<IHttpActionResult> PostContacts(ICollection<ContactDTO> contacts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //might be a way of doing the mapping at once
            string UserId = User.Identity.GetUserId();
            ICollection<ContactNumber> contact = Mapper.Map<ICollection<ContactNumber>>(contacts);
            Company company = await db.Companies.FindAsync(UserId);
            //company.Contacts.Union(contact); //might be able to use this but not working as it is
            foreach (var c in contact)
            {
                company.Contacts.Add(c);
            }
            
            db.Entry(company).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }

            return Ok(company.Contacts);
        }

        [Route("get-contacts")]
        public IHttpActionResult GetContacts()
        {
            //the company variable under contacts has a null on it
            string UserId = User.Identity.GetUserId();
            var contacts = db.ContactNumbers.Where(c => c.CompanyId == UserId)
                                .Project().To<ContactDTO>();

            return Ok(contacts);
        }
        //create broadcast
        [Route("create-broadcast")]
        public async Task<IHttpActionResult> PostBroadcast(CreateB broadcast)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string UserId = User.Identity.GetUserId();
            //broadcast.companyId = UserId;
            Broadcast bd = Mapper.Map<Broadcast>(broadcast);
            bd.CompanyId = UserId;

            if (broadcast.categories != null)
            {
                foreach (string c in broadcast.categories)
                {
                    var obj = db.Categorys.Find(c);
                    if (obj == null)
                        obj = new Category() { category = c };


                    bd.categorys.Add(obj);
                }
            }
            
            ICollection<Customer> cust = db.Companies.Where(a => a.CompanyId == UserId).Select(c => c.Followers)
                                        .FirstOrDefault().ToList();
            bd.Customers = cust;
            db.Broadcasts.Add(bd);
           
            foreach (Customer c in cust)
            {
                await db.Customers.Where(a => a.CustomerId == c.CustomerId).ForEachAsync(o => o.Broadcasts.Add(bd));
            }
            
            db.Entry(bd).State = EntityState.Added;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (db.Broadcasts.Any(a => a.BroadcastId == bd.BroadcastId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(bd.BroadcastId);
        }

        
        
        //Like Broadcasts
        [Route("like-broadcast")]
        public async Task<IHttpActionResult> PutLikeB(int id)
        {
            try { 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Broadcast broad = await db.Broadcasts.FindAsync(id);
            string Id = User.Identity.GetUserId();
            //check for advert not found
            if (broad == null || id != broad.BroadcastId)
            {
                return BadRequest();
            }

            BLike duplicate = await db.BLikes.FindAsync(id, Id);
            if (duplicate != null)
            {
                return StatusCode(HttpStatusCode.Conflict);
            }
            broad.BLikes.Add(new BLike
            {
                UserId = Id,
                CompanyId = Id,
                BroadcastId = id,
                time = DateTime.UtcNow
            });
            /**
                CompanyNotification notification = new CompanyNotification()
                {
                    BroadcastId = id,
                    type = "like",
                    seen = false,
                    CustomerId = Id,
                    CompanyId = broad.CompanyId
                };
            */
            //db.CompanyNotifications.Add(notification);
            //db.Entry(notification).State = EntityState.Added;
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

        //Comment On Broadcast
        [Route("comment-on-broadcast")]
        public async Task<IHttpActionResult> PutCommentB(int id, CommentDTO comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Broadcast broad = await db.Broadcasts.FindAsync(id);
            //check for advert not found
            if (broad == null)
            {
                return NotFound();
            }
            
            if (id != broad.BroadcastId)
            {
                return BadRequest();
            }
            string UserId = User.Identity.GetUserId();
            broad.BComments.Add(new BComment
            {
                CompanyId = UserId,
                BroadcastId = id,
                comment = comment.comment,
                time = DateTime.UtcNow
            });
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

            return Ok(comment.comment);
        }

        //unlike broadcast
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

            if (like.CompanyId != UserId)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            //ensure cascade delete

            db.BLikes.Remove(like);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

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
            if (comment.CompanyId != UserId)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            //ensure cascade delete

            db.BComments.Remove(comment);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        //Get More Broadcast comments
        [Route("broadcast-get-comments")]
        [ResponseType(typeof(IQueryable<BroadcastComment>))]
        public IHttpActionResult GetBComments(int id, int page, int amount)
        {
            var result = db.Broadcasts.Where(b => b.BroadcastId == id)
                .Select(c => c.BComments.OrderBy(a => a.time).Skip(amount * (page - 1)).Take(amount))
                            .Project().To<BroadcastComment>();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //Get More Broadcasts likes
        [Route("broadcast-get-likes")]
        [ResponseType(typeof(IQueryable<BroadcastLike>))]
        public IHttpActionResult GetBLikes(int id , int page, int amount)
        {
            var result = db.Broadcasts.Where(b => b.BroadcastId == id)
                .Select(c => c.BLikes.OrderBy(a => a.time).Skip(amount * (page - 1)).Take(amount))
                            .Project().To<BroadcastLike>();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //Edit Profile
        [Route("edit-profile")]
        public async Task<IHttpActionResult> PutCompany(CompanyData companyData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string UserId = User.Identity.GetUserId();
            Company company = await db.Companies.FindAsync(UserId);
            //ICollection<Category> category = Mapper.Map<ICollection<Category>>(companyData.categories);
            Mapper.Map(companyData, company);
            HashSet<string> set = new HashSet<string>();

            foreach (var c in companyData.contacts)
            {
                ContactNumber contact = db.ContactNumbers.Where(a => a.CompanyId == UserId & a.type == c.type )
                                            
                                            .FirstOrDefault();
                 
                if (contact != null)
                {
                    contact.emails = c.emails;
                    contact.numbers = c.numbers;
                    db.Entry(contact).State = EntityState.Modified;
                    continue;
                }
                ContactNumber newContact = Mapper.Map<ContactNumber>(c);
                newContact.CompanyId = UserId;
                company.Contacts.Add(newContact);
              
            }
           
            foreach (string c in companyData.categories)
            {
                var obj = db.Categorys.Find(c);
                if(obj == null)             
                    obj = new Category() { category = c };

                int test = db.Companies.Where(a => a.CompanyId == UserId).Select(b => b.Categorys.Where(d => d.category.Equals(c))).FirstOrDefault().Count() ;
                if (test == 0)
                {
                    obj.companys.Add(company);
                    company.Categorys.Add(obj);
                }
                     
            }
            db.Entry(company).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                /**if (!CompanyExists(id))
                {
                    return NotFound();
                }
                else*/
                {
                    throw;
                }
            }

            //return StatusCode(HttpStatusCode.Created);
            return Ok();
        }

        // POST: Add address
        [Route("add-address")]
        public async Task<IHttpActionResult> PostCompanyAddress(Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string UserId = User.Identity.GetUserId();
            Company company = await db.Companies.FindAsync(UserId);
            CompanyAddress add = Mapper.Map<CompanyAddress>(address);

            company.CompanyAddresses.Add(add);
            db.Entry(company).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                /**if (CompanyExists(company.CompanyId))
                {
                    return Conflict();
                }
                else*/
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.Created);
        }

        // DELETE: address
        [Route("delete-address")]
        public async Task<IHttpActionResult> DeleteAddress(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            CompanyAddress address = await db.CompanyAddresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            db.CompanyAddresses.Remove(address);
            db.Entry(address).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
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
            var result = db.CompanyNotifications.Where(c => c.CompanyId == UserId)
                            .OrderBy(a => a.time).Skip(amount * (page - 1)).Take(amount)
                            .Project().To<Notification>();
            
            if (result.Count() == 0)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return Ok(result);
        }

        [Route("notification-seen")]
        public async Task<IHttpActionResult> PutSeen(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CompanyNotification notification = await db.CompanyNotifications.FindAsync(id);

            //Ensure that the person modifying is either the owner of the comment
            string UserId = User.Identity.GetUserId();
            if (notification.CompanyId != UserId)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            //check for not found
            if (notification == null)
            {
                return NotFound();
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

        //Go to notification
        [Route("go-to-notification")]
        [ResponseType(typeof(IQueryable<BroadcastDTO>))]
        public IHttpActionResult GetNotification(int id)
        {
            var result = db.CompanyNotifications.Where(c => c.Id == id).Select(a => a.Broadcast)
                            .Project().To<BroadcastDTO>();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompanyExists(string id)
        {
            return db.Companies.Count(e => e.CompanyId == id) > 0;
        }
    }
}