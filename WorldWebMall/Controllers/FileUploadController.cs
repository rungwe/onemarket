using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Threading.Tasks;
using WorldWebMall.Models;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.AspNet.Identity;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Web.Http.Cors;
using WorldWebMall.Providers;

namespace WorldWebMall.Controllers
{

    //[Authorize]
    //[EnableCors(origins: "http://ec2-52-88-34-32.us-west-2.compute.amazonaws.com", headers: "*", methods: "*")]
    public class FileUploadController : ApiController
    {
        static readonly string ServerUploadFolder = "C:/Users/tariro/Documents/Uploads";
        private MallContext db = new MallContext();

        private static IAmazonS3 s3Client = Amazon.AWSClientFactory.CreateAmazonS3Client("AKIAJMKP7P3R6S7U5LIQ", "LJUg7L8k86x5vszvEH5CNMeWOVOT3iSXMqS4OM6A", Amazon.RegionEndpoint.USWest2);
        //private static IAmazonS3 s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USWest2);    

        [HttpPost]
        //upload advert pictures
        [Route("upload-advert-pictures")]
        public async Task<IHttpActionResult> UploadAdvertPictures(int id)
        {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Advert ad = await db.Adverts.FindAsync(id);

                if (ad == null)
                {
                    return NotFound();
                }

                if (ad.CustomerId != User.Identity.GetUserId())
                {
                    return Unauthorized();
                }

                if (!Request.Content.IsMimeMultipartContent()) 
                {   
                    this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType); 
                } 
                
                InMemoryMultipartStreamProvider provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartStreamProvider>(new InMemoryMultipartStreamProvider()); 
                IList<HttpContent> files = provider.Files; 
                HttpContent file = files[0]; 
                
                Stream fileStream = await file.ReadAsStreamAsync();
               
                // Create a stream provider for setting up output streams
                string bucketname = "debug123456789oitems";
                string filename = GenerateName(bucketname);
                try
                {
                    PutObjectRequest s3Request = new PutObjectRequest();   
                    s3Request.BucketName = bucketname;
                    s3Request.InputStream = fileStream;
                    s3Request.Key = filename; 
                    s3Request.ContentType = provider.GetImageType();
                    s3Request.CannedACL = S3CannedACL.PublicRead;
                    s3Request.StorageClass = S3StorageClass.Standard;
                    s3Client.PutObject(s3Request);
                }
                catch (Exception e)
                {
                    return Ok(e);
                }
                
