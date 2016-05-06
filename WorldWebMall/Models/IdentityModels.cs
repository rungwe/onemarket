using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Security.Principal;

namespace WorldWebMall.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FacebookToken", this.FacebookToken));
            userIdentity.AddClaim(new Claim("LinkedInToken", this.LinkedInToken));
            userIdentity.AddClaim(new Claim("TwitterToken", this.TwitterToken));
            userIdentity.AddClaim(new Claim("FBPageId", this.FBPageId));

            
            return userIdentity;
        }

        //Extended Properties
        public string FacebookToken { get; set; }
        public string FBPageId { get; set; }
        public string LinkedInToken { get; set; }
        public string TwitterToken { get; set; }
    }

    

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

    }


}
