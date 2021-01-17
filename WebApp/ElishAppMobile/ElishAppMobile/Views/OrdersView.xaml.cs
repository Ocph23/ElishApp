using ElishAppMobile.Models;
using ElishAppMobile.ViewModels;
using ShareModels;
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
        private ObservableCollection<Orderpenjualan> _SourceItems;

        public Command LoadItemsCommand { get; }
        public Command AddNewCommand { get; }
        public Command SelectCommand { get; }
        public Command PackingListCommand { get; }
        public Profile Sales { get; }
        public ObservableCollection<Orderpenjualan> Items { get; private set; } = new ObservableCollection<Orderpenjualan>();

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
            var order = (Orderpenjualan)obj;
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
                Items.Clear();
               if( await Account.UserInRole("Administrator"))
                {
                    var orders = await PenjualanService.GetOrders();
                    if (orders != null)
                    {
                        _SourceItems = new ObservableCollection<Orderpenjualan>(orders);
                    }
                }
                else
                {
                    var orderSales = await PenjualanService.GetOrdersBySalesId(Sales.Id);
                    if (orderSales != null)
                    {
                        _SourceItems = new ObservableCollection<Orderpenjualan>(orderSales);
                    }
                }

                foreach (var item in _SourceItems.OrderByDescending(x => x.Id))
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
                foreach (var item in _SourceItems.Where(x => x.Customer.Name.ToLower().Contains(data) ||
                     x.Nomor.ToLower().Contains(data)).AsEnumerable())
                {
                    Items.Add(item);
                }
            }
            return Task.CompletedTask;
        }


       

    }
}