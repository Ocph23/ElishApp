using ShareModels;
using System.Collections.Generic;

namespace ShareModels
{
    public class RegisterModel
    {

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Role> Roles { get; set; }
    }
}