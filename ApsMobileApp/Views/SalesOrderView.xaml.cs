using ApsMobileApp.Helpers;
using ApsMobileApp.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System.Collections.ObjectModel;

namespace ApsMobileApp.Views;

[QueryProperty(nameof(Model), "Model")]
public partial class SalesOrderView : ContentPage
{

    private PenjualanAndOrderModel model;

    public PenjualanAndOrderModel Model
    {
        get { return model; }
        set
        {
            model = value;
            OnPropertyChanged(nameof(Model));
            var vm = BindingContext as SalesOrderViewModel;
            _ = vm.InitAsync(Model);
        }
    }


    public SalesOrderView(SalesOrderViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {

        if(!productPicker.IsFocused)
            productPicker.Focus();
        else
        {
            productPicker.Unfocus();
            productPicker.Focus();
        }
    }

    private void ImageButton_Clicked_1(object sender, EventArgs e)
    {
        // var form = new CreateCustomer();
        Shell.Current.GoToAsync("customer/create");
    }
}


public class SalesOrderViewModel : BaseViewModel
{
    #region Constructor
    public SalesOrderViewModel(ICustomerService _customerService,   
        IPenjualanService _penjualanService ,
        IProductService _productService, IKaryawanService _karyawanService)
    {
        CustomerService = _customerService;
        penjualanService = _penjualanService;
        productService = _productService;
        karyawanService = _karyawanService;
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        SaveCommand = new Command(SaveAction, CanSaved);
        Order = new OrderPenjualan { OrderDate = DateTime.Now, DeadLine = 12 };
        ClearCommand = new Command(() =>
        {
            Datas.Clear();
            CustomerSelected = null;
            Title = "Create Order";
            Order = new OrderPenjualan { OrderDate = DateTime.Now , DeadLine=12};
          _=  RefreshProductStock();
        });
        QRCommand = new Command( () =>
        {
            var vm = new InputBarcodeViewModel();
            vm.OnResultScanHandler += Vm_OnResultScanHandler;
            var form = new InputBarcodeView() { BindingContext = vm };
            Shell.Current.Navigation.PushModalAsync(form);

        });
        AddCommand = new Command(async () => await ExecuteLoadItemsCommand());
        Datas.CollectionChanged += Datas_CollectionChanged;
        DeleteCommand = new Command(DeleteAction);
        this.PropertyChanged += SalesOrderViewModel_PropertyChanged;
        Title = "Create Order";
       _= Load();

    }

    private async Task Load()
    {
        salessource = await karyawanService.GetSales();

        foreach (var item in salessource.Where(x=>x.IsSales))
        {
            SalesSource.Add(item);
        }

        customersource = await CustomerService.Get();
        products = await productService.GetProductStock();
        foreach (var p in products.GroupBy(x => x.Supplier.Id))
        {
            var prod = p.FirstOrDefault();
            DataSupplier.Add(prod.Supplier);
        }
        foreach (var item in customersource)
        {
            CustomerSource.Add(item);
        }
    }

    private void SalesOrderViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        SaveCommand.ChangeCanExecute();
    }

    private void DeleteAction(object obj)
    {
        var item = (ItemPenjualanModel)obj;
        if (item != null)
        {
            Datas.Remove(item);
            RefreshProductStock();
        }

    }
    #endregion

    #region Fields 
    private double total;
    private Command _saveCommand;
    private ProductStock productSelect;
    private IEnumerable<Karyawan> salessource;
    private IEnumerable<Customer> customersource;
    private IEnumerable<ProductStock> products;
    #endregion

    #region Properties
    private OrderPenjualan _order;
    private int _selectedIndex = -1;
    private int _supplierIndex;

    public OrderPenjualan Order
    {
        get => _order;
        set => SetProperty(ref _order, value);
    }

    private string paymentType;

    public string PaymentType
    {
        get => paymentType;
        set
        {
            SetProperty(ref paymentType, value);
        }
    }

    public bool AddCustomerVisible => !Account.UserInRole("Customer").Result;

