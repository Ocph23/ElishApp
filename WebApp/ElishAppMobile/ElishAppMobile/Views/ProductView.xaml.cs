using ElishAppMobile.Helpers;
using ElishAppMobile.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ElishAppMobile.Views
{
    public partial class ProductView : ContentPage
    {
        readonly ProductViewViewModel _viewModel;

        public ProductView()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ProductViewViewModel();
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

        public ObservableCollection<Supplier> DataSuppliers { get; }
        public ObservableCollection<ProductStock> Items { get; }

        private ObservableCollection<ProductStock> _SourceItems;

        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command ScanBarcode { get; }
        public Command SearchScanCommand { get; }
        public Command<ProductStock> ItemTapped { get; }

        public ProductViewViewModel()
        {
            Title = "Products";
            Items = new ObservableCollection<ProductStock>();
            DataSuppliers = new ObservableCollection<Supplier>();
            _SourceItems = new ObservableCollection<ProductStock>();
            ItemTapped = new Command<ProductStock>(OnItemSelected);
            ScanBarcode = new Command(ScanAction);
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SearchScanCommand = new Command(SearchScanCommandAction);
        }

        private async void SearchScanCommandAction(object obj)
        {
            var vm = new InputBarcodeViewModel();
            vm.AutoCount = true;
            vm.ShowAutoCount = false;
            var form = new InputBarcodeView() { BindingContext = vm };
            vm.OnResultScanHandler += async (dynamic result) => {

                if (result != null && result.Article != null)
                {
                    var products = await DependencyService.Get<IProductService>().GetProductStock();
                    if (products != null)
                    {
                        var data = products.Where(x => x.CodeArticle == result.Article).FirstOrDefault();
                        if (data != null)
                        {
                            var detailForm = new ProductDetailView() { BindingContext = new ProductDetailViewModel(data) };
                            await Shell.Current.Navigation.PushAsync(detailForm);
                            await Shell.Current.Navigation.PopModalAsync();
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
                var barcodeScan = new BarcodeScanner();
                var vmScanBarcode = new BarcodeScannerViewModel();
                barcodeScan.BindingContext = vmScanBarcode;
                vmScanBarcode.OnResultScanHandler += async (x) => {
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
                            var aa = await Products.Update(item.Id, item);
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

        async  Task ExecuteLoadItemsCommand()
        {
            try
            {

                if (!FromDetail)
                {
                    var suppliers = await Suppliers.GetSuppliers();
                    DataSuppliers.Clear();
                    foreach (var supplier in suppliers)
                    {
                        DataSuppliers.Add(supplier);
                    }
                    Items.Clear();
                    _SourceItems = new ObservableCollection<ProductStock>(await Products.GetProductStock());
                    Supplier = DataSuppliers.FirstOrDefault();
                    foreach (var item in _SourceItems.Where(x => x.SupplierId == Supplier.Id))
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
                var form = new ProductDetailView() { BindingContext=new ProductDetailViewModel(item)};
                await Shell.Current.Navigation.PushAsync(form);
            }
        }

        internal Task Search(string textSearch)
        {
            Items.Clear();
            if (string.IsNullOrEmpty(textSearch))
            {
                foreach (var item in _SourceItems.Where(x=>x.SupplierId==Supplier.Id))
                {
                    Items.Add(item);
                }
            }
            else
            {
                var data = textSearch.ToLower();
                foreach (var item in _SourceItems.Where(x => x.SupplierId == Supplier.Id && x.CodeName.ToLower().Contains(data) ||
                     x.Name.ToLower().Contains(data)).AsEnumerable())
                {
                    Items.Add(item);
                }
            }
            return Task.CompletedTask;
        }


        private int supplierIndex;

        public int SupplierIndex
        {
            get { return supplierIndex; }
            set { 
                if(value>=0)
                {
                    SetProperty(ref supplierIndex, value);
                  
                }
            }
        }

        private Supplier supplier;

        public Supplier Supplier
        {
            get { return supplier; }
            set { SetProperty(ref supplier , value);
                if (value != null)
                {
                    Items.Clear();
                    foreach (var item in _SourceItems.Where(x => x.SupplierId == Supplier.Id))
                    {
                        Items.Add(item);
                    }
                }
            }
        }

        public bool FromDetail { get; private set; }
    }
}