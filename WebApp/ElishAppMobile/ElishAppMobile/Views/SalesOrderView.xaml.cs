using ElishAppMobile.Helpers;
using ElishAppMobile.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalesOrderView : ContentPage
    {

        public SalesOrderView()
        {
            InitializeComponent();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            productPicker.Focus();
        }

        private void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            var form = new CreateCustomer();
            Shell.Current.Navigation.PushModalAsync(form);
        }
    }


    public class SalesOrderViewModel : BaseViewModel
    {
        #region Constructor
        public SalesOrderViewModel(PenjualanAndOrderModel order)
        {

            _orderParameter = order;
           
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SaveCommand = new Command(SaveAction, CanSaved);
            ClearCommand = new Command(()=>{
                Datas.Clear();
                SelectedIndex = -1;
                Title = "Create Order";
                Order = new Orderpenjualan { OrderDate = DateTime.Now };
                Order.DeadLine = 12;
                RefreshProductStock();
            });

            QRCommand = new Command(async () => {
                var vm = new InputBarcodeViewModel();
                vm.OnResultScanHandler += Vm_OnResultScanHandler;
                var form = new InputBarcodeView() { BindingContext = vm };
                await Shell.Current.Navigation.PushModalAsync(form);

            });

            AddCommand = new Command(async () => await ExecuteLoadItemsCommand());
            Datas.CollectionChanged += Datas_CollectionChanged;
            DeleteCommand = new Command(DeleteAction);

            this.PropertyChanged += SalesOrderViewModel_PropertyChanged;

            InitAsync(order);

          
            
        }

        private void SalesOrderViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveCommand.ChangeCanExecute();
        }

        private void DeleteAction(object obj)
        {
           var item= (ItemPenjualanModel)obj;
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
       
        private IEnumerable<Customer> customersource;
        private IEnumerable<ProductStock> products;
        #endregion
        

        #region Properties
        private Orderpenjualan _order;
        private int _selectedIndex=-1;
        private int _supplierIndex;

        public Orderpenjualan Order
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

        private PenjualanAndOrderModel _orderParameter;

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
                    var data = Datas.Where(x => x.ProductId == value.Id).FirstOrDefault();
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

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (value >= 0)
                {
                    Order.Customer = CustomerSource.ToList()[value];
                    if (Order.Customer != null)
                    {
                        Order.CustomerId = Order.Customer.Id;
                    }
                }
                SetProperty(ref _selectedIndex, value);
            }
        }

        public int SupplierIndex
        {
            get
            {
                return _supplierIndex;                      
            }
            set
            {
                SetProperty(ref _supplierIndex, value);
                SupplierSelected = DataSupplier[value];
                Datas.Clear();
                RefreshProductStock();
            }
        }


        private Supplier supplierSelected;

        public Supplier SupplierSelected
        {
            get { return supplierSelected; }
            set { SetProperty(ref supplierSelected , value); }
        }


        #endregion

        #region Methods
        private bool CanSaved(object arg)
        {
            if(Order!=null && Datas.Count>0 && Order.CustomerId>0 && Order.Status== OrderStatus.Baru)
                return true;
            return false;
        }

        private void Datas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
           
            Total = Datas.Sum(x => x.Total);
        }

        private async void InitAsync(PenjualanAndOrderModel vParam)
        {
            try
            {
                if (vParam == null)
                {
                    Order = new Orderpenjualan { SalesId = 1, OrderDate = DateTime.Now, Items = new List<OrderPenjualanItem>() };
                    Order.DeadLine = 12;
                    Title = "Create Order";
                }
                else
                {
                    Order = await PenjualanService.GetOrder(vParam.OrderId);
                    Title = "View/Edit Order";
                }

                IsBusy = true;
                customersource = await Customers.Get();

                products = await Products.GetProductStock();

                if (products != null)
                {
                    foreach (var p in products.GroupBy(x => x.SupplierId))
                    {
                        var prod = p.FirstOrDefault();
                       DataSupplier.Add(prod.Supplier) ;
                    }
                    
                }

                if (Order != null && vParam!=null)
                {
                    foreach (var item in customersource)
                    {
                        CustomerSource.Add(item);
                    }

                    var customer = CustomerSource.Where(x=>x.Id == Order.CustomerId).FirstOrDefault();
                    SelectedIndex = CustomerSource.ToList().IndexOf(customer);
                    var ll = Order.Items.FirstOrDefault();
                    SupplierIndex = DataSupplier.IndexOf(DataSupplier.SingleOrDefault(x=>x.Id==ll.Product.SupplierId));
                    foreach (var item in Order.Items)
                    {
                        var stock = products.ToList().Where(x => x.Id == item.ProductId).FirstOrDefault();
                        if (stock != null)
                        {
                            stock.Pembelian += item.Amount;
                            var newData = new ItemPenjualanModel()
                            {
                                Id = item.Id,
                                Amount = stock == null ? 0 : stock.StockView,
                                Units = new ObservableCollection<Unit>(stock.Units),
                                Product = stock,
                                ProductId = item.ProductId,
                                Unit = item.Unit,
                                UnitId = item.UnitId,
                            };
                            newData.Real = item.Amount;
                            Datas.Add(newData);
                        }

                    }
                }
                else
                {
                    var profile = await Account.GetProfile();
                    foreach (var item in customersource.Where(x=>x.KaryawanId==profile.Id))
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
                    Order.Items.Add(new OrderPenjualanItem { Id=item.Id, OrderPenjualanId=Order.Id,  Amount=item.Real, Price= item.Unit.Sell, UnitId=item.UnitId, Unit=item.Unit, 
                        ProductId=item.ProductId, Product=item.Product });
                }

                var profile = await Account.GetProfile();
                if (profile != null)
                    Order.SalesId = profile.Id;


                Orderpenjualan result;

                if (Order.Id <= 0)
                {
                    result = await PenjualanService.CreateOrder(Order);
                    Order.Id = result.Id;
                }
                else
                    result = await PenjualanService.UpdateOrder(Order.Id, Order);


                if(result!=null)
                {
                    await Toas.ShowLong($"Success : !");
                    if(_orderParameter!=null)
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
                    if(max <=0)
                    {
                        await Toas.ShowLong($"Sisa Stock {data.Product.Name}, Amount : {max}");
                        return;
                    }

                    if(result.Type == "Auto")
                    {
                        data.Real += max < 0.5 ? max : 0.5;
                        await Toas.ShowLong($"{data.Product.Name} , Amount : {data.Real}");
                    }
                    else
                    {
                       if((double)result.Count > max)
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

        private Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            RefreshProductStock();
            IsBusy = false;
            return Task.CompletedTask;
        }

        private async void AddNewItem(ProductStock value)
        {

            var unit = value.Units.First();
            var newData = new ItemPenjualanModel()
            { 
                
                Amount = value.StockView,
                Units = new ObservableCollection<Unit>(value.Units),
                Product = value,
                ProductId = value.Id,
                Unit = unit,
                UnitId = unit.Id,
                Real = value.Stock < 0.5 ? value.Stock : 0.5 
            };

            newData.UpdateEvent += NewData_UpdateEvent;
            Datas.Add(newData);
            RefreshProductStock();
            await Toas.ShowLong($"{newData.Product.Name} , Amount : {newData.Real}");
        }

        private Task NewData_UpdateEvent(ItemPenjualanModel arg)
        {
            Datas_CollectionChanged(arg, null);
            return Task.CompletedTask;
        }

        private async void RefreshProductStock()
        {
            await Task.Delay(1000);
            ProductStocks.Clear();
            if(products!=null && SupplierSelected!=null)
            {
                var productFilter = products.Where(item => item.SupplierId==SupplierSelected.Id && item.Stock > 0 && !Datas.Any(data => data.ProductId.Equals(item.Id)));
                foreach (var item in productFilter.OrderBy(x => x.Name))
                {
                    ProductStocks.Add(item);
                }
                SaveCommand = new Command(SaveAction, CanSaved);




            }
        }


        #endregion

    }
}