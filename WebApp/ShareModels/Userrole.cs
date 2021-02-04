using System; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{

    public class Userrole  
   {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        [NotMapped]
        public virtual Role Role { get; set; }
        [NotMapped]
        public virtual User User { get; set; }

    }
}


