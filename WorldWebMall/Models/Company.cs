using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;

namespace WorldWebMall.Models
{
    public class Company
    {
        public Company()
        {
            //contains the comments that have been put by varies users on the company
            this.CComments = new HashSet<CComment>();
            //contains a list of the categories that the company deals with
            this.Categorys = new HashSet<Category>();
            this.Followers = new HashSet<Customer>();
            //contains all the broadcasts the company has made
            this.Broadcasts = new HashSet<Broadcast>();
            //contains the address of the company incase it has different locations
            this.CompanyAddresses = new HashSet<CompanyAddress>();
            this.Contacts = new HashSet<ContactNumber>();
        }

        [Key]
        public string CompanyId { get; set; }
        public string name { get; set; }
        public DateTime registrationDate { get; set; }
        public string website { get; set; }
        public string description { get; set; }
        public string theme { get; set; }

        //what services the company paid for
        public string substription { get; set; }

        public virtual Picture p_pic { get; set; }
        public virtual Picture wallpaper { get; set; }

        public virtual ICollection<CompanyAddress> CompanyAddresses { get; set; }
        public virtual ICollection<CComment> CComments { get; set; }
        public virtual HashSet<Category> Categorys { get; set; }
        public virtual ICollection<Customer> Followers { get; set; }
        public virtual ICollection<Broadcast> Broadcasts { get; set; }
        public virtual ICollection<ContactNumber> Contacts { get; set; }

        //[NotMapped]
        //public virtual ICollection<string> FollowersId { get { return Followers.Select(c => c.CustomerId).ToList(); } }


    }

    public class ContactNumber
    {
        [Key, Column(Order=1)]
        public string CompanyId { get; set; }
        [Key, Column(Order=2)]
        public string type { get; set; }
        public string numbers { get; set; }
        public string emails { get; set; }

        public virtual Company Company { get; set; }
    }

    public class CompanyAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string suburb { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public DbGeography Location { get; set; }
        //public virtual Company Company { get; set; }
    }

    public class CompanyNotification
    {
        [Key]
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public int BroadcastId { get; set; }
        public bool seen { get; set; }
        public DateTime time { get; set; }
        public string type { get; set; }//whether its a comment or a like

        /**for the customer who would have commented on either the broadcast or
         * the company profile.
         * If the Broadcast object is empty then the customer has commented on the 
         * company profile and should be directed there
         */
        public string CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Broadcast Broadcast { get; set; }
    }



    //contains comments on the company put by the customers
    public class CComment
    {
        [Key]
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public string CustomerId { get; set; }
        public string comment { get; set; }
        public DateTime time { get; set; }

        public virtual Company Company { get; set; }
        public virtual Customer Customer { get; set; }
    }

    public class Category
    {
        public Category()
        {
            //contains a list of the companies under this category
            this.companys = new HashSet<Company>();
            this.broadcasts = new HashSet<Broadcast>();
            this.Adverts = new HashSet<Advert>();
        }

        [Key]
        public string category { get; set; }

        public virtual ICollection<Company> companys { get; set; }
        public virtual ICollection<Broadcast> broadcasts { get; set; }
        public virtual ICollection<Advert> Adverts { get; set; }
    }

    public class Broadcast
    {
        public Broadcast()
        {
            
            this.BLikes = new HashSet<BLike>();
            this.BComments = new HashSet<BComment>();
            this.pictures = new HashSet<Picture>();
            this.categorys = new HashSet<Category>();
            this.Customers = new HashSet<Customer>();
        }

        [Key]
        public int BroadcastId { get; set; }
        public string CompanyId { get; set; }
        public string title { get; set; }
        public string info { get; set; }
        public DateTime time { get; set; }
        public DateTime exp { get; set; }
        public int points { get; set; }

        public virtual Company Company { get; set; }
        
        public virtual ICollection<Picture> pictures { get; set; }
        public virtual ICollection<Category> categorys { get; set; }
        public virtual ICollection<BLike> BLikes { get; set; }
        public virtual ICollection<BComment> BComments { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }

    

    public class BLike 
    {
        [Key, Column(Order = 1)]
        public int BroadcastId { get; set; }
        [Key, Column(Order = 2)]
        public string UserId { get; set; }
        public DateTime time { get; set; }
        public string CompanyId { get; set; }
        public string CustomerId { get; set; }
        public virtual Company Company { get; set; }
        //public virtual Broadcast Broadcast { get; set; }
        public virtual Customer Customer { get; set; }
        //public virtual ApplicationUser User { get; set; }

    }

    //check on how to indicte that COmpanyId is a foreign key
    public class BComment
    {
        [Key]
        public int Id { get; set; }
        public int BroadcastId { get; set; }
        public string CustomerId { get; set; }
        public string comment { get; set; }
        public DateTime time { get; set; }
        public string CompanyId { get; set; }

        public virtual Company Company {get; set;}
        public virtual Broadcast Broadcast { get; set; }
        public virtual Customer Customer { get; set; }
        
    }

    public class SetCompare<T> : IEqualityComparer<T> where T : Customer
    {
        public bool Equals(T x, T y)
        {
             
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else if (x.CustomerId == y.CustomerId)
                return true;
            else
                return false;
        }

        public int GetHashCode(T x)
        {
            return x.CustomerId.GetHashCode();
        }
    }

    public interface IEqualCompare
    {
        string GetValue();
        //public int EqualCompare();
            
    }
}