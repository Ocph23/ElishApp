using ApsMobileApp.Models;
using ApsMobileApp.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;




namespace ApsMobileApp.Views.PembelianViews;

public partial class PembelianView : ContentPage
{
    private PembelianViewModel _viewModel;

    public PembelianView(PembelianViewModel vm)
    {
        InitializeComponent();
        BindingContext= _viewModel = vm;
    }
    private void Search_OnSearchFound(object data)
    {
        _viewModel.Search(data.ToString());
    }
}


public class PembelianViewModel : BaseViewModel
{
    private ObservableCollection<PembelianDataModel> _SourceItems;
    private bool fromdetail;
    private IPembelianService pembelianService;

    public Command LoadItemsCommand { get; }
    public Command SelectCommand { get; }
    public Command PembayaranCommand { get; }
    public Profile UserProfile { get; set; }
    public ObservableCollection<PembelianDataModel> Items { get; private set; } = new ObservableCollection<PembelianDataModel>();

    public PembelianViewModel(IPembelianService _pembelianService)
    {
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        SelectCommand = new Command(async (x)=> await SelectCommandAction(x));
        PembayaranCommand = new Command(async (x)=> await PembayaranCommandAction(x));
        LoadItemsCommand.Execute(null);
        pembelianService = _pembelianService;
    }

    private async Task SelectCommandAction(object obj)
    {
        IsBusy = true;
        fromdetail = true;
        var pembelian = obj as PembelianDataModel;
        //var vm = new PenjualanDetailViewModel(order);

        var dataParameter = new Dictionary<string, object>() { 
             {"PembelianId", pembelian.Id }
        };

       await Shell.Current.GoToAsync($"//{nameof(PembelianView)}/{nameof(PembelianDetailView)}", dataParameter);

        await Task.Delay(1000);
        IsBusy = false;
       
    }

    private async Task PembayaranCommandAction(object obj)
    {
        IsBusy = true;
        fromdetail = true;
        var order = obj as PembelianDataModel;
        var dataParameter = new Dictionary<string, object>() {
             {"Pembelian", order }
        };

        await Shell.Current.GoToAsync($"//{nameof(PembelianView)}/{nameof(PembelianPembayaranView)}",true, dataParameter);

        await Task.Delay(1000);
        IsBusy = false;

    }


   

    async Task ExecuteLoadItemsCommand()
    {
        try
        {
            if (IsBusy)
                return;

            IsBusy= true;
            if (UserProfile == null)
            {
                UserProfile = await Account.GetProfile();
            }

            Items.Clear();
            var orders = await pembelianService.GetPembelians();
            if (orders != null)
            {
                _SourceItems = new ObservableCollection<PembelianDataModel>(orders);
            }

            if (await Account.UserInRole("Customer"))
            {
                Title = "Pembelian";
            }

            foreach (var item in _SourceItems.OrderByDescending(x => x.Id))
            {
                Items.Add(item);
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
            Items = new ObservableCollection<PembelianDataModel>(_SourceItems.ToList());
            //foreach (var item in _SourceItems)
            //{
            //    Items.Add(item);
            //}
        }
        else if(textSearch.Length>3)
        {
            Items.Clear();
            var data = textSearch.ToLower();
            foreach (var item in _SourceItems.Where(x => x.InvoiceNumber.ToLower().Contains(data)
            || x.SupplierName.ToLower().Contains(data)
            || x.Status.ToString().ToLower().Contains(data)
            || x.OrderNumber.ToString().ToLower().Contains(data)
            || x.Nomor.ToLower().Contains(data)))
            {
                Items.Add(item);
            }
        }
        return Task.CompletedTask;
    }
}