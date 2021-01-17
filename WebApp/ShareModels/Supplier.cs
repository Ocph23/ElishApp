using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{
    public class Supplier
    {
        public Supplier()
        {
            Orderpembelian = new HashSet<Orderpembelian>();
            Product = new HashSet<Product>();
        }
        public int Id { get; set; }

        public string Nama { get; set; }

        public string ContactPerson { get; set; }

        public string ContactPersonName { get; set; }

        public string Address { get; set; }
        public string Telepon { get; set; }

        public string Email { get; set; }

        public string NPWP { get; set; }

        public virtual ICollection<Orderpembelian> Orderpembelian { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}


