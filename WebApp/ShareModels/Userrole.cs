using System; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{

    public class UserRole:IEntity  
   {
        public int Id { get; set; }
        public Role Role { get; set; }
        public User User { get; set; }

    }
}


