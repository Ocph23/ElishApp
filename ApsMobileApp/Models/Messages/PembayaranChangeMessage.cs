using CommunityToolkit.Mvvm.Messaging.Messages;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApsMobileApp.Models.Messages
{
    public class PembayaranPenjualanChangeMessage : ValueChangedMessage<PembayaranPenjualan>
    {
        public PembayaranPenjualanChangeMessage(PembayaranPenjualan value) : base(value)
        {
        }
    }
}
