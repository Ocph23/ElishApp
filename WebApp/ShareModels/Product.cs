using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{
    public class Product  :BaseNotify,IEntity
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string CodeName { get; set; }

        private string _codeArticle;
        public string CodeArticle {
            get => _codeArticle;
            set => SetProperty(ref _codeArticle, value);
        }

        public string Size { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }
        public Merk Merk { get; set; }
        public Supplier Supplier { get; set; }
        public Category Category { get; set; }
        public ICollection<Unit> Units { get; set; }
        //public ICollection<IncomingItem> Incomingitem { get; set; }
        //public ICollection<OrderPembelianItem> Orderpembelianitem { get; set; }
        //public ICollection<OrderPenjualanItem> Orderpenjualanitem { get; set; }
        //public ICollection<PembelianItem> PembelianItem { get; set; }
        //public ICollection<Penjualanitem> PenjualanItem { get; set; }
        public ICollection<ProductImage> ProductImage { get; set; }
        public Unit UnitSelected => Units!=null&& Units.Count>0?Units.FirstOrDefault():new Unit();
        
    }
}


