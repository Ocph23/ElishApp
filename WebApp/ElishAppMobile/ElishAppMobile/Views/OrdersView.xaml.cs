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
    }


    public class OrdesrViewModel : BaseViewModel
    {
        private ObservableCollection<Orderpenjualan> _SourceItems;

        public Command LoadItemsCommand { get; }
        public Command AddNewCommand { get; }
        public Command SelectCommand { get; }
        public Profile Sales { get; }
        public ObservableCollection<Orderpenjualan> Items { get; private set; } = new ObservableCollection<Orderpenjualan>();

        public OrdesrViewModel()
        {
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddNewCommand = new Command(AddNewCommandAction);
            SelectCommand = new Command(SelectCommandAction);
            Sales = Account.GetProfile().Result;
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
                _SourceItems = new ObservableCollection<Orderpenjualan>(await PenjualanService.GetOrdersBySalesId(Sales.Id));
                foreach (var item in _SourceItems.OrderByDescending(x=>x.Id))
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