using ElishAppMobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views.CustomerViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerProfileView : ContentPage
    {
        public CustomerProfileView()
        {
            InitializeComponent();
            BindingContext = new CustomerProfileViewModel();
        }
    }


    public class CustomerProfileViewModel
    {
       public Profile MyProfile { get; set; }
        public CustomerProfileViewModel()
        {
            MyProfile = Account.GetProfile().Result;
        }
    }
}