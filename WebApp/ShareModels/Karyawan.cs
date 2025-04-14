using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class Karyawan
    {
        public Karyawan()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Telepon { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public virtual User User { get; set; }

        public bool IsSales => User != null && User.Roles.Where(x => x.Role.Name == "Sales").Count() > 0 ? true : false;

    }

}