    public ObservableCollection<Customer> CustomerSource { get; set; } = new ObservableCollection<Customer>();
    public ObservableCollection<ItemPenjualanModel> Datas { get; set; } = new ObservableCollection<ItemPenjualanModel>();
    //  public ObservableCollection<Customer> DataCustomers { get; set; } = new ObservableCollection<Customer>();
    public ObservableCollection<Supplier> DataSupplier { get; set; } = new ObservableCollection<Supplier>();
    public ObservableCollection<ProductStock> ProductStocks { get; set; } = new ObservableCollection<ProductStock>();

    public ObservableCollection<Karyawan> SalesSource { get; set; } = new ObservableCollection<Karyawan>();

    private PenjualanAndOrderModel _orderParameter;
    private readonly IPenjualanService penjualanService;
    private readonly IProductService productService;
    private readonly IKaryawanService karyawanService;

    public Command LoadItemsCommand { get; }
    public Command SaveCommand { get => _saveCommand; set => SetProperty(ref _saveCommand, value); }
    public Command ClearCommand { get; }
    public Command DeleteCommand { get; set; }
    public Command QRCommand { get; }
    public Command AddCommand { get; }
    public ProductStock ProductSelect
    {
        get { return productSelect; }
        set
        {
            SetProperty(ref productSelect, value);
            if (value != null)
            {
                var data = Datas.Where(x => x.Product.Id == value.Id).FirstOrDefault();
                if (data == null)
                {
                    AddNewItem(value);

                }
            }
        }
    }
    public double Total
    {
        get { return total; }
        set { SetProperty(ref total, value); }
    }


    private Customer customerSelected;

    public Customer CustomerSelected
    {
        get { return customerSelected; }
        set { SetProperty(ref customerSelected , value);
            if (value != null)
            {
                Order.Customer = value;
                if (Account.UserInRole("Sales").Result)
                {
                    var profile = Account.GetProfile().Result;
                    Order.Sales = new Karyawan { Id=profile.Id, Name=profile.Name };
                }else if (Account.UserInRole("Administrator").Result || Account.UserInRole("Customer").Result)
                {
                    Order.Sales = value.Karyawan;
                }   
            }
        }
    }


    private Karyawan salesSelected;

    public Karyawan SalesSelected
    {
        get { return salesSelected; }
        set { SetProperty(ref salesSelected , value); }
    }



    private Supplier supplierSelected;

    public Supplier SupplierSelected
    {
        get { return supplierSelected; }
        set { SetProperty(ref supplierSelected, value);
           _= RefreshProductStock();
        }
    }

    public ICustomerService CustomerService { get; }


    #endregion

    #region Methods
    private bool CanSaved(object arg)
    {
        if (Order != null && Datas.Count > 0 && Order.Sales != null && Order.Customer !=null && Order.Status == OrderStatus.Baru)
            return true;
        return false;
    }

    private void Datas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {

        Total = Datas.Sum(x => x.Total);
    }

