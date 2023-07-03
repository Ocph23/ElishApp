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



namespace ApsMobileApp.Views.PenjualanViews;

[QueryProperty("PenjualanId","PenjualanId")]
public partial class PenjualanDetailView : ContentPage
{
    private int penjualanId;

    public int PenjualanId
    {
        get { return penjualanId; }
        set { 
            penjualanId = value; OnPropertyChanged(); 
            var vm = BindingContext as PenjualanDetailViewModel;
           _= vm.InitAsync(penjualanId);
        }
            
    }


    public PenjualanDetailView(PenjualanDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}


public class PenjualanDetailViewModel : BaseViewModel
{
    public PenjualanDetailViewModel(IPenjualanService _penjualanService)
    {
        OrderDetailCommand = new Command(async () => await ShowDetail());
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        Title = "Detail Penjualan";
        penjualanService = _penjualanService;
    }

    private async Task ShowDetail()
    {
        var order = await penjualanService.GetOrder(_penjualanModel.OrderId);
     //   var form = new SalesOrderView();
        //form.BindingContext = new SalesOrderViewModel(_penjualanModel);
        //await  Shell.Current.Navigation.PushModalAsync(form);
    }

    private Penjualan penjualan;

    public Command OrderDetailCommand { get; }
    public Command LoadItemsCommand { get; }

    private PenjualanAndOrderModel _penjualanModel;
    private int penjualanId;
    private readonly IPenjualanService penjualanService;

    public Penjualan Penjualan
    {
        get => penjualan;
        set => SetProperty(ref penjualan, value);
    }

    public bool AddCustomerVisible => !Account.UserInRole("Customer").Result;


    public ObservableCollection<ItemPenjualanModel> Items { get; set; } = new ObservableCollection<ItemPenjualanModel>();
    private async Task ExecuteLoadItemsCommand()
    {
        try
        {
            if (IsBusy)
                return;
            IsBusy = true;
            Penjualan = await penjualanService.GetPenjualan(penjualanId);
            IsBusy = false;
        }catch(Exception ex)
        {
            await Toas.ShowLong(ex.Message);
            if(Penjualan==null)
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

    internal Task InitAsync(int penjualanId)
    {
        this.penjualanId = penjualanId;
        LoadItemsCommand.Execute(null);
        return Task.CompletedTask;
    }
}