using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Net.Http.Headers;
using WorldWebMall.Models;
using System.Web.Http;
using System.Web.Http.Description;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
//using System.Threading.Tasks;

namespace WorldWebMall.SocialNetworks
{
    public class TwitterClient
    {
        HttpClient client = new HttpClient();
        private const string baseUrl = "https://api.twitter.com/";
        private string access_token = null;
        private string oauth_consumer_key = "SIaKQTGJhdcSkZ5zzfJMtSPYz";
        private string oauth_consumer_secret = "uSxvfZ5MgTbfkxUPzLDp5sOaE3S8700cinTtS1dNVghMGN5K8E";
        private string oauth_version = "1.0";
        private string oauth_nonce = null;
        private string oauth_signature_method = "HMAC-SHA1";
        //private string oauth_token = null;
        private string oauth_timestamp = null;
        private string oauth_signature = null;
        private string oauth_callback = null;
        string oauth_url = "https://api.twitter.com/oauth2/token";

        public TwitterClient(string access_token = null)
        {
            /**this.client.BaseAddress = new Uri(baseAddress);
            this.access_token = access_token;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = Request.CreateResponse<MyObject>(HttpStatusCode.OK, objInstance);
             */
            //fomulating 
            

            
        }

        public HttpResponseMessage GetAccessToken()
        {
            
            string postBody = "grant_type=client_credentials";
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}:{2}:{4}:{5}",
                Uri.EscapeDataString(oauth_consumer_key),
                Uri.EscapeDataString(oauth_consumer_secret),
                Uri.EscapeDataString(oauth_nonce),
                Uri.EscapeDataString(oauth_signature_method),
                Uri.EscapeDataString(oauth_timestamp)))));

            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept‐Encoding", "gzip");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Host", "api.twitter.com");
            var content = new StringContent(postBody);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x‐www‐formurlencoded");
            HttpResponseMessage response = client.PostAsync(oauth_url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                response.Content.Dispose();
                return response;
            }
            
            Company c = response.Content.ReadAsAsync<Company>().Result;
            
            using (var responseStream = response.Content.ReadAsStreamAsync())
            using (var decompressedStream = new GZipStream(responseStream.Result, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(decompressedStream))
            {
                var rawJWt = streamReader.ReadToEnd(); 
                var jwt = JsonConvert.DeserializeObject(rawJWt);
                //string token = JsonConvert.DeserializeObject<String>(rawJWt);
                //JsonObjectAttribute.GetCustomAttribute(jwt ,);
            }
            
            
            response.Content.Dispose();
            return response;
        }

        private void Initialise()
        {
            this.oauth_nonce = Convert.ToBase64String(
                                  new ASCIIEncoding().GetBytes(
                                       DateTime.Now.Ticks.ToString()));

            var timeSpan = DateTime.UtcNow
                                  - new DateTime(1970, 1, 1, 0, 0, 0, 0,
                                       DateTimeKind.Utc);

            this.oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
        }

        public TokenRequest RequestToken()
        {
            Initialise();
            string url = "https://api.twitter.com/oauth/request_token";
            this.oauth_callback = "http://tariro.com";
            //this.oauth_signature = CreateSignature(url , "post");
            var authHeader = CreateSignature(url, "post");
            /*
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth",
               Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
               Uri.EscapeDataString(oauth_callback),
               Uri.EscapeDataString(oauth_consumer_key),
               Uri.EscapeDataString(oauth_signature),
               Uri.EscapeDataString(oauth_nonce),
               Uri.EscapeDataString(oauth_version),
               Uri.EscapeDataString(oauth_signature_method),
               Uri.EscapeDataString(oauth_timestamp)))));
            */

            
            /**
            var headerFormat = "oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                   "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                   "oauth_callback=\"{4}\", oauth_signature=\"{4}\", " +
                   "oauth_version=\"{5}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauth_nonce),
                                    Uri.EscapeDataString(oauth_signature_method),
                                    Uri.EscapeDataString(oauth_timestamp),
                                    Uri.EscapeDataString(oauth_consumer_key),
                                    Uri.EscapeDataString(oauth_callback),
                                    Uri.EscapeDataString(oauth_signature),
                                    Uri.EscapeDataString(oauth_version)
                            );
             */
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", authHeader);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept‐Encoding", "gzip");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Host", "api.twitter.com");

            HttpResponseMessage response = client.PostAsync(url, null).Result;
           
            if (!response.IsSuccessStatusCode)
            {
                response.Content.Dispose();
                
            }

            TokenRequest c = response.Content.ReadAsAsync<TokenRequest>().Result;
            return c;
             
        }

        public HttpResponseMessage Authorize(string oauth_token)
        {
            string url = baseUrl + "oauth/authorize?oauth_token=" + oauth_token;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                //response.Content.Dispose();

            }
            return response;
        }

        private string CreateSignature(string url , string type , string oauth_token = null)
        {
            //string builder will be used to append all the key value pairs
            type = type.ToUpper() + "&";
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(type); //POST& or GET&
            stringBuilder.Append(Uri.EscapeDataString(url));
            stringBuilder.Append("&");
 
            //the key value pairs have to be sorted by encoded key
            var dictionary = new SortedDictionary<string, string>
                                 {
                                     {"oauth_version", oauth_version},
                                     {"oauth_consumer_key", oauth_consumer_key},
                                     {"oauth_nonce", oauth_nonce},
                                     {"oauth_signature_method", oauth_signature_method},
                                     {"oauth_timestamp", oauth_timestamp},
                                     {"oauth_callback", oauth_callback}
                                 };
            
            string signatureKey = "";
            if (oauth_token != null)
            {
                signatureKey = Uri.EscapeDataString(oauth_consumer_key) + "&" +
                    Uri.EscapeDataString(oauth_token);
                dictionary.Add("oauth_token", oauth_token);
            }
            else
                signatureKey = Uri.EscapeDataString(oauth_consumer_key) + "&";
            
            foreach (var keyValuePair in dictionary)
            {
                //append a = between the key and the value and a & after the value
                stringBuilder.Append(Uri.EscapeDataString(string.Format("{0}={1}&", keyValuePair.Key, keyValuePair.Value)));
            }
            string signatureBaseString = stringBuilder.ToString().Substring(0, stringBuilder.Length - 3);
 
            //generation the signature key the hash will use
            

            var hmacsha1 = new HMACSHA1(
                new ASCIIEncoding().GetBytes(signatureKey));
 
            //hash the values
            string signatureString = Convert.ToBase64String(
                hmacsha1.ComputeHash(
                    new ASCIIEncoding().GetBytes(signatureBaseString)));
            dictionary.Add("oauth_signature", signatureString);
            var oauth_header = new StringBuilder(); 
            foreach (var keyValuePair in dictionary)
            {
                //append a = between the key and the value and a & after the value
                oauth_header.Append(string.Format("{0}=\"{1}\", ", keyValuePair.Key, Uri.EscapeDataString(keyValuePair.Value)));
            }
            return oauth_header.ToString().Substring(0, oauth_header.Length - 2); ;
        }

        /**
        public HttpResponseMessage Get(string path)
        {
            

        }

        public HttpResponseMessage Get(string path, object parameters)
        {

        }

        public HttpResponseMessage Post(string path, object parameters)
        {

        }

        public HttpResponseMessage GetAsnyc(string path, object parameters)
        {
            var response = this.client.GetAsync(path + "accessToken=" + this.access_token).Result;
            this.client.
            return response;
        }

        public HttpResponseMessage PostAsnyc(string path, object parameters)
        {

        }
        */
    }

    public class TokenRequest
    {
        string oauth_token { get; set; }
        string oauth_token_secret { get; set; }
        string oauth_callback_confirmed { get; set; }
    }
}