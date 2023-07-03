using ShareModels;
using System.Collections.Generic;
using System.Linq;

namespace ApsWebApp
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}