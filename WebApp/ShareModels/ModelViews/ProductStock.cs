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
        public double Stock { get; set; }
        public virtual Unit SelectedUnit {

            get { return _selected; }
            set
            {
                _selected = value;
            }
        }
    }
}
