using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{
    public class Pembelian
    {
        public int Id { get; set; }
        public int OrderPembelianId { get; set; }
        public DateTime PayDeadLine { get; set; }
        public virtual ICollection<PembelianItem> Items { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }

    }
}


