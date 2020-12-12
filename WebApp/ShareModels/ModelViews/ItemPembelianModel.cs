//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace ShareModels.ModelViews
//{
//    public class ItemPembelianModel  : BaseNotify
//    {
//        public ItemPembelianModel(){}

//        public ItemPembelianModel(PembelianItem item)
//        {
//            Id = item.Id;
//            PembelianId = item.PembelianId;
//            ProductId = item.ProductId;
//            Amount = item.Amount;
//            Real = 0;
//            UnitId = item.UnitId;
//            Unit = item.Unit;
//            Product = item.Product;
//        }


//        private double _real;
//        public int Id { get; set; }
//        public int PembelianId { get; set; }
//        public int ProductId { get; set; }
//        public double Real {
//            get
//            {
//                return _real;
//            }
//            set
//            {
//                if (_real != value)
//                {
//                    SetProperty(ref _real, value);
//                    UpdateEvent?.Invoke(this);
//                }
//                Status = value.ToString();
//            }
//        }
//        public double Amount { get; set; }
//        public int UnitId { get; set; }
//        public virtual Unit Unit { get; set; }
//        public virtual Product Product { get; set; }
//        public event Func<ItemPembelianModel, Task> UpdateEvent;



//        string _status;
//        public string Status
//        {
//            get
//            {
//                if (Real == Amount)
//                    return "Lengkap";

//                if (Real < Amount)
//                    return "Kurang";

//                return "Lebih";
//            }
//            set
//            {
//                SetProperty(ref _status, value);
//            }
//        }
//    }
//}
