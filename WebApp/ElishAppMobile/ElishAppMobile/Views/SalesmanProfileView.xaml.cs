using ElishAppMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalesmanProfileView : ContentPage
    {
        public SalesmanProfileView()
        {
            InitializeComponent();
            BindingContext = new SalesmanProfileViewModel();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                var number = (Label)sender;
                PhoneDialer.Open(number.Text);
            }
            catch (Exception ex)
            {
                MessageHelper.ErrorAsync(ex.Message).Wait();
            }
        }
    }


    public class SalesmanProfileViewModel
    {
        Profile Karyawan { get; set; }
        public SalesmanProfileViewModel()
        {

            var result = Account.GetProfile().Result;
            Karyawan = result.Karyawan;
        }

    }
}