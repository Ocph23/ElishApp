using System.Threading.Tasks;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using WebApp.Middlewares;
using System.Collections.Generic;
using ShareModels;

namespace WebApp
{
    public class DbInitializer
    {
        public static async Task Initialize(OcphDbContext context, IUserService userService)
        {
           var trans= context.BeginTransaction();
            try
            {
                if (context.Roles.GetLastItem() == null)
                {
                    context.Roles.Insert(new Role { Name = "Administrator" });
                    context.Roles.Insert(new Role { Name = "Admin" });
                    context.Roles.Insert(new Role { Name = "Accounting" });
                    context.Roles.Insert(new Role { Name = "Sales" });
                    context.Roles.Insert(new Role { Id = 6, Name = "Customer" });


                    var adminModel = new RegisterModel { UserName = "Administrator", Password = "Sony@77" };
                    var user= await userService.Register(adminModel);
                    if (user == null)
                        throw new System.SystemException("Administrator Not Add !");
                    
                    trans.Commit();
                    await userService.AddUserRole("Administrator", user);
                }
            }
            catch (System.Exception ex)
            {
                trans.Rollback();
                throw new System.SystemException(ex.Message);
            }

        }
    }
}