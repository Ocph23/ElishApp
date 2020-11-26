using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{ 
     public class Penjualan  
   {
          public int Id {  get; set;} 

          public int OrderPenjualanId {  get; set;} 

          public DateTime CreateDate {  get; set;} 

          public DateTime PayDeadLine {  get; set;} 

          public int UserId {  get; set;} 

          public double Discount {  get; set;} 

          public PaymentType Payment {  get; set;} 

        public virtual ICollection<Penjualanitem> Items { get; set; }
    }
}


