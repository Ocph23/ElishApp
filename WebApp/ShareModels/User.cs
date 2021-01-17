using System; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public bool Activated { get; set; }

        [NotMapped]
        public virtual ICollection<Role> Roles { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Karyawan> Sales { get; set; }
    }
}


