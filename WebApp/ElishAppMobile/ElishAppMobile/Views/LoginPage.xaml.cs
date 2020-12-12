using ElishAppMobile.ViewModels;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }
    }

    public class LoginViewModel:BaseViewModel
    {
        private string url;

        public string Url
        {
            get { return Helper.Url; }
            set { SetProperty(ref url , value);
                Helper.Url = value;
            }
        }

    }
}