    public async Task InitAsync(PenjualanAndOrderModel vParam)
    {
        try
        {
            IsBusy = true;
            await Task.Delay(3000);
            if (vParam == null)
            {
                var profile = await Account.GetProfile();
                Order = new OrderPenjualan { OrderDate = DateTime.Now, Items = new List<OrderPenjualanItem>() };
                Order.DeadLine = 12;
                Title = "Create Order";
            }
            else
            {
                Order = await penjualanService.GetOrder(vParam.OrderId);
                Title = "View/Edit Order";
                SalesSelected = SalesSource.FirstOrDefault(x => x.Id== vParam.SalesId);
            }

            if (Order != null && vParam != null)
            {
                CustomerSelected = CustomerSource.Where(x => x.Id == Order.Customer.Id).FirstOrDefault();
                var ll = Order.Items.FirstOrDefault();
                SupplierSelected = DataSupplier.SingleOrDefault(x => x.Id == ll.Product.Supplier.Id);
                foreach (var item in Order.Items)
                {
                    var stock = products.ToList().Where(x => x.Id == item.Product.Id).FirstOrDefault();
                    if (stock != null)
                    {
                        //stock.Pembelian += item.Amount;
                        var newData = new ItemPenjualanModel()
                        {
                            Id = item.Id,
                            Amount = stock == null ? 0 : stock.Stock,
                            Units = new ObservableCollection<Unit>(stock.Units),
                            Product = stock,
                            //ProductId = item.ProductId,
                            Unit = item.Unit,
                            //UnitId = item.UnitId,
                        };
                        newData.Real = item.Quantity;
                        Datas.Add(newData);
                    }

                }
            }
            else
            {
                var profile = await Account.GetProfile();
                foreach (var item in customersource.Where(x => x.Karyawan.Id == profile.Id))
                {
                    CustomerSource.Add(item);
                }
            }

            foreach (var item in products.Where(x => x.Stock > 0))
            {
                ProductStocks.Add(item);
            }

            Datas.Reverse();
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(ex.Message);
            await Shell.Current.Navigation.PopAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CustomerCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void SaveAction(object obj)
    {
        try
        {
            string message = Order.Id <= 0 ? "Yakin Buat Order ?" : "Yakin Ubah Data ?";

            if (!await MessageHelper.DialogAsync("Dialog", message))
                return;

            Order.Items.Clear();
            foreach (var item in Datas)
            {
                Order.Items.Add(new OrderPenjualanItem { Id = item.Id, Unit = item.Unit, Product = item.Product });
            }

            OrderPenjualan result;

            if (Order.Id <= 0)
            {
                result = await penjualanService.CreateOrder(Order);
                Order.Id = result.Id;
            }
            else
                result = await penjualanService.UpdateOrder(Order.Id, Order);


            if (result != null)
            {
                await Toas.ShowLong($"Success : !");
                if (_orderParameter != null)
                {
                    _orderParameter.Total = Datas.Sum(x => x.Total);
                }

                Datas.Clear();
                await Shell.Current.Navigation.PopAsync();
            }

        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync($"Error : {ex.Message} !");
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
                    await Toas.ShowLong($"Sisa Stock {data.Product.Name}, Amount : {max}");
                    return;
                }

                if (result.Type == "Auto")
                {
                    data.Real += max < 0.5 ? max : 0.5;
                    await Toas.ShowLong($"{data.Product.Name} , Amount : {data.Real}");
                }
                else
                {
                    if ((double)result.Count > max)
                        await Toas.ShowLong($"Sorry, Stock {data.Product.Name} tidak cukup , Amount : {data.Real}");
                    else
                    {
                        data.Real += (double)result.Count;
                        await Toas.ShowLong($"{data.Product.Name} , Amount : {data.Real}");
                        return;
                    }

                    if (result.Count != null)
                        await Toas.ShowLong($"Sisa Stock {data.Product.Name}, Amount : {data.Amount - data.Real}");
                }
            }
            else
            {
                var product = ProductStocks.Where(x => x.CodeArticle == article).FirstOrDefault();
                if (product != null)
                {
                    AddNewItem(product);
                    return;
                }
                await Toas.ShowLong($"Error : {result.Article.ToString()} Not Found !");
            }
        }
    }

    private async Task ExecuteLoadItemsCommand()
    {
        await RefreshProductStock();
    }

    private async void AddNewItem(ProductStock value)
    {

        var unit = value.Units.First();
        var newData = new ItemPenjualanModel()
        {

            Amount = value.Stock,
            Units = new ObservableCollection<Unit>(value.Units),
            Product = value,
            // ProductId = value.Id,
            Unit = unit,
            UnitId = unit.Id,
            Real = value.Stock < 1 ? value.Stock : 1
        };

        newData.UpdateEvent += NewData_UpdateEvent;
        Datas.Add(newData);
        ProductSelect = null;
        await RefreshProductStock();
        await Toas.ShowLong($"{newData.Product.Name} , Amount : {newData.Real}");
    }

    private Task NewData_UpdateEvent(ItemPenjualanModel arg)
    {
        Datas_CollectionChanged(arg, null);
        return Task.CompletedTask;
    }

    private async Task RefreshProductStock()
    {
        try
        {
            await Task.Delay(2000);
            ProductStocks.Clear();
            if (products != null && SupplierSelected != null)
            {
                var productFilter = products.Where(item => item.Supplier.Id == SupplierSelected.Id && item.Stock > 0 && !Datas.Any(data => data.Product.Id.Equals(item.Id)));
                foreach (var item in productFilter.OrderBy(x => x.Name))
                {
                    ProductStocks.Add(item);
                }
                SaveCommand = new Command(SaveAction, CanSaved);
            }
        }
        catch (Exception ex)
        {
            await MessageShow.ErrorAsync(ex.Message);
        }
        finally { IsBusy = false; }
    }
    #endregion

}