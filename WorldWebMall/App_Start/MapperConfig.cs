using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorldWebMall.Models;
using AutoMapper.QueryableExtensions.Impl;
using AutoMapper.Impl;
using AutoMapper;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Spatial;


namespace WorldWebMall.App_Start
{
    public static class MapperConfig
    {
        public static UserManager<ApplicationUser, string> manager = new UserManager<ApplicationUser, string>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        public static void Configure()
        {
            ConfigureMapping();
        }
    /**    Mapper.CreateMap<Customer, CustomerDto>()
    .ForMember(d => d.FullName, opt => opt.MapFrom(c => c.FirstName + " " + c.LastName))
    .ForMember(d => d.TotalContacts, opt => opt.MapFrom(c => c.Contacts.Count()));*/

        public static string GetRole(string id)
        {
            IList<string> roles = UserManagerExtensions.GetRoles(manager, id);

            if (roles.Contains("customer"))
            {
                
                return "customer";
            }
            else if(roles.Contains("company"))
            {
               
                return "company";
            }

            return null;
        }

        public static void ConfigureMapping()
        {
        
            //should modify to use stringbuilder
            Mapper.CreateMap<Customer, CustomerDTO>()
                .ForMember(a => a.ID, opt => opt.MapFrom(c => c.CustomerId))
                .ForMember(a => a.fname, opt => opt.MapFrom(c => c.fname))
                .ForMember(a => a.lname, opt => opt.MapFrom(c => c.lname))
                .ForMember(a => a.profile_pic, opt => opt.MapFrom(c => c.p_pic));

            Mapper.CreateMap<Company, CompanyDTO>()
                .ForMember(a => a.ID, opt => opt.MapFrom(c => c.CompanyId))
                .ForMember(a => a.name, opt => opt.MapFrom(c => c.name))
                .ForMember(a => a.wallpaper, opt => opt.MapFrom(c => c.wallpaper))
                .ForMember(a => a.number_of_followers, opt => opt.MapFrom(c => c.Followers.Count()))
                .ForMember(a => a.profile_pic, opt => opt.MapFrom(c => c.p_pic));

            Mapper.CreateMap<CustomerAddress, Location>()
                .ForMember(a => a.surburb, opt => opt.MapFrom(c => c.suburb))
                .ForMember(a => a.city, opt => opt.MapFrom(c => c.city))
                .ForMember(a => a.country, opt => opt.MapFrom(c => c.country));

            Mapper.CreateMap<Address, CompanyAddress>()
                .ForMember(a => a.address1, opt => opt.MapFrom(c => c.address1))
                .ForMember(a => a.address2, opt => opt.MapFrom(c => c.address2))
                .ForMember(a => a.suburb, opt => opt.MapFrom(c => c.suburb))
                .ForMember(a => a.city, opt => opt.MapFrom(c => c.city))
                .ForMember(a => a.country, opt => opt.MapFrom(c => c.country))
                .ForMember(a => a.Location, opt => opt.MapFrom(c => DbGeography.FromText(string.Format("POINT({0} {1})", c.longitude, c.latitude))));

            Mapper.CreateMap<Address, CustomerAddress>()
                .ForMember(a => a.address1, opt => opt.MapFrom(c => c.address1))
                .ForMember(a => a.address2, opt => opt.MapFrom(c => c.address2))
                .ForMember(a => a.suburb, opt => opt.MapFrom(c => c.suburb))
                .ForMember(a => a.city, opt => opt.MapFrom(c => c.city))
                .ForMember(a => a.country, opt => opt.MapFrom(c => c.country))
                .ForMember(a => a.Location, opt => opt.MapFrom(c => DbGeography.FromText(string.Format("POINT({0} {1})", c.longitude, c.latitude))));

            Mapper.CreateMap<CompanyAddress, Address>();

            Mapper.CreateMap<CustomerAddress, Address>();

            Mapper.CreateMap<Company, CompanyDetails>()
                .ForMember(a => a.ID, opt => opt.MapFrom(c => c.CompanyId))
                .ForMember(a => a.name, opt => opt.MapFrom(c => c.name))
                .ForMember(a => a.wallpaper, opt => opt.MapFrom(c => c.wallpaper))
                .ForMember(a => a.contacts, opt => opt.MapFrom(c => c.Contacts))
                .ForMember(a => a.addresses, opt => opt.MapFrom(c => c.CompanyAddresses))
                .ForMember(a => a.category, opt => opt.MapFrom(c => c.Categorys))
                .ForMember(a => a.website, opt => opt.MapFrom(c => c.website))
                .ForMember(a => a.profile_pic, opt => opt.MapFrom(c => c.p_pic))
                .ForMember(a => a.overview, opt => opt.MapFrom(c => c.description))
                .ForMember(a => a.regDate, opt => opt.MapFrom(c => c.registrationDate))
                .ForMember(a => a.color, opt => opt.MapFrom(c => c.substription == "premium" ? c.theme : "default"))
                .ForMember(a => a.number_of_followers, opt => opt.MapFrom(c => c.Followers.Count()));
                //because of possible concurrent write, number_of_followers is better off calculated
                //more than one user can possibly click the follow button at close enough time to cause
                //error. However not sure if this is well taken of by EF

            Mapper.CreateMap<Broadcast, BroadcastDTO>()
                .ForMember(a => a.ID, opt => opt.MapFrom(c => c.BroadcastId))
                .ForMember(a => a.likes, opt => opt.MapFrom(c => c.BLikes.Take(20))) //return only 20, to view more, an api call for GetBLikes is made
                .ForMember(a => a.comments, opt => opt.MapFrom(c => c.BComments.Take(10))) //return only 10, to view more, an api call for GetBComments is made
                .ForMember(a => a.details, opt => opt.MapFrom(c => c.info))
                .ForMember(a => a.number_of_likes, opt => opt.MapFrom(c => c.BLikes.Count()))
                .ForMember(a => a.number_of_comments, opt => opt.MapFrom(c => c.BComments.Count()))
                .ForMember(a => a.number_of_views, opt => opt.MapFrom(c => 0))
                .ForMember(a => a.title, opt => opt.MapFrom(c => c.title))
                .ForMember(a => a.minutes, opt => opt.MapFrom(c => DbFunctions.DiffMinutes(c.time, DateTime.UtcNow) > 60 ? 0 : DbFunctions.DiffMinutes(c.time, DateTime.UtcNow)))
                .ForMember(a => a.hours, opt => opt.MapFrom(c => DbFunctions.DiffHours(c.time, DateTime.UtcNow) > 24 ? 0 : DbFunctions.DiffHours(c.time, DateTime.UtcNow)))
                .ForMember(a => a.date, opt => opt.MapFrom(c => c.time))
                .ForMember(a => a.images, opt => opt.MapFrom(c => c.pictures))
                .ForMember(a => a.company, opt => opt.MapFrom(c => c.Company));


            Mapper.CreateMap<Customer, Feeds>()
                .ForMember(a => a.feeds, opt => opt.MapFrom(c => c.Broadcasts));

            Mapper.CreateMap<Category, Suggestions>()
                .ForMember(a => a.suggestions, opt => opt.MapFrom(c => c.companys.OrderBy(x =>x.Followers.Count())));
           
            Mapper.CreateMap<Customer, CustomerDetails>()
                .ForMember(a => a.ID, opt => opt.MapFrom(c => c.CustomerId))
                .ForMember(a => a.fname, opt => opt.MapFrom(c => c.fname))
                .ForMember(a => a.lname, opt => opt.MapFrom(c => c.lname))
                .ForMember(a => a.profile_pic, opt => opt.MapFrom(c => c.p_pic))
                .ForMember(a => a.contacts, opt => opt.MapFrom(c => c.contacts))
                .ForMember(a => a.gender, opt => opt.MapFrom(c => c.gender))
                .ForMember(a => a.active_ads, opt => opt.MapFrom(c => c.active_ads))
                .ForMember(a => a.num_of_followigs, opt => opt.MapFrom(c => c.Followings.Count()))
                .ForMember(a => a.num_friends, opt => opt.MapFrom(c => c.num_friends))
                .ForMember(a => a.date_of_birth, opt => opt.MapFrom(c => c.dob))
                .ForMember(a => a.location, opt => opt.MapFrom(c => c.CustomerAddress));
                //.ForMember(a => a.location, opt => opt.MapFrom(c => Mapper.Map<Customer, Location>(c)));

            Mapper.CreateMap<CustomerData, Customer>()
                .ForMember(a => a.fname, opt => opt.Condition(c => c.fname != null))
                .ForMember(a => a.mname, opt => opt.Condition(c => c.mname != null))
                .ForMember(a => a.lname, opt => opt.Condition(c => c.lname != null))
                .ForMember(a => a.contacts, opt => opt.Condition(c => c.contacts == 0))
                .ForMember(a => a.dob, opt => opt.Condition(c => c.date_of_birth != null))
                .ForMember(a => a.gender, opt => opt.Condition(c => c.gender != null));                

            Mapper.CreateMap<CompanyData, Company>()
                .ForMember(a => a.name, opt => opt.Condition(c => c.name != null))
                .ForMember(a => a.website, opt => opt.Condition(c => c.website != null))
                .ForMember(a => a.theme, opt => opt.Condition(c => c.color != null))
                .ForMember(a => a.description, opt => opt.Condition(c => c.overview != null));

            Mapper.CreateMap<ContactDTO, ContactNumber>()
                .ForMember(a => a.numbers, opt => opt.MapFrom(c => c.numbers))
                .ForMember(a => a.emails, opt => opt.MapFrom(c => c.emails))
                .ForMember(a => a.type, opt => opt.MapFrom(c => c.type));

            Mapper.CreateMap<ContactNumber, ContactDTO>()
                .ForMember(a => a.emails, opt => opt.MapFrom(c => c.emails))
                .ForMember(a => a.numbers, opt => opt.MapFrom(c => c.numbers))
                .ForMember(a => a.type, opt => opt.MapFrom(c => c.type));

            Mapper.CreateMap<Advert, Advertisement>()
                .ForMember(a => a.ID, opt => opt.MapFrom(c => c.Id))
                .ForMember(a => a.title, opt => opt.MapFrom(c => c.title))
                .ForMember(a => a.details, opt => opt.MapFrom(c => c.info))
                .ForMember(a => a.price, opt => opt.MapFrom(c => c.price))
                .ForMember(a => a.minutes, opt => opt.MapFrom(c => DbFunctions.DiffMinutes(c.time, DateTime.UtcNow) > 60 ? 0 : DbFunctions.DiffMinutes(c.time, DateTime.UtcNow)))
                .ForMember(a => a.hours, opt => opt.MapFrom(c => DbFunctions.DiffHours(c.time, DateTime.UtcNow) > 24 ? 0 : DbFunctions.DiffHours(c.time, DateTime.UtcNow)))
                .ForMember(a => a.date, opt => opt.MapFrom(c => c.time))
                .ForMember(a => a.number_of_comments, opt => opt.MapFrom(c => c.AComments.Count()))
                .ForMember(a => a.number_of_likes, opt => opt.MapFrom(c => c.ALikes.Count()))
                .ForMember(a => a.number_of_views, opt => opt.MapFrom(c => 0)) //need to work on it
                .ForMember(a => a.images, opt => opt.MapFrom(c => c.pictures))
                .ForMember(a => a.likes, opt => opt.MapFrom(c => c.ALikes))
                .ForMember(a => a.comments, opt => opt.MapFrom(c => c.AComments))
                .ForMember(a => a.longitude, opt => opt.MapFrom(c => c.Location.Longitude))
                .ForMember(a => a.latitude, opt => opt.MapFrom(c => c.Location.Latitude))
                .ForMember(a => a.seller, opt => opt.MapFrom(c => c.Customer));

            Mapper.CreateMap<ALike, AdLike>()
                .ForMember(a => a.user, opt => opt.MapFrom(c => c.Customer));

            Mapper.CreateMap<AComment, AdComment>()
                .ForMember(a => a.comment, opt => opt.MapFrom(c => c.comment))
                .ForMember(a => a.ID, opt => opt.MapFrom(c => c.Id))
                .ForMember(a => a.minutes, opt => opt.MapFrom(c => DbFunctions.DiffMinutes(c.time, DateTime.UtcNow) > 60 ? 0 : DbFunctions.DiffMinutes(c.time, DateTime.UtcNow)))
                .ForMember(a => a.hours, opt => opt.MapFrom(c => DbFunctions.DiffHours(c.time, DateTime.UtcNow) > 24 ? 0 : DbFunctions.DiffHours(c.time, DateTime.UtcNow)))
                .ForMember(a => a.date, opt => opt.MapFrom(c => c.time));

            Mapper.CreateMap<BLike, BroadcastLike>()
                .ForMember(a => a.company, opt => opt.MapFrom(c => c.Company))
                .ForMember(a => a.customer, opt => opt.MapFrom(c => c.Customer));

            //Mapper.CreateMap<Category, string>();


            //Mapper.CreateMap<string, Category>();
                

            Mapper.CreateMap<BComment, BroadcastComment>()
                .ForMember(a => a.comment, opt => opt.MapFrom(c => c.comment))
                .ForMember(a => a.ID, opt => opt.MapFrom(c => c.Id))
                .ForMember(a => a.minutes, opt => opt.MapFrom(c => DbFunctions.DiffMinutes(c.time, DateTime.UtcNow) > 60 ? 0 : DbFunctions.DiffMinutes(c.time, DateTime.UtcNow)))
                .ForMember(a => a.hours, opt => opt.MapFrom(c => DbFunctions.DiffHours(c.time, DateTime.UtcNow) > 24 ? 0 : DbFunctions.DiffHours(c.time, DateTime.UtcNow)))
                .ForMember(a => a.company, opt => opt.MapFrom(c => c.Company))
                .ForMember(a => a.customer, opt => opt.MapFrom(c => c.Customer))
                .ForMember(a => a.date, opt => opt.MapFrom(c => c.time));

            Mapper.CreateMap<CComment, CompanyComment>()
                .ForMember(a => a.comment, opt => opt.MapFrom(c => c.comment))
                .ForMember(a => a.ID, opt => opt.MapFrom(c => c.Id))
                .ForMember(a => a.minutes, opt => opt.MapFrom(c => DbFunctions.DiffMinutes(c.time, DateTime.UtcNow) > 60 ? 0 : DbFunctions.DiffMinutes(c.time, DateTime.UtcNow)))
                .ForMember(a => a.hours, opt => opt.MapFrom(c => DbFunctions.DiffHours(c.time, DateTime.UtcNow) > 24 ? 0 : DbFunctions.DiffHours(c.time, DateTime.UtcNow)))
                .ForMember(a => a.company, opt => opt.MapFrom(c => c.Company))
                .ForMember(a => a.customer, opt => opt.MapFrom(c => c.Customer))
                .ForMember(a => a.date, opt => opt.MapFrom(c => c.time));

            Mapper.CreateMap<Picture, PictureDTO>()
                .ForMember(a => a.Id, opt => opt.MapFrom(c => c.Id))
                .ForMember(a => a.url, opt => opt.MapFrom(c => c.path));

            Mapper.CreateMap<Category, CategoryDTO>()
                .ForMember(a => a.category, opt => opt.MapFrom(c => c.category));

            Mapper.CreateMap<CategoryDTO, Category>()
                .ForMember(a => a.category, opt => opt.MapFrom(c => c.category));

            
            Mapper.CreateMap<CreateAd, Advert>()
                .ForMember(a => a.exp, opt => opt.MapFrom(c => DateTime.UtcNow.AddDays(20)))
                .ForMember(a => a.info, opt => opt.MapFrom(c => c.details))
                .ForMember(a => a.time, opt => opt.MapFrom(c => DateTime.UtcNow))
                .ForMember(a => a.title, opt => opt.MapFrom(c => c.title));
            
            
            Mapper.CreateMap<CreateB, Broadcast>()
                //.ForMember(a => a.categorys, opt => opt.MapFrom(c => c.categories))
                .ForMember(a => a.exp, opt => opt.MapFrom(c => DateTime.UtcNow.AddDays(30)))
                .ForMember(a => a.info, opt => opt.MapFrom(c => c.details))
                .ForMember(a => a.points, opt => opt.MapFrom(c => 0))
                .ForMember(a => a.time, opt => opt.MapFrom(c => DateTime.UtcNow))
                .ForMember(a => a.title, opt => opt.MapFrom(c => c.title));                

            Mapper.CreateMap<CustomerNotification, Notification>()
                .ForMember(a => a.Id, opt => opt.MapFrom(c => c.Id))
                .ForMember(a => a.seen, opt => opt.MapFrom(c => c.seen))
                .ForMember(a => a.customer, opt => opt.MapFrom(c => c.User))
                .ForMember(a => a.minutes, opt => opt.MapFrom(c => DbFunctions.DiffMinutes(c.time, DateTime.UtcNow) > 60 ? 0 : DbFunctions.DiffMinutes(c.time, DateTime.UtcNow)))
                .ForMember(a => a.hours, opt => opt.MapFrom(c => DbFunctions.DiffHours(c.time, DateTime.UtcNow) > 24 ? 0 : DbFunctions.DiffHours(c.time, DateTime.UtcNow)))
                .ForMember(a => a.date, opt => opt.MapFrom(c => c.time))
                .ForMember(a => a.type, opt => opt.MapFrom(c => c.type));

            Mapper.CreateMap<CompanyNotification, Notification>()
                .ForMember(a => a.Id, opt => opt.MapFrom(c => c.Id))
                .ForMember(a => a.seen, opt => opt.MapFrom(c => c.seen))
                .ForMember(a => a.customer, opt => opt.MapFrom(c => c.Customer))
                .ForMember(a => a.minutes, opt => opt.MapFrom(c => DbFunctions.DiffMinutes(c.time, DateTime.UtcNow) > 60 ? 0 : DbFunctions.DiffMinutes(c.time, DateTime.UtcNow)))
                .ForMember(a => a.hours, opt => opt.MapFrom(c => DbFunctions.DiffHours(c.time, DateTime.UtcNow) > 24 ? 0 : DbFunctions.DiffHours(c.time, DateTime.UtcNow)))
                .ForMember(a => a.date, opt => opt.MapFrom(c => c.time))
                .ForMember(a => a.type, opt => opt.MapFrom(c => c.type));

            Mapper.CreateMap<Request, FriendRequest>()
                .ForMember(a => a.Id, opt => opt.MapFrom(c => c.Id))
                .ForMember(a => a.seen, opt => opt.MapFrom(c => c.seen))
                .ForMember(a => a.friend, opt => opt.MapFrom(c => c.Sender));

            Mapper.CreateMap<Request, AcceptedRequest>()
                .ForMember(a => a.Id, opt => opt.MapFrom(c => c.Id))
                .ForMember(a => a.seen, opt => opt.MapFrom(c => c.seen))
                .ForMember(a => a.friend, opt => opt.MapFrom(c => c.Receiver));

            /**
            Mapper.CreateMap<Customer, User>()
                .ForMember(a => a.Id, opt => opt.MapFrom(c => c.CustomerId))
                .ForMember(a => a.fname, opt => opt.MapFrom(c => c.fname))
                .ForMember(a => a.lname, opt => opt.MapFrom(c => c.lname))
                .ForMember(a => a.profile_pic, opt => opt.MapFrom(c => c.p_pic.path));

            Mapper.CreateMap<Company, User>()
                .ForMember(a => a.Id, opt => opt.MapFrom(c => c.CompanyId))
                .ForMember(a => a.fname, opt => opt.MapFrom(c => c.name))
                .ForMember(a => a.profile_pic, opt => opt.MapFrom(c => c.p_pic));
            */
            Mapper.CreateMap<Customer, UserDTO>()
                .ForMember(a => a.Id, opt => opt.MapFrom(c => c.CustomerId))
                .ForMember(a => a.fname, opt => opt.MapFrom(c => c.fname))
                .ForMember(a => a.lname, opt => opt.MapFrom(c => c.lname));                

            Mapper.CreateMap<Company, UserDTO>()
                .ForMember(a => a.Id, opt => opt.MapFrom(c => c.CompanyId))
                .ForMember(a => a.fname, opt => opt.MapFrom(c => c.name));

            Mapper.CreateMap<Customer, Seller>()
                .ForMember(a => a.Id, opt => opt.MapFrom(c => c.CustomerId))
                .ForMember(a => a.fname, opt => opt.MapFrom(c => c.fname))
                .ForMember(a => a.lname, opt => opt.MapFrom(c => c.lname))
                .ForMember(a => a.profile_pic, opt => opt.MapFrom(c => c.p_pic.path))
                .ForMember(a => a.contacts, opt => opt.MapFrom(c => c.contacts))
                .ForMember(a => a.email, opt => opt.MapFrom(c => c.email));

            Mapper.CreateMap<Company, CategoryCompanies>()
                .ForMember(a => a.category, opt => opt.MapFrom(c => ""))
                .ForMember(a => a.Companies, opt => opt.MapFrom(c => c));
        }
    }
}
