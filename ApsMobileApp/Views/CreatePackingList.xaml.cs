using ApsMobileApp.Helpers;
using ApsMobileApp.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApsMobileApp.Views;

public partial class CreatePackingList : ContentPage
{
    public CreatePackingList()
    {
        InitializeComponent();
    }
}


public class CreatePackingListViewModel :BaseViewModel
{
    public CreatePackingListViewModel(ShareModels.OrderPenjualan _order)
    {
        this.Order = _order;
        Datas = new ObservableCollection<ItemPenjualanModel>();
        QRCommand = new Command(async () => {
            var vm = new InputBarcodeViewModel();
            vm.OnResultScanHandler += Vm_OnResultScanHandler;
            var form = new InputBarcodeView() { BindingContext = vm };
            await Shell.Current.Navigation.PushModalAsync(form);

        });
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        LoadItemsCommand.Execute(null);
    }

    private async Task ExecuteLoadItemsCommand()
    {
        try
        {
            await Task.Delay(50);
            Datas.Clear();
            foreach (var item in Order.Items)
            {
                var newData = new ItemPenjualanModel()
                {
                    //Amount = item.Amount,
                    Units = new ObservableCollection<Unit>(item.Product.Units),
                    Product = item.Product,
                    //ProductId = item.ProductId,
                    Unit = item.Unit,
                    //UnitId = item.UnitId,
                    Real = 0
                };
                Datas.Add(newData);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }

    }

    private async void Vm_OnResultScanHandler(dynamic result)
    {
        if (result != null)
        {
            string article = result.Article.ToString();
            var data = Datas.Where(x => x.Product.CodeArticle == article).FirstOrDefault();
            if (data != null)
            {
                double max = data.Amount - data.Real;
                if (max <= 0)
                {
                    await Toas.ShowLong($"Sorry, {data.Product.Name}-{data.Product.Size}, Sudah Lengkap");
                    return;
                }

                if (result.Type == "Auto")
                {
                    data.Real += max < 0.5 ? max : 0.5;
                    await Toas.ShowLong($"{data.Product.Name}-{data.Product.Size}, Jumlah: {data.Real} {data.Unit.Name}, Dari : {data.Amount} { data.Unit.Name}");
                }
                else
                {
                    if ((double)result.Count > max)
                        await Toas.ShowLong($"Sorry, {data.Product.Name}-{data.Product.Size} hanya kurang  : {max} {data.Unit.Name}");
                    else
                    {
                        data.Real += (double)result.Count;
                        await Toas.ShowLong($"{data.Product.Name}-{data.Product.Size}, Jumlah: {data.Real} {data.Unit.Name}, Dari : {data.Amount} { data.Unit.Name}");
                        return;
                    }
                }
            }
            else
            {
                await Toas.ShowLong($"Error : {result.Article.ToString()} Tidak Ada Diorder !");
            }
        }
    }


    public ObservableCollection<ItemPenjualanModel> Datas { get; }

    public OrderPenjualan Order { get; set; }
    
    public Command QRCommand { get; }
    public Command LoadItemsCommand { get; }
}