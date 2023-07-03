using ApsMobileApp.Views;
using ApsMobileApp.Views.CustomerViews;
using ApsMobileApp.Views.PenjualanViews;

namespace ApsMobileApp;

public partial class AccountShell : Shell
{
    public AccountShell()
    {
        InitializeComponent();
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