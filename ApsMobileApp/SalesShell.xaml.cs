using ApsMobileApp.Helpers;
using ApsMobileApp.Views;
using ApsMobileApp.Views.CustomerViews;
using ApsMobileApp.Views.PenjualanViews;
using System;
using System.Threading.Tasks;


namespace ApsMobileApp;

public partial class SalesShell : Shell
{
    public SalesShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(IncomingCheckView), typeof(IncomingCheckView));
        Routing.RegisterRoute($"//{nameof(PenjualanView)}/{nameof(PenjualanDetailView)}", typeof(PenjualanDetailView));
        Routing.RegisterRoute($"//customer/{nameof(CreateCustomer)}", typeof(CreateCustomer));
        Routing.RegisterRoute($"//product/{nameof(ProductDetailView)}", typeof(ProductDetailView));
        Routing.RegisterRoute($"//{nameof(InputBarcodeView)}", typeof(InputBarcodeView));
        Routing.RegisterRoute($"//{nameof(OrdesrView)}/{nameof(SalesOrderView)}", typeof(SalesOrderView));
        Routing.RegisterRoute($"//{nameof(CustomerPageView)}/{nameof(CustomerProfileView)}", typeof(CustomerProfileView));
        _ = Load();
    }

    private async Task Load()
    {
        var result = await Account.GetProfile();
        if (result != null)
        {
            user.Text = result.Name;
        }
    }

    private async void OnMenuItemClicked(object sender, EventArgs e)
    {
        await Current.GoToAsync("//LoginPage");
    }

    private int countPress;
    protected override bool OnBackButtonPressed()
    {
        countPress++;
        if (countPress >=2)
        {
            return base.OnBackButtonPressed();
        }

        Toas.ShowLong("Tekan Sekali Lagi Untuk Keluar !");
        Task.Run(()=>ClearCount());
        return true;
    }

    private Task ClearCount()
    {
        Device.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(2000);
            countPress = 0;
        });

        return Task.CompletedTask;
    }
}
