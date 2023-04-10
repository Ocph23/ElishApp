using ApsMobileApp.Views;
using ApsMobileApp.Views.CustomerViews;
using ApsMobileApp.Views.PembelianViews;
using ApsMobileApp.Views.PenjualanViews;

namespace ApsMobileApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute($"//{nameof(PenjualanView)}/{nameof(PenjualanDetailView)}", typeof(PenjualanDetailView));
        Routing.RegisterRoute($"//{nameof(PenjualanView)}/{nameof(PenjualanPembayaranView)}", typeof(PenjualanPembayaranView));
        Routing.RegisterRoute($"//{nameof(PenjualanView)}/{nameof(PenjualanPembayaranView)}/{nameof(PembayaranDialogView)}", typeof(PembayaranDialogView));

        Routing.RegisterRoute($"//{nameof(PembelianView)}/{nameof(PembelianDetailView)}", typeof(PembelianDetailView));
        Routing.RegisterRoute($"//{nameof(PembelianView)}/{nameof(PembelianPembayaranView)}", typeof(PembelianPembayaranView));
        Routing.RegisterRoute($"//{nameof(PembelianView)}/{nameof(PembelianPembayaranView)}/{nameof(PembelianPembayaranDialogView)}", typeof(PembelianPembayaranDialogView));


        Routing.RegisterRoute($"//customer/{nameof(CreateCustomer)}", typeof(CreateCustomer));
        Routing.RegisterRoute($"//product/{nameof(ProductDetailView)}", typeof(ProductDetailView));
        Routing.RegisterRoute($"//{nameof(InputBarcodeView)}", typeof(InputBarcodeView));
        Routing.RegisterRoute($"//{nameof(OrdesrView)}/{nameof(SalesOrderView)}", typeof(SalesOrderView));
        Routing.RegisterRoute($"//{nameof(CustomerPageView)}/{nameof(CustomerProfileView)}", typeof(CustomerProfileView));
        BindingContext = this;
       _ = SetUserName();
    }

    private bool showPembelian;

    public bool ShowPembelian
    {
        get { return showPembelian; }
        set { showPembelian = value;
            OnPropertyChanged(nameof(ShowPembelian));
        }
    }


    private async Task SetUserName()
    {
        var result = await Account.GetProfile();
        if (result != null)
            if (result != null)
        {
            user.Text = result.Name;
        }

        if(!await Account.UserInRole("Customer") && !await Account.UserInRole("Sales"))
            ShowPembelian = true;


    }

    private async void OnMenuItemClicked(object sender, EventArgs e)
    {
        await Account.SetProfile(null);
        await Account.SetUser(null);
        await Current.GoToAsync("//LoginPage");
    }

    private int countPress;
    protected override bool OnBackButtonPressed()
    {
        countPress++;
        if (countPress >= 2)
        {
            return base.OnBackButtonPressed();
        }

       // Toas.ShowLong("Tekan Sekali Lagi Untuk Keluar !");
        Task.Run(() => ClearCount());
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