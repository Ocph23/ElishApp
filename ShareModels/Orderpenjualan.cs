using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{ 
     public class Orderpenjualan  
   {
          public int Id {  get; set;} 

          public DateTime OrderDate {  get; set;} 

          public int CustomerId {  get; set;} 
          public double Discount {  get; set; }
        public int? SalesId { get; set; }
        public virtual ICollection<OrderPenjualanItem> Items { get; set; }
    }
}


