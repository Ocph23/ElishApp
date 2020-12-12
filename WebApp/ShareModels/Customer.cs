using System; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string ContactName { get; set; }

        public string Telepon { get; set; }

        public string NPWP { get; set; }
        public string Address { get; set; }

        public int UserId { get; set; }

        public virtual bool HasUser { get { return UserId == 0 ? false : true; } } 

    }
}
