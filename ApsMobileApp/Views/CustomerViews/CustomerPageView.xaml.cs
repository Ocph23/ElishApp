using ApsMobileApp.Models;
using ApsMobileApp.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ApsMobileApp.Views.CustomerViews;

public partial class CustomerPageView : ContentPage
{
    private CustomerPageViewModel _viewModel;

    public CustomerPageView(CustomerPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
        //search.OnSearchFound += Search_OnSearchFound;
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


public class CustomerPageViewModel : BaseViewModel
{
    private IEnumerable<Customer> _SourceItems;
    private ICustomerService _customerService;
    public Command LoadItemsCommand { get; }
    public Command AddNewCommand { get; }
    public Command SelectCommand { get; }
    public Profile Sales { get; }
    public ObservableCollection<Customer> Items { get; set; } = new();

    public CustomerPageViewModel(ICustomerService customerService)
    {
        _customerService = customerService;
        LoadItemsCommand = new Command(async(x)=> await ExecuteLoadItemsCommand(x));
        AddNewCommand = new Command(AddNewCommandAction);
        SelectCommand = new Command(SelectCommandAction);
        LoadItemsCommand.Execute(true);

    }


    private void SelectCommandAction(object obj)
    {
        var cust = obj as Customer;
        var data = new Dictionary<string, object>() {
            { "Customer", cust}
        };

        Shell.Current.GoToAsync($"//{nameof(CustomerPageView)}/{nameof(CustomerProfileView)}",data);
    }

    private void AddNewCommandAction(object obj)
    {
        //var vm = new SalesOrderViewModel(null);
        //var form = new Views.SalesOrderView() { BindingContext = vm };
        //Shell.Current.Navigation.PushAsync(form);
    }

    async Task ExecuteLoadItemsCommand(object obj)
    {
        try
        {
            if(IsBusy) return;
            IsBusy= true;
            Items.Clear();
            var orders = await _customerService.Get();
            var profile = await Account.GetProfile();
            if (orders != null)
            {
                if (await Account.UserInRole("Sales"))
                {
                    _SourceItems = new ObservableCollection<Customer>(orders.Where(x => x.Karyawan.Id == profile.Id));
                }
                else
                {
                    _SourceItems = new ObservableCollection<Customer>(orders);
                }
            }

            foreach (var item in _SourceItems)
            {
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
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
            foreach (var item in _SourceItems.Where(x => x.Name.ToLower().Contains(data) ||
                 x.ContactName.ToLower().Contains(data)).AsEnumerable())
            {
                Items.Add(item);
            }
        }
        return Task.CompletedTask;
    }
}