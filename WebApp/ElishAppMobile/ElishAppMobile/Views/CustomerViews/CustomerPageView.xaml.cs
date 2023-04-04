using ElishAppMobile.Models;
using ElishAppMobile.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views.CustomerViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerPageView : ContentPage
    {
        private CustomerPageViewModel _viewModel;

        public CustomerPageView()
        {
            InitializeComponent();
            BindingContext= _viewModel = new CustomerPageViewModel();
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

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                var number = (Label)sender;
                PhoneDialer.Open(number.Text);
            }
            catch (Exception ex)
            {
                MessageHelper.ErrorAsync(ex.Message).Wait();
            }
        }
    }


    public class CustomerPageViewModel : BaseViewModel
    {
        private ObservableCollection<Customer> _SourceItems;
        private bool fromdetail;
        public Command LoadItemsCommand { get; }
        public Command AddNewCommand { get; }
        public Command SelectCommand { get; }
        public Profile Sales { get; }
        public ObservableCollection<Customer> Items { get; private set; } = new ObservableCollection<Customer>();

        public CustomerPageViewModel()
        {
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddNewCommand = new Command(AddNewCommandAction);
            SelectCommand = new Command(SelectCommandAction);
            Sales = Account.GetProfile().Result;
        }


        private void SelectCommandAction(object obj)
        {
            fromdetail = true;
            var cust = (Customer)obj;
            var form = new CustomerProfileView(cust);
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
                    var orders = await Customers.Get();
                    var profile = await Account.GetProfile();
                    if (orders != null)
                    {
                        if (await Account.UserInRole("Sales"))
                        {
                            _SourceItems = new ObservableCollection<Customer>(orders.Where(x=>x.Karyawan.Id==profile.Id));
                        }
                        else
                        {
                            _SourceItems = new ObservableCollection<Customer>(orders);
                        }
                    }

                    foreach (var item in _SourceItems)
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
                foreach (var item in _SourceItems.Where(x => x.Name.ToLower().Contains(data) ||
                     x.ContactName.ToLower().Contains(data)).AsEnumerable())
                {
                    Items.Add(item);
                }
            }
            return Task.CompletedTask;
        }
    }
}