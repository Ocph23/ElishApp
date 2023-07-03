using ApsMobileApp.Models;
using ApsMobileApp.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ApsMobileApp.Views;

public partial class OrdesrView : ContentPage
{
    private OrdesrViewModel _viewModel;

    public OrdesrView(OrdesrViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
        search.OnSearchFound += Search_OnSearchFound;
    }

    private void Search_OnSearchFound(object data)
    {
        _viewModel.Search(data.ToString());
    }

    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();
    //    _viewModel.OnAppearing();
    //}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        //var order = (OrderPenjualan)ItemsListView.SelectedItem;
        //var vm = new CreatePackingListViewModel(order);
        //var form = new CreatePackingList() { BindingContext = vm };
        //await Shell.Current.Navigation.PushAsync(form);
    }
}


public class OrdesrViewModel : BaseViewModel
{
    private ObservableCollection<PenjualanAndOrderModel> _SourceItems;
    private readonly IPenjualanService penjualanService;
    public Command LoadItemsCommand { get; }
    public Command AddNewCommand { get; }
    public Command SelectCommand { get; }
    public Command PackingListCommand { get; }
    public Profile Sales { get; set; }
    public ObservableCollection<PenjualanAndOrderModel> Items { get; private set; } = new ObservableCollection<PenjualanAndOrderModel>();

    public OrdesrViewModel(IPenjualanService _penjualanService)
    {
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        AddNewCommand = new Command(AddNewCommandAction);
        SelectCommand = new Command(SelectCommandAction);
        PackingListCommand = new Command(PackingListAction);
        penjualanService = _penjualanService;
        LoadItemsCommand.Execute(null);
    }

    private async void PackingListAction(object obj)
    {
        var order = (OrderPenjualan)obj;
        var vm = new CreatePackingListViewModel(order);
        var form = new CreatePackingList() { BindingContext = vm };
        await Shell.Current.Navigation.PushAsync(form);
    }

    private async void SelectCommandAction(object obj)
    {
        IsBusy = true;
        var order = (PenjualanAndOrderModel)obj;
        var navigationParameter = new Dictionary<string, object>
                                        {
                                            { "Model", order }
                                        };
        await Shell.Current.GoToAsync($"//{nameof(OrdesrView)}/{nameof(SalesOrderView)}", navigationParameter);
    }

    private void AddNewCommandAction(object obj)
    {
        Shell.Current.GoToAsync($"//{nameof(OrdesrView)}/{nameof(SalesOrderView)}");
        //var vm = new SalesOrderViewModel(null);
        //var form = new Views.SalesOrderView() { BindingContext = vm };
        //Shell.Current.Navigation.PushAsync(form);
    }

    async Task ExecuteLoadItemsCommand()
    {
        try
        {
            if (await Account.UserInRole("Sales"))
                Sales = await Account.GetProfile();
            Items.Clear();
            var orders = await penjualanService.GetOrders();
            if (orders != null)
            {
                _SourceItems = new ObservableCollection<PenjualanAndOrderModel>(orders.OrderByDescending(x => x.OrderId));
            }

            foreach (var item in _SourceItems.OrderByDescending(x => x.OrderId))
            {
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
           await MessageShow.ErrorAsync(ex.Message);
            Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
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
            foreach (var item in _SourceItems)
            {
                Items.Add(item);
            }
        }
        else
        {
            Items.Clear();
            var data = textSearch.ToLower();
            foreach (var item in _SourceItems.Where(x => x.Customer.ToLower().Contains(data) ||
                 x.NomorSO.ToLower().Contains(data)).AsEnumerable())
            {
                Items.Add(item);
            }
        }
        return Task.CompletedTask;
    }

    public bool CanAddOrder => Account.UserInRole("Administrator").Result ? false : true;
}