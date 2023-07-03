using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public enum  PaymentType
    {
          Kredit, Tunai
    }

    public enum PayType
    {
        Tunai, Transfer, Giro, Etc
    }

     public enum PaymentStatus
    {
        Belum, Panjar, Lunas, All = -1
    }

    public enum OrderStatus{
          Baru, Diproses, Selesai, Batal=-1, Semua=-2
    }

    public enum ActivityStatus
    {
        None, Created, Packing, Delivered, Complete
    }

}
