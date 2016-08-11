using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using ArtProject2016.Models;
using ArtProject2016.ViewModel;
using WebMatrix.WebData;

namespace ArtProject2016.Functions
{
    public class Membership
    {

        public static bool UserLogin(string userName, string password)
        {
            using (var context = new ArtContext())
            {
                var user = context.UserAccounts.Where(us => us.userName == userName);

                if (user.Any())
                {
                    if (WebSecurity.Login(userName, password, true))
                    //Crypto.VerifyHashedPassword(user.FirstOrDefault().password, password))
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }



        public static bool RegisterIsValid(registerViewModel user)
        {
            try
            {
                using(var context = new ArtContext())
                {
                //  Roles.CreateRole("Administrator");
                //    Roles.AddUserToRole("Admin", "Administrator");
                WebSecurity.CreateUserAndAccount(user.userName, user.password, new
                                                                                   {
                                                                                       firstName = user.firstName,
                                                                                       lastName = user.lastName,
                                                                                       nickName = user.nickName,
                                                                                       userType = user.userType
                                                                                   });

                   Roles.AddUserToRole(user.userName, user.userType);

                    var profile = new UserProfile()
                                     {
                                       
                                         UserAccount = context.UserAccounts.First(a => a.userName == user.userName)
                                     };

                    context.UserProfiles.Add(profile);
                  

                    context.SaveChanges();

                  //      art => art.Id, new Artist()
                  //                         {
                  //                             profileDesc = "Magaling na painter",
                  //                             UserAccount = context.UserAccounts.FirstOrDefault(d => d.firstName == "Jeryl")
                  //                         });
                //var userAccount = new UserAccount
                
                //                      {
                //                       //   userName = user.userName,

                //                      //    password = Crypto.HashPassword(user.password),

                //                      };

                //context.UserAccounts.Add(userAccount);
                //context.SaveChanges();
                WebSecurity.Login(user.userName, user.password, true);
                return true;
                    }
            }
            catch (Exception ex)
            {

                return false;
            }

        }
    }



}
