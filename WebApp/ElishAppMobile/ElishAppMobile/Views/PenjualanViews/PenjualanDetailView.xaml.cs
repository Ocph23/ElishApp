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

namespace ElishAppMobile.Views.PenjualanViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PenjualanDetailView : ContentPage
    {

        public PenjualanDetailView()
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


    public class PenjualanDetailViewModel : BaseViewModel
    {
        #region Constructor
        public PenjualanDetailViewModel(Penjualan order)
        {
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            Order = order;
            Title = "Detail Penjualan";
        }

        private void PenjualanDetailViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
        private Penjualan order;
        private int _selectedIndex=-1;
        private int _supplierIndex;

        public Penjualan Order
        {
            get => order;
            set => SetProperty(ref order, value);
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


        public ObservableCollection<ItemPenjualanModel> Datas { get; set; } = new ObservableCollection<ItemPenjualanModel>();
      //  public ObservableCollection<Customer> DataCustomers { get; set; } = new ObservableCollection<Customer>();
        public ObservableCollection<Supplier> DataSupplier { get; set; } = new ObservableCollection<Supplier>();
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
                SetProperty(ref _selectedIndex, value);
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
            return false;
        }

        private void Datas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
           
            Total = Datas.Sum(x => x.Total);
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
            }
        }


        #endregion

    }
}