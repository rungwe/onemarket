using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WorldWebMall.Models
{
    public class MallContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public MallContext() : base("name=MallContext")
        {
            this.Configuration.LazyLoadingEnabled = false; 
        }

        public System.Data.Entity.DbSet<WorldWebMall.Models.Customer> Customers { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.TestVer> TestVers { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.CustomerAddress> CustomerAddresses { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.Test> Tests { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.Company> Companies { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.AComment> AComments { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.Advert> Adverts { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.ALike> ALikes { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.BComment> BComments { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.BLike> BLikes { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.Broadcast> Broadcasts { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.Buddy> Buddies { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.Category> Categorys { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.CComment> CComments { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.CompanyAddress> CompanyAddresses { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.CompanyNotification> CompanyNotifications { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.Picture> Pictures { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.CustomerNotification> CustomerNotifications { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.Friend> Friends { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.Request> Requests { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.ContactNumber> ContactNumbers { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.SocialNetworks> SocialNetworks { get; set; }
        public System.Data.Entity.DbSet<WorldWebMall.Models.FacebookPageTokens> FacebookPageTokens { get; set; }

    }
}
