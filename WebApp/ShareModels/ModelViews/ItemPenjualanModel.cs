using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels.ModelViews
{
    public class ItemPenjualanModel : BaseNotify
    {

        public ItemPenjualanModel() { }
        public ItemPenjualanModel(Penjualanitem item)
        {
            Id = item.Id;
            PenjualanId = item.PenjualanId;
            ProductId = item.ProductId;
            Amount = item.Amount;
            Real = 0;
            UnitId = item.UnitId;
            Unit = item.Unit;
            Product = item.Product;
        }

        public int Id { get; set; }
        public int PenjualanId { get; set; }
        public int ProductId { get; set; }
        public double Real {
            get
            {
                return _real;
            }
            set
            {
                if (_real != value)
                {
                    SetProperty(ref _real, value);
                    Total = value;
                    UpdateEvent?.Invoke(this);
                }
                Status = value.ToString();
            }
        }
        public double Amount {
            get => _amount;
            set  {
                SetProperty(ref _amount, value);
               
            }
        }
        public int UnitId { get; set; }
        public virtual Unit Unit
        {
            get {
                if (_unit == null && Product != null && Product.Units.Any())
                    _unit = Product.Units.Where(x => x.Level == 0).FirstOrDefault();
                    return _unit;
            }
            set => SetProperty(ref _unit, value);
        }
        public virtual Product Product { get; set; }
        public virtual ObservableCollection<Unit> Units { get; set; }
        public event Func<ItemPenjualanModel, Task> UpdateEvent;
        string _status;
        private double _amount;
        private double _real;
        private Unit _unit;
        private double _total;

        public virtual double Total {
            get
            {
                if(Unit!=null)
                    return Real * Unit.Sell;
                return _total;
            }
            set
            {
              SetProperty(ref _total , value);
            }
        }

        public string Status
        {
            get
            {
                if (Real == Amount)
                    return "Lengkap";

                if (Real < Amount)
                    return "Kurang";

                return "Lebih";
            }
            set
            {
                SetProperty(ref _status, value);
            }
        }
    }
}
