using ElishAppMobile.Models;
using ElishAppMobile.ViewModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
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
        }

       

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }


    public class PenjualanViewModel : BaseViewModel
    {
        private ObservableCollection<PenjualanAndOrderModel> _SourceItems;
        private bool fromdetail;

        public Command LoadItemsCommand { get; }
        public Command AddNewCommand { get; }
        public Command SelectCommand { get; }
        public Profile UserProfile { get; }
        public ObservableCollection<PenjualanAndOrderModel> Items { get; private set; } = new ObservableCollection<PenjualanAndOrderModel>();

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
            var order = (PenjualanAndOrderModel)obj;
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
                    Title = "Penjualan";
                    Items.Clear();

                    var orders = await PenjualanService.GetPenjualans();
                    if (orders != null)
                    {
                        _SourceItems = new ObservableCollection<PenjualanAndOrderModel>(orders);
                    }


                    if(await Account.UserInRole("Customer"))
                    {
                        Title = "Pembelian";
                    }

                    foreach (var item in _SourceItems.OrderByDescending(x => x.PenjualanId))
                    {
                        Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
               await MessageHelper.ErrorAsync(ex.Message);
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