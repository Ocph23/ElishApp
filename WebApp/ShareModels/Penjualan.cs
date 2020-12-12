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

          public virtual string Nomor
            {
                get
                {
                    return $"{Id}/INV-ELISH/{CreateDate.Month}/{CreateDate.Year}";
                }
            }

          public int OrderPenjualanId {  get; set;} 

          public DateTime CreateDate {  get; set;} 

          public DateTime PayDeadLine {  get; set;} 


          public double Discount {  get; set;} 

         public PaymentType Payment {  get; set;}
         public PaymentStatus Status { get; set; }

         public ActivityStatus Activity { get; set; }

        public virtual ICollection<Penjualanitem> Items { get; set; }
        private double _total;
        public virtual double Total
        {
            get
            {
                if (Items == null)
                    return 0;
                return Items.Sum(x => x.Total);
            }

            set
            {
                _total = value;
            }
        }


        public virtual Orderpenjualan OrderPenjualan{ get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Karyawan Sales { get; set; }
    }
}


