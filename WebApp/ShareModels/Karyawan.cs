using System; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    }

}


