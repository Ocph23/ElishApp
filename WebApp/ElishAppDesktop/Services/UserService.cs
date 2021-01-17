using ElishAppDesktop.Models;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElishAppDesktop.Services
{
    public class UserService : IUserStateService
    {
        readonly string controller = "/api/user";

        public AuthenticateResponse User { get; set; }

        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Login(UserLogin model)
        {
            try
            {
                using var client = new RestService();
                var response = await client.PostAsync($"{controller}/login", client.GenerateHttpContent(model));
                if(response.IsSuccessStatusCode)
                {
                    var result =await response.GetResult<AuthenticateResponse>();
                    if (result != null)
                    {
                        Account.SetUser(result);
                        client.SetToken(result.Token);
                        response = await client.GetAsync($"{controller}/profile");
                        if (response.IsSuccessStatusCode)
                        {
                            var profile = await response.GetResult<Profile>();
                            if (profile != null)
                            {
                               Account.SetProfile(profile);
                            }
                        }
                            return true;
                    }
                    return false;
                }else
                    throw  new SystemException(await client.Error(response));
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }
    }
}
