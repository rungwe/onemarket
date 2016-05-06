using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Web.Http.Description;
using System.Threading.Tasks;

namespace WorldWebMall.Controllers
{
    public class TestUpController : ApiController
    {
        [HttpPost]
        [Route("uploadImage")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> UploadFile()
        {
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection
                var httpPostedFile =  HttpContext.Current.Request.Files["UploadedImage"];
                Stream img = HttpContext.Current.Request.Files["UploadedImage"].InputStream;
                
                if (httpPostedFile != null)
                {
                    bool status = UploadFileToS3("testing.png", img, "oitems");
                    if (status)
                    {
                        return StatusCode(HttpStatusCode.OK);
                    }
                    else
                    {
                        return StatusCode(HttpStatusCode.NotAcceptable);
                    }
                    
                    
                }
                else
                {
                    return StatusCode(HttpStatusCode.ExpectationFailed);
                }
            }
            else
            {
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }
        }
       
        public bool UploadFileToS3(string uploadAsFileName, Stream ImageStream, string toWhichBucketName)
        {

            try
            {
               

                //put your aws credential
                var client = Amazon.AWSClientFactory.CreateAmazonS3Client("AKIAJMKP7P3R6S7U5LIQ", "LJUg7L8k86x5vszvEH5CNMeWOVOT3iSXMqS4OM6A", Amazon.RegionEndpoint.USWest2);
                PutObjectRequest request = new PutObjectRequest();
                request.Key=uploadAsFileName;
                request.InputStream=ImageStream;
                request.BucketName= toWhichBucketName;
                request.CannedACL = S3CannedACL.PublicRead;
                request.StorageClass = S3StorageClass.Standard;
                client.PutObject(request);
                client.Dispose();
            }
            catch (Exception e)
            {

                return false;
            }
            return true;

        }
    }
}
