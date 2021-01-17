using System.Threading.Tasks;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using WebClient.Services;
using System.Collections.Generic;
using ShareModels;

namespace WebClient
{
    public class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, IUserService userService)
        {
            if (!context.Role.Any())
            {
                var trans = context.Database.BeginTransaction();
                try
                {
                    context.Role.Add(new Role { Name = "Administrator" });
                    context.Role.Add(new Role { Name = "Admin" });
                    context.Role.Add(new Role { Name = "Accounting" });
                    context.Role.Add(new Role { Name = "Sales" });
                    context.Role.Add(new Role { Name = "Customer" });


                    var adminModel = new RegisterModel { UserName = "Administrator", Password = "Sony@77" };
                    var user = await userService.Register(adminModel);
                    if (user == null)
                        throw new System.SystemException("Administrator Not Add !");

                    trans.Commit();
                    await userService.AddUserRole("Administrator", user);
                }
                catch (System.Exception ex)
                {
                    trans.Rollback();
                    throw new System.SystemException(ex.Message);
                }
               
            }
        }
    }
}