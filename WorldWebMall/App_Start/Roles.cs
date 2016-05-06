using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WorldWebMall.Models;


namespace WorldWebMall.App_Start
{
    public static class Roles
    {
        public static void RolesSetup()
        {
            initialiseRoles();
        }

        private static void initialiseRoles()
        {
            
            List<string> userRoles = new List<string>(){"customer" , "company", "companyManager" , "merchant" };

            using (var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
                foreach (var item in userRoles)
                {
                    if (!rm.RoleExists(item))
                    {
                        var roleResult = rm.Create(new IdentityRole(item));
                        //if (!roleResult.Succeeded);
                          //  throw new ApplicationException();
                    }
                    /**var user = um.FindByName(item.Key);
                    if (!um.IsInRole(user.Id, item.Value))
                    {
                        var userResult = um.AddToRole(user.Id, item.Value);
                        if (!userResult.Succeeded)
                            throw new ApplicationException("Adding user '" + item.Key + "' to '" + item.Value + "' role failed with error(s): " + userResult.Errors);
                    }*/
                }
             

        }

    }

    
}