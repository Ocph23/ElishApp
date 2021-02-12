using System; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

          public double PayDeadLine {  get; set;} 


          public double Discount {  get; set;}

     
         public PaymentStatus Status { get; set; }

         public ActivityStatus Activity { get; set; }


        public double FeeSalesman { get; set; }



        [NotMapped]
        public PaymentType Payment{ get => PayDeadLine <= 0 ? PaymentType.PayOff : PaymentType.Credit; }
        private double _total;

        [NotMapped]
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
    


        public Penjualan()
        {
            Pembayaranpenjualan = new HashSet<Pembayaranpenjualan>();
            Items = new HashSet<Penjualanitem>();
        }


        public virtual ICollection<Penjualanitem> Items { get; set; }
        public virtual ICollection<Pembayaranpenjualan> Pembayaranpenjualan { get; set; }
    }
}


