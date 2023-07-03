using ApsMobileApp.Helpers;
using ApsMobileApp.Models;
using ApsMobileApp.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;



namespace ApsMobileApp.Views.PembelianViews;

[QueryProperty("PembelianId","PembelianId")]
public partial class PembelianDetailView : ContentPage
{
    private int pembelianId;

    public int PembelianId
    {
        get { return pembelianId; }
        set { 
            pembelianId = value; OnPropertyChanged(); 
            var vm = BindingContext as PembelianDetailViewModel;
           _= vm.InitAsync(pembelianId);
        }
            
    }


    public PembelianDetailView(PembelianDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}


public class PembelianDetailViewModel : BaseViewModel
{
    public PembelianDetailViewModel(IPembelianService _pembelianService)
    {
        OrderDetailCommand = new Command(async () => await ShowDetail());
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        Title = "Detail Pembelian";
        pembelianService = _pembelianService;
    }

    private async Task ShowDetail()
    {
        var order = await pembelianService.GetOrder(_pembelianModel.OrderPembelianId);
        //   var form = new SalesOrderView();
        //form.BindingContext = new SalesOrderViewModel(_pembelianModel);
        //await  Shell.Current.Navigation.PushModalAsync(form);
    }

    private Pembelian pembelian;

    public Command OrderDetailCommand { get; }
    public Command LoadItemsCommand { get; }

    private PembelianDataModel _pembelianModel;
    private int pembelianId;
    private readonly IPembelianService pembelianService;

    public Pembelian Pembelian
    {
        get => pembelian;
        set => SetProperty(ref pembelian, value);
    }

    public bool AddCustomerVisible => !Account.UserInRole("Customer").Result;

    private async Task ExecuteLoadItemsCommand()
    {
        try
        {
            if (IsBusy)
                return;
            IsBusy = true;
            Pembelian = await pembelianService.GetPembelian(pembelianId);
            IsBusy = false;
        }catch(Exception ex)
        {
            await Toas.ShowLong(ex.Message);
            if(Pembelian==null)
            {
                _ = Back();
            }
        }
        finally 
        {
            IsBusy = false;
        }
    }

    private async Task Back()
    {
        Shell.Current.Navigation.PopAsync();
    }

    internal Task InitAsync(int pembelianId)
    {
        this.pembelianId = pembelianId;
        LoadItemsCommand.Execute(null);
        return Task.CompletedTask;
    }
}