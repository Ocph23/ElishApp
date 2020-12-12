using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public enum  PaymentType
    {
          Credit, PayOff
    }

    public enum PayType
    {
        Chash, Transfer, Giro, Etc
    }

     public enum PaymentStatus
    {
        None, DownPayment, PaidOff  , All = -1
    }

    public enum OrderStatus{
          New, Proccess, Complete, Cancel=-1, All=-2
    }

    public enum ActivityStatus
    {
        None, Created, Packing, Delivered, Complete
    }
}
