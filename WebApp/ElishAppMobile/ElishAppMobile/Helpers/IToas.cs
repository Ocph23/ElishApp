using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ElishAppMobile.Helpers
{
   public interface IToas
    {
        Task ShowLong(string message);
        Task ShowShort(string message);
    }

    public class Toas
    {
        public static Task ShowLong(string message) {
           return DependencyService.Get<IToas>().ShowLong(message);
        }

        public static Task ShowShort(string message)
        {
            return DependencyService.Get<IToas>().ShowShort(message);
        }

    }
}
