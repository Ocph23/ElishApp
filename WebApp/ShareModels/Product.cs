using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{
    public class Product  :BaseNotify
    {

        public Product()
        {
            Incomingitem = new HashSet<IncomingItem>();
            Orderpembelianitem = new HashSet<OrderpembelianItem>();
            Orderpenjualanitem = new HashSet<OrderPenjualanItem>();
            PembelianItem = new HashSet<PembelianItem>();
            PenjualanItem = new HashSet<Penjualanitem>();
            Units = new HashSet<Unit>();
        }

        private string _codeArticle;

        public int Id { get; set; }

        public int CategoryId { get; set; }
        public int SupplierId { get; set; }

        public string Merk { get; set; }

        public string Name { get; set; }

        public string CodeName { get; set; }

        public string CodeArticle {
            get => _codeArticle;
            set => SetProperty(ref _codeArticle, value);
        }

        public string Size { get; set; }
        public string Description { get; set; }

        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<Unit> Units { get; set; }
        public virtual ICollection<IncomingItem> Incomingitem { get; set; }
        public virtual ICollection<OrderpembelianItem> Orderpembelianitem { get; set; }
        public virtual ICollection<OrderPenjualanItem> Orderpenjualanitem { get; set; }
        public virtual ICollection<PembelianItem> PembelianItem { get; set; }
        public virtual ICollection<Penjualanitem> PenjualanItem { get; set; }
    }
}


