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
            BindingContext = new SalesOrderViewModel();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            productPicker.Focus();
        }
    }


    public class SalesOrderViewModel : BaseViewModel
    {
        #region Constructor
        public SalesOrderViewModel()
        {
            Order = new Orderpenjualan { SalesId = 1, OrderDate = DateTime.Now, Items = new List<OrderPenjualanItem>() };
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SaveCommand = new Command(SaveAction, CanSaved);

            QRCommand = new Command(async () => {
                var vm = new InputBarcodeViewModel();
                vm.OnResultScanHandler += Vm_OnResultScanHandler;
                var form = new InputBarcodeView() { BindingContext = vm };

                await Shell.Current.Navigation.PushModalAsync(form);

            });
            AddCommand = new Command(async () => await ExecuteLoadItemsCommand());

            Datas.CollectionChanged += Datas_CollectionChanged;
            InitAsync();
        }
        #endregion

        #region Fields 
        private double total;
        private Command _saveCommand;
        private Customer customerSelected;
        private ProductStock productSelect;
        private IEnumerable<ProductStock> products;
        #endregion
        

        #region Properties
        private Orderpenjualan order;
        public Orderpenjualan Order
        {
            get => order;
            set => SetProperty(ref order, value);
        }
        public ObservableCollection<ItemPenjualanModel> Datas { get; set; } = new ObservableCollection<ItemPenjualanModel>();
        public ObservableCollection<Customer> DataCustomers { get; set; } = new ObservableCollection<Customer>();
        public ObservableCollection<ProductStock> ProductStocks { get; set; } = new ObservableCollection<ProductStock>();
        public Command LoadItemsCommand { get; }
        public Command SaveCommand { get => _saveCommand; set => SetProperty(ref _saveCommand, value); }
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
        public Customer CustomerSelected
        {
            get { return customerSelected; }
            set
            {
                SetProperty(ref customerSelected, value);
                if (value != null)
                {
                    Order.Customer = value;
                    Order.CustomerId = value.Id;
                }
            }
        }
        #endregion
        
        #region Methods
        private bool CanSaved(object arg)
        {
            if(Order!=null && Datas.Count>0 && Order.CustomerId>0)
                return true;
            return false;
        }

        private void Datas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Total = Datas.Sum(x => x.Unit.Sell * x.Real);
        }

        private async void InitAsync()
        {
            var customers = await Customers.Get();
            foreach (var item in customers)
            {
                DataCustomers.Add(item);
            }

            products = await Products.GetProductStock();
            foreach (var item in products.Where(x=>x.Stock>0))
            {
                ProductStocks.Add(item);
            }
        }

        private async void SaveAction(object obj)
        {
            try
            {
                Order.Items.Clear();
                foreach (var item in Datas)
                {
                    Order.Items.Add(new OrderPenjualanItem { Amount=item.Real, Price= item.Unit.Sell, UnitId=item.UnitId, Unit=item.Unit, ProductId=item.ProductId, Product=item.Product });
                }
                var result = await PenjualanService.CreateOrder(Order);
                if(result!=null)
                {
                    Order = new Orderpenjualan { SalesId = 1, OrderDate = DateTime.Now, Items = new List<OrderPenjualanItem>() };
                    CustomerSelected = null;
                    Datas.Clear();
                }

            }
            catch (Exception ex)
            {
                await Toas.ShowLong($"Error : {ex.Message} !");
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
                        data.Real += max < 1 ? max : 1 ;
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

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            await Task.Delay(100);
            IsBusy = false;
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
                Real = value.Stock < 1 ? value.Stock:1 
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
            SaveCommand = new Command(SaveAction, CanSaved);
            await Task.Delay(1000);
            ProductStocks.Clear();
            var productFilter = products.Where(item =>item.Stock >0 && !Datas.Any(data => data.ProductId.Equals(item.Id)));
            foreach (var item in productFilter)
            {
                ProductStocks.Add(item);
            }
        }


        #endregion

    }
}