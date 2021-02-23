using ElishAppMobile.ViewModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views.SalesmanView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeSalesman : ContentPage
    {
        public HomeSalesman()
        {
            InitializeComponent();
        }
    }

    public class HomeSalesmanViewModel:BaseViewModel
    {

        private List<PenjualanAndOrderModel> _penjualanSource = new List<PenjualanAndOrderModel>();
        public ObservableCollection<HomeSalesModel> PeriodePenjualan { get; set; }
        public ObservableCollection<PenjualanAndOrderModel> JatuhTempo { get; set; } = new ObservableCollection<PenjualanAndOrderModel>();
        public ObservableCollection<PenjualanAndOrderModel> Orders { get; set; } = new ObservableCollection<PenjualanAndOrderModel>();
        public Command LoadItemsCommand { get; }

        public HomeSalesmanViewModel()
        {
            PeriodePenjualan = new ObservableCollection<HomeSalesModel>();
            LoadItemsCommand = new Command( async() => await LoadAsync());
            LoadItemsCommand.Execute(null);
        }

        private async Task LoadAsync()
        {
            try
            {
                if (IsBusy)
                    return;
                var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var profile = await Account.GetProfile();
                SalesName = profile.Name;
                var penjualanSource = await PenjualanService.GetPenjualans();
                foreach (var item in penjualanSource)
                {
                    var data = (PenjualanAndOrderModel)item;
                    if (data != null)
                    {
                        _penjualanSource.Add(data);
                        if(data.PaymentStatus != ShareModels.PaymentStatus.PaidOff &&  data.Created.AddDays(data.DeadLine) < now)
                        {
                            JatuhTempo.Add(data);
                        }
                    }
                }

                PeriodePenjualan.Add(new HomeSalesModel { Name = "Capaian Bulan Ini", Total = _penjualanSource.Where(x => x.Created >= now && x.Created <= now.AddMonths(1).AddDays(-1)).Sum(x => x.Total) });
                var bulanlalu = now.AddMonths(-1);
                PeriodePenjualan.Add(new HomeSalesModel { Name = "Capaian Bulan Lalu", Total = _penjualanSource.Where(x => x.Created >= bulanlalu && x.Created <= bulanlalu.AddMonths(1).AddDays(-1)).Sum(x => x.Total) });

                var orders = await PenjualanService.GetOrders();

                foreach (var item in orders.Where(x=>x.OrderStatus == ShareModels.OrderStatus.New))
                {
                    Orders.Add(item);
                }

                if (Orders.Count <= 0)
                {
                    Orders.Add(new PenjualanAndOrderModel());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }



        private string salesName;

        public string SalesName
        {
            get { return salesName; }
            set { SetProperty(ref salesName , value); }
        }

    }


    public class HomeSalesModel
    {
        public string Name { get; set; }
        public double Total{ get; set; }
    }

}