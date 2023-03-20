using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShareModels;

namespace ApsWebApp
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (User)context.HttpContext.Items["User"];
            if (user == null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            if (!string.IsNullOrEmpty(Roles))
            {
                var splite = Roles.Split(",");
                var found = false;
                foreach (var item in splite)
                {
                    var role = item.ToLower().Trim();
                    if (user.UserInRole(role))
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
        }

        public string Roles { get; set; }

    }


    public static class UserExtention
    {
        public static bool UserInRole(this User user, string roleName)
        {
           try
           {
                if(user==null)
                    return false;

                if(user.Roles.Where(x=>x.Role.Name.ToLower()==roleName.ToLower()).Count()>0)
                    return true;
                return false;
           }
           catch 
           {
                return false;
           }
       }



        public static int? UserId(this ClaimsPrincipal principal)
        {
            try
            {
                var cl = principal.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                if (cl != null && !string.IsNullOrEmpty(cl.Value))
                {
                    var result = Convert.ToInt32(cl.Value);
                    if (result > 0)
                        return result;
                }

                return null;
            }
            catch 
            {
                return null;
            }
        }

        public static bool InRole(this ClaimsPrincipal principal, string roleName)
        {
            try
            {
                var cl = principal.Claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault();

                if (cl != null && !string.IsNullOrEmpty(cl.Value))
                {
                    var roles = cl.Value.Split(',').ToList();
                    if(roles.Where(x=>x.ToLower() == roleName.ToLower()).Count()>0)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}