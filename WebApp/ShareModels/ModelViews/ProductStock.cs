using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels.ModelViews
{
    public class ProductStock : Product
    {
        private Unit _selected;

        public double Pembelian { get; set; }
        public double Penjualan { get; set; }


        public virtual double Stock => Pembelian - Penjualan;



        public virtual double StockView
        {
            get
            {
                if (SelectedUnit == null && Units != null)
                {
                    SelectedUnit = Units.Where(x => x.Level == 0).FirstOrDefault();
                }
                return Stock / SelectedUnit.Amount;
            }
        }
        public virtual Unit SelectedUnit {

            get { return _selected; }
            set
            {
                _selected = value;
            }
        }
    }
}
