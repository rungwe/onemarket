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

namespace WorldWebMall.SocialNetworks
{
    public class LinkedInClient
    {
        HttpClient client = new HttpClient();
        private static string client_id = null;
        private static string client_secret = null;
        private static string baseUrl = "https://www.linkedin.com/";
        private string access_token = null;

        public LinkedInClient(string access_token = null)
        {
            this.access_token = access_token;
        }

        public HttpResponseMessage PostAsync(string url, Object parameters)
        {
            
            var postBody = new ObjectContent<Object>(parameters, new JsonMediaTypeFormatter(), "application/json");
            HttpResponseMessage response = client.PostAsync(url, postBody).Result;

            return response;
        }

        public HttpResponseMessage GetAsync(string url, Dictionary<string, string> parameters = null)
        {
            if (parameters != null)
            {


                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("&");
                foreach (var keyValuePair in parameters)
                {
                    //append a = between the key and the value and a & after the value
                    stringBuilder.Append(string.Format("{0}={1}&", keyValuePair.Key, Uri.EscapeDataString(keyValuePair.Value)));
                }

                url +=  stringBuilder.ToString();
            }
            HttpResponseMessage response = client.GetAsync(url).Result;

            return response;
        }
           
    }

    public class AccessTokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }

        
        public override string ToString()
        {
            
            return base.ToString();
        }
    }
}