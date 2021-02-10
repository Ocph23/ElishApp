using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ElishAppMobile
{
    public class MessageDataCenter
    {
        public string Title { get;  set; }
        public string Message { get;  set; }
        public string Ok { get;  set; }
        public string Cancel { get;  set; }
    }



    public class MessageHelper
    {
        public static Task InfoAsync(string message, string cancel="close")
        {
            return Application.Current.MainPage.DisplayAlert("Info", message, cancel);
        }

        public static Task ErrorAsync(string message, string cancel = "close")
        {
            return Application.Current.MainPage.DisplayAlert("Error", message, cancel);
        }


        public static  Task<bool> DialogAsync(string title, string message, string okbutton="OK", string cancelbutton = "Cancel")
        {
            return Application.Current.MainPage.DisplayAlert(title, message, okbutton, cancelbutton);
        }
    }
}
