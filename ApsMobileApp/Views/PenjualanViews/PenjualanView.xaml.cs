using ApsMobileApp.Models;
using ApsMobileApp.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;




namespace ApsMobileApp.Views.PenjualanViews;

public partial class PenjualanView : ContentPage
{
    private PenjualanViewModel _viewModel;

    public PenjualanView(PenjualanViewModel vm)
    {
        InitializeComponent();
        BindingContext= _viewModel = vm;
    }
    private void Search_OnSearchFound(object data)
    {
        _viewModel.Search(data.ToString());
    }
}


public class PenjualanViewModel : BaseViewModel
{
    private ObservableCollection<PenjualanAndOrderModel> _SourceItems;
    private bool fromdetail;
    private readonly IPenjualanService penjualanService;

    public Command LoadItemsCommand { get; }
    public Command AddNewCommand { get; }
    public Command SelectCommand { get; }
    public Command PembayaranCommand { get; }
    public Profile UserProfile { get; set; }
    public ObservableCollection<PenjualanAndOrderModel> Items { get; private set; } = new ObservableCollection<PenjualanAndOrderModel>();

    public PenjualanViewModel(IPenjualanService _penjualanService)
    {
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        AddNewCommand = new Command(AddNewCommandAction);
        SelectCommand = new Command(async (x)=> await SelectCommandAction(x));
        PembayaranCommand = new Command(async (x)=> await PembayaranCommandAction(x));
        LoadItemsCommand.Execute(null);
        penjualanService = _penjualanService;
    }

    private async Task SelectCommandAction(object obj)
    {
        IsBusy = true;
        fromdetail = true;
        var order = (PenjualanAndOrderModel)obj;
        //var vm = new PenjualanDetailViewModel(order);

        var dataParameter = new Dictionary<string, object>() { 
             {"PenjualanId", order.PenjualanId }
        };

       await Shell.Current.GoToAsync($"//{nameof(PenjualanView)}/{nameof(PenjualanDetailView)}", dataParameter);

        await Task.Delay(1000);
        IsBusy = false;
       
    }

    private async Task PembayaranCommandAction(object obj)
    {
        IsBusy = true;
        fromdetail = true;
        var order = (PenjualanAndOrderModel)obj;
        //var vm = new PenjualanDetailViewModel(order);

        var dataParameter = new Dictionary<string, object>() {
             {"Penjualan", order }
        };

        await Shell.Current.GoToAsync($"//{nameof(PenjualanView)}/{nameof(PenjualanPembayaranView)}", dataParameter);

        await Task.Delay(1000);
        IsBusy = false;

    }

    private void AddNewCommandAction(object obj)
    {
        
        ////var vm = new PenjualanDetailViewModel(null);
        ////var form = new PenjualanDetailView() { BindingContext = vm };
        //Shell.Current.Navigation.PushAsync(form);
    }

    async Task ExecuteLoadItemsCommand()
    {
        try
        {
            IsBusy= true;
            if (!fromdetail)
            {

                if(UserProfile==null)
                {
                    UserProfile = await Account.GetProfile();
                }

                Title = "Penjualan";
                Items.Clear();



                var orders = await penjualanService.GetPenjualans();
                if (orders != null)
                {
                    _SourceItems = new ObservableCollection<PenjualanAndOrderModel>(orders);
                }


                if(await Account.UserInRole("Customer"))
                {
                    Title = "Pembelian";
                }

                foreach (var item in _SourceItems.OrderByDescending(x => x.PenjualanId))
                {
                    Items.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
           await MessageHelper.ErrorAsync(ex.Message);
        }
        finally
        {
            IsBusy = false;
            fromdetail = false;
        }
    }
    public void OnAppearing()
    {
        IsBusy = true;
    }

    public Task Search(string textSearch)
    {
        if (_SourceItems == null)
            return Task.CompletedTask;

        if (string.IsNullOrEmpty(textSearch))
        {
            Items.Clear();
            Items = new ObservableCollection<PenjualanAndOrderModel>(_SourceItems.ToList());
            //foreach (var item in _SourceItems)
            //{
            //    Items.Add(item);
            //}
        }
        else if(textSearch.Length>3)
        {
            Items.Clear();
            var data = textSearch.ToLower();
            foreach (var item in _SourceItems.Where(x => x.Invoice.ToLower().Contains(data)
            || x.Customer.ToLower().Contains(data)
            || x.PaymentStatus.ToString().ToLower().Contains(data)
            || x.Sales.ToLower().Contains(data)))
            {
                Items.Add(item);
            }
        }
        return Task.CompletedTask;
    }
}