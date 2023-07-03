using ApsMobileApp.Helpers;
using ApsMobileApp.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace ApsMobileApp.Views;

public partial class ProductView : ContentPage
{
    readonly ProductViewViewModel _viewModel;

    public ProductView(ProductViewViewModel viewmodel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewmodel;
        search.OnSearchFound += Search_OnSearchFound;
    }

    private void Search_OnSearchFound(object data)
    {
        _viewModel.Search(data.ToString());
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}

public class ProductViewViewModel : BaseViewModel
{
    private ProductStock _selectedItem;

    public ObservableCollection<Merk> DataMerk { get; } = new();
    public ObservableCollection<ProductStock> Items { get; } = new();

    private ObservableCollection<ProductStock> _SourceItems = new();

    public Command LoadItemsCommand { get; }
    public Command AddItemCommand { get; }
    public Command ScanBarcode { get; }
    public Command SearchScanCommand { get; }
    public Command<ProductStock> ItemTapped { get; }
    public bool IsNotCustomer => !Account.UserInRole("Customer").Result;
    public ProductViewViewModel(IProductService productService)
    {
        Title = "Products";
        ItemTapped = new Command<ProductStock>(OnItemSelected);
        ScanBarcode = new Command(ScanAction);
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        SearchScanCommand = new Command(SearchScanCommandAction);
        LoadItemsCommand.Execute(null);
        ProductService = productService;
    }

    private async void SearchScanCommandAction(object obj)
    {
        var vm = new InputBarcodeViewModel();
        vm.AutoCount = true;
        vm.ShowAutoCount = false;
        var form = new InputBarcodeView() { BindingContext = vm };
        vm.OnResultScanHandler += async (dynamic result) =>
        {

            if (result != null && result.Article != null)
            {
                var products = await DependencyService.Get<IProductService>().GetProductStock();
                if (products != null)
                {
                    var data = products.Where(x => x.CodeArticle == result.Article).FirstOrDefault();
                    if (data != null)
                    {
                        FromDetail = true;
                        var navigationParameter = new Dictionary<string, object>
                                        {
                                            { "Product", data }
                                        };
                        await Shell.Current.GoToAsync($"//product/{nameof(ProductDetailView)}", navigationParameter);
                        return;
                    }
                }
            }

            await Toas.ShowLong($"{result.Article} Not Found !");
        };

        await Shell.Current.Navigation.PushModalAsync(form);
    }

    private async void ScanAction(object obj)
    {
        try
        {
            var item = (Product)obj;
            var barcodeScan = new BarcodeScannerView();
            var vmScanBarcode = new BarcodeScannerViewModel();
            barcodeScan.BindingContext = vmScanBarcode;
            vmScanBarcode.OnResultScanHandler += async (x) =>
            {
                if (item != null)
                {
                    if (_SourceItems.Where(prod => !string.IsNullOrEmpty(prod.CodeArticle) && prod.CodeArticle == x.ToString()).Count() > 0)
                    {
                        MessagingCenter.Send<MessageDataCenter>(new MessageDataCenter
                        {
                            Title = "Error",
                            Message = $"Data {x} Sudah Ada",
                            Ok = "Ya"
                        }, "message");
                    }
                    else
                    {
                        item.CodeArticle = x.ToString();
                        var aa = await ProductService.Update(item.Id, item);
                    }
                }
            };
            await Shell.Current.Navigation.PushModalAsync(barcodeScan);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    async Task ExecuteLoadItemsCommand()
    {
        try
        {

            if (!FromDetail)
            {
                //var suppliers = await Suppliers.GetSuppliers();
                DataMerk.Clear();
                //foreach (var supplier in suppliers)
                //{
                //    DataMerk.Add(supplier);
                //}
                Items.Clear();
                var dataSource = await ProductService.GetProductStock();
                _SourceItems = new ObservableCollection<ProductStock>(dataSource.OrderByDescending(x => x.Stock).ToList());

                var gg = _SourceItems.GroupBy(x => x.Merk.Name);
                foreach (var item in gg)
                {
                    DataMerk.Add(item.FirstOrDefault().Merk);
                }

                foreach (var item in _SourceItems.Where(x => x.Merk == Merk))
                {
                    Items.Add(item);
                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
            FromDetail = false;
        }
    }

    public void OnAppearing()
    {
        IsBusy = true;
        SelectedItem = null;
    }

    public ProductStock SelectedItem
    {
        get => _selectedItem;
        set
        {
            SetProperty(ref _selectedItem, value);
            OnItemSelected(value);
        }
    }


    async void OnItemSelected(ProductStock item)
    {
        if (item == null)
            return;
        else
        {
            FromDetail = true;

            var navigationParameter = new Dictionary<string, object>
                                        {
                                            { "Product", item }
                                        };

            await Shell.Current.GoToAsync($"//product/{nameof(ProductDetailView)}", navigationParameter);
        }
    }

    internal Task Search(string textSearch)
    {
        Items.Clear();
        if (string.IsNullOrEmpty(textSearch))
        {
            foreach (var item in _SourceItems.Where(x => x.Merk == Merk))
            {
                Items.Add(item);
            }
        }
        else
        {
            var data = textSearch.ToLower();
            foreach (var item in _SourceItems.Where(x => x.Merk == Merk && x.CodeName.ToLower().Contains(data) ||
                 x.Name.ToLower().Contains(data)).AsEnumerable())
            {
                Items.Add(item);
            }
        }
        return Task.CompletedTask;
    }



    private Merk merk;

    public Merk Merk
    {
        get { return merk; }
        set
        {
            SetProperty(ref merk, value);
            if (value != null)
            {
                _ = SetDataByMerk(value);
            }
        }
    }

    private async Task SetDataByMerk(Merk value)
    {
        try
        {
            await Task.Delay(200);
            Items.Clear();
            var datas = _SourceItems.Where(x => x.Merk.Id == value.Id).AsEnumerable();
            foreach (var item in datas)
            {
                Items.Add(item);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public bool FromDetail { get; private set; }
    public IProductService ProductService { get; }
}