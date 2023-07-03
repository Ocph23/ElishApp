using ApsMobileApp.ViewModels;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;


namespace ApsMobileApp.Views;
public partial class AboutPage : ContentPage
{

    public AboutPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.AboutViewModel();
        //cr.Text = $"Copyright @Ocph23 2020 - {DateTime.Now.Year}";
        //version.Text = $"Version : {VersionTracking.CurrentVersion} - Build:{VersionTracking.CurrentBuild}";
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var form = new BarcodeScannerView();
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

