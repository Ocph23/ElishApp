using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApsMobileApp
{
    public class MessageShow
    {
        public static Task InfoAsync(string message, string cancel = "close")
        {
            return Application.Current.MainPage.DisplayAlert("Info", message, cancel);
        }

        public static Task ErrorAsync(string message, string cancel = "close")
        {
            return Application.Current.MainPage.DisplayAlert("Error", message, cancel);
        }


        public static Task<bool> DialogAsync(string title, string message, string okbutton = "OK", string cancelbutton = "Cancel")
        {
            return Application.Current.MainPage.DisplayAlert(title, message, okbutton, cancelbutton);
        }
    }

}
