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

        public ObservableCollection<ProductStock> Items { get; }

        private ObservableCollection<ProductStock> _SourceItems;

        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command ScanBarcode { get; }
        public Command<ProductStock> ItemTapped { get; }

        public ProductViewViewModel()
        {
            Title = "Products";
            Items = new ObservableCollection<ProductStock>();
            _SourceItems = new ObservableCollection<ProductStock>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<ProductStock>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
            ScanBarcode = new Command(ScanAction);
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
                            MessagingCenter.Send<MessageDialogData>(new MessageDialogData
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

        async Task ExecuteLoadItemsCommand()
        {
            try
            {
                Items.Clear();
                _SourceItems = new ObservableCollection<ProductStock>(await Products.GetProductStock());
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

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(ProductStock item)
        {
            if (item == null)
                return;
            else
            {
                var form = new ProductDetailView() { BindingContext=new ProductDetailViewModel(item)};
                await Shell.Current.Navigation.PushAsync(form);
            }
        }

        internal Task Search(string textSearch)
        {
            Items.Clear();
            if (string.IsNullOrEmpty(textSearch))
            {
                foreach (var item in _SourceItems)
                {
                    Items.Add(item);
                }
            }
            else
            {
                var data = textSearch.ToLower();
                foreach (var item in _SourceItems.Where(x => x.CodeName.ToLower().Contains(data) ||
                     x.Name.ToLower().Contains(data)).AsEnumerable())
                {
                    Items.Add(item);
                }
            }
            return Task.CompletedTask;
        }
    }
}