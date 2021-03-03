using ElishAppMobile.Models;
using ElishAppMobile.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrdesrView : ContentPage
    {
        private OrdesrViewModel _viewModel;

        public OrdesrView()
        {
            InitializeComponent();
            BindingContext= _viewModel = new OrdesrViewModel();
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

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var order = (Orderpenjualan)ItemsListView.SelectedItem;
            var vm = new CreatePackingListViewModel(order);
            var form = new CreatePackingList() { BindingContext = vm };
            await Shell.Current.Navigation.PushAsync(form);
        }
    }


    public class OrdesrViewModel : BaseViewModel
    {
        private ObservableCollection<PenjualanAndOrderModel> _SourceItems;
        private bool fromdetail;

        public Command LoadItemsCommand { get; }
        public Command AddNewCommand { get; }
        public Command SelectCommand { get; }
        public Command PackingListCommand { get; }
        public Profile Sales { get; }
        public ObservableCollection<PenjualanAndOrderModel> Items { get; private set; } = new ObservableCollection<PenjualanAndOrderModel>();

        public OrdesrViewModel()
        {
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddNewCommand = new Command(AddNewCommandAction);
            SelectCommand = new Command(SelectCommandAction);
            PackingListCommand = new Command(PackingListAction);
            Sales = Account.GetProfile().Result;
        }

        private async void PackingListAction(object obj)
        {
            var order = (Orderpenjualan)obj;
            var vm = new CreatePackingListViewModel(order);
            var form = new CreatePackingList() { BindingContext = vm };
            await   Shell.Current.Navigation.PushAsync(form);
        }

        private void SelectCommandAction(object obj)
        {
            fromdetail = true;
            var order = (PenjualanAndOrderModel)obj;
            var vm = new SalesOrderViewModel(order);
            var form = new Views.SalesOrderView() { BindingContext = vm };
            Shell.Current.Navigation.PushAsync(form);
        }

        private void AddNewCommandAction(object obj)
        {
            var vm = new SalesOrderViewModel(null);
            var form = new Views.SalesOrderView() { BindingContext = vm };
            Shell.Current.Navigation.PushAsync(form);
        }

        async Task ExecuteLoadItemsCommand()
        {
            try
            {
                if (!fromdetail)
                {
                    Items.Clear();
                    var orders = await PenjualanService.GetOrders();
                    if (orders != null)
                    {
                        _SourceItems = new ObservableCollection<PenjualanAndOrderModel>(orders);
                    }

                    foreach (var item in _SourceItems.Where(x=>x.OrderStatus == OrderStatus.Baru).OrderByDescending(x => x.OrderId))
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
                foreach (var item in _SourceItems)
                {
                    Items.Add(item);
                }
            }
            else
            {
                Items.Clear();
                var data = textSearch.ToLower();
                foreach (var item in _SourceItems.Where(x => x.Customer.ToLower().Contains(data) ||
                     x.NomorSO.ToLower().Contains(data)).AsEnumerable())
                {
                    Items.Add(item);
                }
            }
            return Task.CompletedTask;
        }
    }
}