                ad.pictures.Add(new Picture() { path = bucketname + "/" + filename });
                db.Entry(ad).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    throw;
                }
                return StatusCode(HttpStatusCode.Created);
        }

        private string GenerateName(string bucketname)
        {
            string filename = Guid.NewGuid().ToString();
            Amazon.S3.IO.S3FileInfo s3FileInfo = new Amazon.S3.IO.S3FileInfo(s3Client, bucketname , filename);
            if (s3FileInfo.Exists)
            {
                return GenerateName(bucketname);
            }
            else
            {
                return filename;
            }
  
        }

        //upload broadcasts pictures
        [Route("upload-broadcast-pictures")]
        public async Task<IHttpActionResult> UploadBroadcastPictures(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Broadcast bd = await db.Broadcasts.FindAsync(id);

            if (bd == null)
            {
                return NotFound();
            }

            if (bd.CompanyId != User.Identity.GetUserId())
            {
                return Unauthorized();
            }

            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            InMemoryMultipartStreamProvider provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartStreamProvider>(new InMemoryMultipartStreamProvider());
            IList<HttpContent> files = provider.Files;
            HttpContent file = files[0];

            Stream fileStream = await file.ReadAsStreamAsync();

            // Create a stream provider for setting up output streams
            string bucketname = "debug123456789oitems";
            string filename = GenerateName(bucketname);
            try
            {
                PutObjectRequest s3Request = new PutObjectRequest();
                s3Request.BucketName = bucketname;
                s3Request.InputStream = fileStream;
                s3Request.Key = filename;
                s3Request.ContentType = provider.GetImageType();
                s3Request.CannedACL = S3CannedACL.PublicRead;
                s3Request.StorageClass = S3StorageClass.Standard;
                s3Client.PutObject(s3Request);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

            bd.pictures.Add(new Picture() { path = bucketname + "/" + filename });
            db.Entry(bd).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }
            
            return StatusCode(HttpStatusCode.Created);

        }

        //upload customer profile picture
        [Route("upload-customer-profile-picture")]
        public async Task<IHttpActionResult> UploadCustomerPP()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string UserId = User.Identity.GetUserId();
            Customer customer = await db.Customers.FindAsync(UserId);
            //remember to put this wherever user identity is tested
            if (customer == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            InMemoryMultipartStreamProvider provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartStreamProvider>(new InMemoryMultipartStreamProvider());
            IList<HttpContent> files = provider.Files;
            HttpContent file = files[0];

            Stream fileStream = await file.ReadAsStreamAsync();

            // Create a stream provider for setting up output streams
            string bucketname = "debug123456789oitems";
            string filename = GenerateName(bucketname);
            try
            {
                PutObjectRequest s3Request = new PutObjectRequest();
                s3Request.BucketName = bucketname;
                s3Request.InputStream = fileStream;
                s3Request.Key = filename;
                s3Request.ContentType = provider.GetImageType();
                s3Request.CannedACL = S3CannedACL.PublicRead;
                s3Request.StorageClass = S3StorageClass.Standard;
                s3Client.PutObject(s3Request);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

            Picture pic = db.Customers.Where(c => c.CustomerId == UserId).Select(a => a.p_pic)
                                .FirstOrDefault();

            if (pic != null)
            {
                pic.path = bucketname + "/" + filename;
                db.Entry(pic).State = EntityState.Modified;
            }
            else
            {
                customer.p_pic = new Picture() { path = bucketname + "/" + filename };
                db.Entry(customer).State = EntityState.Modified;
            }


            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }
            return StatusCode(HttpStatusCode.Created);

        }

        //Upload company profile picture
        [Route("upload-company-profile-picture")]
        public async Task<IHttpActionResult> UploadCompanyPP()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string UserId = User.Identity.GetUserId();
            Company company = await db.Companies.FindAsync(UserId);
            //remember to put this wherever user identity is tested
            if (company == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            InMemoryMultipartStreamProvider provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartStreamProvider>(new InMemoryMultipartStreamProvider());
            IList<HttpContent> files = provider.Files;
            HttpContent file = files[0];

            Stream fileStream = await file.ReadAsStreamAsync();

            // Create a stream provider for setting up output streams
            string bucketname = "debug123456789oitems";
            string filename = GenerateName(bucketname);
            try
            {
                PutObjectRequest s3Request = new PutObjectRequest();
                s3Request.BucketName = bucketname;
                s3Request.InputStream = fileStream;
                s3Request.Key = filename;
                s3Request.ContentType = provider.GetImageType();
                s3Request.CannedACL = S3CannedACL.PublicRead;
                s3Request.StorageClass = S3StorageClass.Standard;
                s3Client.PutObject(s3Request);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

            Picture pic = db.Companies.Where(c => c.CompanyId == UserId).Select(a => a.p_pic)
                                .FirstOrDefault();

            if (pic != null)
            {
                pic.path = bucketname + "/" + filename;
                db.Entry(pic).State = EntityState.Modified;   
            }
            else
            {
                company.p_pic = new Picture() { path = bucketname + "/" + filename };
                db.Entry(company).State = EntityState.Modified;
            }

            
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }
            return StatusCode(HttpStatusCode.Created);

        }

        //Add company wallpaper
        [Route("upload-company-wallpaper")]
        public async Task<IHttpActionResult> UploadWallpaper()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string UserId = User.Identity.GetUserId();
            Company company = await db.Companies.FindAsync(UserId);
            //remember to put this wherever user identity is tested
            if (company == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            InMemoryMultipartStreamProvider provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartStreamProvider>(new InMemoryMultipartStreamProvider());
            IList<HttpContent> files = provider.Files;
            HttpContent file = files[0];

            Stream fileStream = await file.ReadAsStreamAsync();

            // Create a stream provider for setting up output streams
            string bucketname = "debug123456789oitems";
            string filename = GenerateName(bucketname);
            try
            {
                PutObjectRequest s3Request = new PutObjectRequest();
                s3Request.BucketName = bucketname;
                s3Request.InputStream = fileStream;
                s3Request.Key = filename;
                s3Request.ContentType = provider.GetImageType();
                s3Request.CannedACL = S3CannedACL.PublicRead;
                s3Request.StorageClass = S3StorageClass.Standard;
                s3Client.PutObject(s3Request);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

            Picture pic = db.Companies.Where(c => c.CompanyId == UserId).Select(a => a.wallpaper)
                                .FirstOrDefault();

            if (pic != null)
            {
                pic.path = bucketname + "/" + filename;
                db.Entry(pic).State = EntityState.Modified;
            }
            else
            {
                company.wallpaper = new Picture() { path = bucketname + "/" + filename };
                db.Entry(company).State = EntityState.Modified;
            }


            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }
            return StatusCode(HttpStatusCode.Created);

        }

        [Route("get-image")]
        public async Task<HttpResponseMessage> Get(string id)
        {
                     
            HttpResponseMessage result = null;

            if (!ModelState.IsValid)
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            Picture picture = await db.Pictures.FindAsync(id);

            if (picture == null)
            {
                result = Request.CreateResponse(HttpStatusCode.NotFound);
            }

            // check if file exists on the server
            else if (!File.Exists(picture.path))
            {
                result = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {// serve the file to the client
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(picture.path, FileMode.Open, FileAccess.Read));
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(Path.GetExtension(picture.path));
                result.Content.Headers.ContentDisposition.FileName = id;
            }
            
            
            return result;
        }
    }
}
