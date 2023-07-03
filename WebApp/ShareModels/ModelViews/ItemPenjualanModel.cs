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
            PenjualanItem = item;
            Id = item.Id;
            Amount = item.Quantity;
            Real = 0;
        }

        public Penjualanitem PenjualanItem { get; }
        public int Id { get; set; }
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
                if (_unit == null)
                {
                    if(UnitId<=0)
                    {
                        if (_unit == null && Product != null && Product.Units.Any())
                            _unit = Product.Units.Where(x => x.Level == 0).FirstOrDefault();
                        if (_unit != null)
                            UnitIndex = Product.Units.ToList().IndexOf(_unit);
                    }
                    else
                    {
                        if (_unit == null && Product != null && Product.Units.Any())
                            _unit = Product.Units.Where(x => x.Id == UnitId).FirstOrDefault();
                        if (_unit != null)
                            UnitIndex = Product.Units.ToList().IndexOf(_unit);
                    }
                    
                }
                return _unit;
            }
            set
            {
                SetProperty(ref _unit, value);
                if(Product!=null)
                    UnitIndex = Product.Units.ToList().IndexOf(_unit);
            }
        }
        public virtual Product Product { get; set; }
        public ObservableCollection<Unit> Units { get; set; }
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


        private int unitIndex;

        public int UnitIndex
        {
            get { return unitIndex; }
            set {SetProperty(ref unitIndex , value); }
        }

    }
}
