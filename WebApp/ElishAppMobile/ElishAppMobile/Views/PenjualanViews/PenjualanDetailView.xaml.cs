using ElishAppMobile.Helpers;
using ElishAppMobile.Models;
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
    }


    public class PenjualanDetailViewModel : BaseViewModel
    {
        public PenjualanDetailViewModel(PenjualanAndOrderModel penjualanModel)
        {
            OrderDetailCommand = new Command(async () => await ShowDetail());
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            _penjualanModel = penjualanModel;
            Title = "Detail Penjualan";
            _ = ExecuteLoadItemsCommand();
        }

        private async Task ShowDetail()
        {
            var order = await PenjualanService.GetOrder(_penjualanModel.OrderId);
            var form = new SalesOrderView();
            form.BindingContext = new SalesOrderViewModel(_penjualanModel);
            await  Shell.Current.Navigation.PushModalAsync(form);
        }

        private Penjualan penjualan;

        public Command OrderDetailCommand { get; }
        public Command LoadItemsCommand { get; }

        private PenjualanAndOrderModel _penjualanModel;

        public Penjualan Penjualan
        {
            get => penjualan;
            set => SetProperty(ref penjualan, value);
        }

        public bool AddCustomerVisible => !Account.UserInRole("Customer").Result;


        public ObservableCollection<ItemPenjualanModel> Items { get; set; } = new ObservableCollection<ItemPenjualanModel>();
        private async Task ExecuteLoadItemsCommand()
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;
                Penjualan = await PenjualanService.GetPenjualan(_penjualanModel.PenjualanId);
                IsBusy = false;
            }
            finally 
            {
                IsBusy = false;
            }
        }
    }
}