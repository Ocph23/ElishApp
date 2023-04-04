using ApsMobileApp.Helpers;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace ApsMobileApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SearchProductByScan : ContentPage
{
    public SearchProductByScan()
    {
        InitializeComponent();
        Load();
    }

    private async void Load()
    {
        var vm = new InputBarcodeViewModel();
        vm.AutoCount = true;
        vm.ShowAutoCount = false;
        var form = new InputBarcodeView() { BindingContext=vm};
        vm.OnResultScanHandler += async (dynamic result) => {

            if (result != null && result.Article != null)
            {
                var products = await DependencyService.Get<IProductService>().GetProductStock();
                if (products != null)
                {
                    var data = products.Where(x => x.CodeArticle == result.Article).FirstOrDefault();
                    if (data != null)
                    {
                        //await Shell.Current.Navigation.PopModalAsync();
                        //var detailForm = new ProductDetailView() { BindingContext = new ProductDetailViewModel(data) };
                        //await Shell.Current.Navigation.PushAsync(detailForm);
                        return;
                    }
                }
            }
             
            await Toas.ShowLong($"{result.Article} Not Found !");
        
        };



        await Shell.Current.Navigation.PushModalAsync(form);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Load();
    }
}