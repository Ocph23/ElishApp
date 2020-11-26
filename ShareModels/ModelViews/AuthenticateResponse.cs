using ShareModels;
using System.Collections.Generic;
using System.Linq;

namespace ShareModels
{
    public class AuthenticateResponse
    {
        public AuthenticateResponse(){}

        public AuthenticateResponse(User user, string token)
        {
            this.UserName = user.UserName;
            this.Email = user.Email;
            this.Token = token;
           // this.Roles = user.Roles.Select(x => x.Role.Name);
        }

        public string UserName { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}