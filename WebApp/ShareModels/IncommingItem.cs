using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class IncomingItem : BaseNotify
    {
        public int Id { get; set; }
        public IncomingItem(){}

        public IncomingItem(PembelianItem pembelianItem){
            //PembelianId = pembelianItem.PembelianId;
            Product = pembelianItem.Product;
            //ProductId = Product.Id;
            Unit = pembelianItem.Unit;
            Amount = pembelianItem.Amount;
        }

        public double ActualValue
        {
            get => _real;
            set
            {
                if (_real != value)
                {
                    SetProperty(ref _real, value);
                    UpdateEvent?.Invoke(this);
                }
                Status = value.ToString();
            }
        }

      //  public int PembelianId { get; set; }
      //  public int UnitId { get; set; }

        public virtual double Amount { get; set; }
        public virtual Unit Unit { 
            get => _unit; 
            set { 
                if (value != null && value != _unit)
                {
                   Unit= value;
                    if(_unit!=null && Product!=null && Product.Units != null)
                    {
                        var unit0 = Product.Units.Where(x => x.Level == 0).FirstOrDefault();
                        if (unit0 != null)
                        {
                           var baseAmount = _unit.Quantity * ActualValue;
                            ActualValue = _unit.Quantity <= value.Quantity ? baseAmount / value.Quantity : value.Quantity/baseAmount ;
                        }
                    }
                }
                SetProperty(ref _unit, value);
            } }


       // public int ProductId { get; set; }
        public event Func<IncomingItem, Task> UpdateEvent;
        public virtual string Status
        {
            get
            {
                if (ActualValue == Amount)
                    return "Lengkap";

                if (ActualValue < Amount)
                    return "Kurang";

                return "Lebih";
            }
            set
            {
                SetProperty(ref _status, value);
            }
        }

        string _status;
        private double _real;
        private Unit _unit;

        public virtual Product Product { get; set; }

        public virtual Pembelian Pembelian { get; set; }
    }
}
