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

namespace ElishAppMobile.Views.PenjualanViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PenjualanView : ContentPage
    {
        private PenjualanViewModel _viewModel;

        public PenjualanView()
        {
            InitializeComponent();
            BindingContext= _viewModel = new PenjualanViewModel();
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


    public class PenjualanViewModel : BaseViewModel
    {
        private ObservableCollection<Penjualan> _SourceItems;
        private bool fromdetail;

        public Command LoadItemsCommand { get; }
        public Command AddNewCommand { get; }
        public Command SelectCommand { get; }
        public Profile UserProfile { get; }
        public ObservableCollection<Penjualan> Items { get; private set; } = new ObservableCollection<Penjualan>();

        public PenjualanViewModel()
        {
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddNewCommand = new Command(AddNewCommandAction);
            SelectCommand = new Command(SelectCommandAction);
            UserProfile = Account.GetProfile().Result;
        }

        private void SelectCommandAction(object obj)
        {
            fromdetail = true;
            var order = (Penjualan)obj;
            var vm = new PenjualanDetailViewModel(order);
            var form = new PenjualanDetailView() { BindingContext = vm };
            Shell.Current.Navigation.PushAsync(form);
        }

        private void AddNewCommandAction(object obj)
        {
            var vm = new PenjualanDetailViewModel(null);
            var form = new PenjualanDetailView() { BindingContext = vm };
            Shell.Current.Navigation.PushAsync(form);
        }

        async Task ExecuteLoadItemsCommand()
        {
            try
            {
                if (!fromdetail)
                {
                    Items.Clear();
                    if (await Account.UserInRole("Administrator"))
                    {
                        var orders = await PenjualanService.GetPenjualans();
                        if (orders != null)
                        {
                            _SourceItems = new ObservableCollection<Penjualan>(orders);
                        }
                    }
                    else if(await Account.UserInRole("Sales"))
                    {
                        var orderSales = await PenjualanService.GetPenjualansBySalesId(UserProfile.Id);
                        if (orderSales != null)
                        {
                            _SourceItems = new ObservableCollection<Penjualan>(orderSales);
                        }
                    }
                    else
                    {
                        var orderSales = await PenjualanService.GetPenjualansByCustomerId(UserProfile.Id);
                        if (orderSales != null)
                        {
                            _SourceItems = new ObservableCollection<Penjualan>(orderSales);
                        }
                    }

                    foreach (var item in _SourceItems.OrderByDescending(x => x.Id))
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
            }
            return Task.CompletedTask;
        }
    }
}