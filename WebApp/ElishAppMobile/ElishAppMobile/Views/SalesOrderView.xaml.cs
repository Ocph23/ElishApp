﻿using ElishAppMobile.Helpers;
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
        public SalesOrderViewModel(Orderpenjualan order)
        {
           
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SaveCommand = new Command(SaveAction, CanSaved);
            ClearCommand = new Command(()=>{
                Datas.Clear();
                SelectedIndex = -1;
                Title = "Create Order";
                Order = new Orderpenjualan { OrderDate = DateTime.Now };
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

            if (order == null)
            {
                Order = new Orderpenjualan { SalesId = 1, OrderDate = DateTime.Now, Items = new List<OrderPenjualanItem>() };
                Title = "Create Order";
                InitAsync(null);
            }
            else
            {
                Order = order;
                Title = "View/Edit Order";
                InitAsync(order);
              
            }


          
            
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
        private IEnumerable<ProductStock> products;
        #endregion
        

        #region Properties
        private Orderpenjualan order;
        private int _selectedIndex=-1;

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
                    Order.Customer = DataCustomers[value];
                    if (Order.Customer != null)
                    {
                        Order.CustomerId = Order.Customer.Id;
                    }
                }
                SetProperty(ref _selectedIndex, value);
            }
        }

        #endregion

        #region Methods
        private bool CanSaved(object arg)
        {
            if(Order!=null && Datas.Count>0 && Order.CustomerId>0 && Order.Status== OrderStatus.New)
                return true;
            return false;
        }

        private void Datas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
           
            Total = Datas.Sum(x => x.Total);
        }

        private async void InitAsync(Orderpenjualan order)
        {
            try
            {
                IsBusy = true;
                var customers = await Customers.Get();
                foreach (var item in customers)
                {
                    DataCustomers.Add(item);
                }
                products = await Products.GetProductStock();
                foreach (var item in products.Where(x => x.Stock > 0))
                {
                    ProductStocks.Add(item);
                }

                if (order != null)
                {
                    var customer = DataCustomers.Where(x => x.Id == order.CustomerId).FirstOrDefault();
                    SelectedIndex = DataCustomers.IndexOf(customer);
                    foreach (var item in order.Items)
                    {
                        var stock = ProductStocks.ToList().Where(x => x.Id == item.ProductId).FirstOrDefault();
                        if (stock != null)
                        {

                            var newData = new ItemPenjualanModel()
                            {
                                Amount = stock == null ? 0 : stock.StockView,
                                Units = new ObservableCollection<Unit>(stock.Units),
                                Product = stock,
                                ProductId = item.ProductId,
                                Unit = stock.Units.Where(x => x.Id == item.UnitId).FirstOrDefault(),
                                UnitId = item.UnitId,
                                Real = item.Amount
                            };
                            Datas.Add(newData);

                        }
                    }
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
                    Order.Items.Add(new OrderPenjualanItem {  Amount=item.Real, Price= item.Unit.Sell, UnitId=item.UnitId, Unit=item.Unit, 
                        ProductId=item.ProductId, Product=item.Product });
                }

                var profile = Account.GetProfile();
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
                    Datas.Clear();
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

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            await Task.Delay(100);
            RefreshProductStock();
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
            if(products!=null)
            {
                var productFilter = products.Where(item => item.Stock > 0 && !Datas.Any(data => data.ProductId.Equals(item.Id)));
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