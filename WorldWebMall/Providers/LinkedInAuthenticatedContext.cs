using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using System;
using System.Security.Claims;

namespace Microsoft.Owin.Security.LinkedIn
{
    // Summary:
    //     Contains information about the login session as well as the user System.Security.Claims.ClaimsIdentity.
    public class LinkedInAuthenticatedContext //: BaseContext
    {
        /**
        // Summary:
        //     Initializes a Microsoft.Owin.Security.LinkedIn.LinkedInAuthenticatedContext
        //
        // Parameters:
        //   context:
        //     The OWIN environment
        //
        //   userId:
        //     LinkedIn user ID
        //
        //   screenName:
        //     LinkedIn screen name
        //
        //   accessToken:
        //     LinkedIn access token
        //
        //   accessTokenSecret:
        //     LinkedIn access token secret
        public LinkedInAuthenticatedContext(IOwinContext context, string userId, string screenName, string accessToken, string accessTokenSecret);

        // Summary:
        //     Gets the LinkedIn access token
        public string AccessToken { get; }
        //
        // Summary:
        //     Gets the LinkedIn access token secret
        public string AccessTokenSecret { get; }
        //
        // Summary:
        //     Gets the System.Security.Claims.ClaimsIdentity representing the user
        public ClaimsIdentity Identity { get; set; }
        //
        // Summary:
        //     Gets or sets a property bag for common authentication properties
        public AuthenticationProperties Properties { get; set; }
        //
        // Summary:
        //     Gets the LinkedIn screen name
        public string ScreenName { get; }
        //
        // Summary:
        //     Gets the LinkedIn user ID
        public string UserId { get; }
        */
    }
}