using System;
using System.Collections.Generic;
using System.Linq;
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

namespace BitCode.Owin.Security.LinkedIn
{
    // Summary:
    //     Default Microsoft.Owin.Security.LinkedIn.ILinkedInAuthenticationProvider implementation.
    public class LinkedInAuthenticationProvider 
    {
        HttpClient client = new HttpClient();
        private static string client_id = "77j50rlnfu0gxd";
        private static string client_secret = "QMaUH4zSOJF4fkUk";
        private static string baseUrl = "https://www.linkedin.com/uas/";

        public HttpResponseMessage Authorize()
        {
            string url = baseUrl + "oauth2/authorization";
            StringBuilder stringBuilder = new StringBuilder();
            Dictionary<string, string> parameters = new Dictionary<string,string>
            {
                {"response_type", "code"},
                {"client_id", client_id},
                {"redirect_url", "http://localhost:54445/external/linkedin_authorization"},
                {"state", GenerateStateCode()},
                {"scope", "rw_company_admin"}
            };

            foreach (var keyValuePair in parameters)
            {
                //append a = between the key and the value and a & after the value
                stringBuilder.Append(string.Format("{0}={1}&", keyValuePair.Key, Uri.EscapeDataString(keyValuePair.Value)));
            }
            url += "?" + stringBuilder.ToString();
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                //response.Content.Dispose();

            }
            return response;
        }

        private string GenerateStateCode()
        {
            return "";
        }

        public HttpResponseMessage GetAccessToken(string code)
        {
            string url = baseUrl + "oauth2/accessToken";
            StringBuilder stringBuilder = new StringBuilder();
            Dictionary<string, string> parameters = new Dictionary<string,string>
            {
                {"grant_type", "authorization_code"},
                {"code", code},
                {"redirect_url", "http://localhost:54445/external/linkedin_authorization"},
                {"client_id", client_id},
                {"client_secret", GenerateStateCode()}
            };

            foreach (var keyValuePair in parameters)
            {
                //append a = between the key and the value and a & after the value
                stringBuilder.Append(string.Format("{0}={1}&", keyValuePair.Key, Uri.EscapeDataString(keyValuePair.Value)));
            }
            var postBody = new StringContent(stringBuilder.ToString());
            HttpResponseMessage response = client.PostAsync(url , postBody).Result;

            if (!response.IsSuccessStatusCode)
            {
                //response.Content.Dispose();

            }

            AccessTokenResponse token = response.Content.ReadAsAsync<AccessTokenResponse>().Result;
            return response;
        }

        public HttpResponseMessage GetPageNames(string token)
        {
            string url = "https://api.linkedin.com/v1/companies?format=json&is-company-admin=true";
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            client.DefaultRequestHeaders.Connection.TryParseAdd("Keep-Alive");
            HttpResponseMessage response = client.GetAsync(url).Result;

            return response;

        }

        
    }

    public class AccessTokenResponse
    {
        string access_token { get; set; }
        int expires_in { get; set; }
    }
}
