using ApsMobileApp.Models;
using ApsMobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace ApsMobileApp.Views;

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


public class SalesmanProfileViewModel:BaseViewModel
{
    public Profile Karyawan { get; set; }
    public SalesmanProfileViewModel()
    {
        var result = Account.GetProfile().Result;
        Karyawan = result.Karyawan;
    }

}