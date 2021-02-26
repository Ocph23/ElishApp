using ElishAppMobile.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ElishAppMobile.Views
{
    public partial class AboutPage : ContentPage
    {

        public AboutPage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.AboutViewModel();
            cr.Text = $"Copyright @Ocph23 2020 - {DateTime.Now.Year}";
            version.Text = $"Version : {VersionTracking.CurrentVersion} - Build:{VersionTracking.CurrentBuild}";
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var form = new BarcodeScanner();
            var vm = new BarcodeScannerViewModel();
            form.BindingContext = vm;
            vm.OnResultScanHandler += Vm_OnResultScanHandler;

            await Navigation.PushModalAsync(form);

        }

        private void Vm_OnResultScanHandler(object obj)
        {
            throw new NotImplementedException();
        }

    }

   
}