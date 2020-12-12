using ElishAppMobile.Services;
using ShareModels;
using Xamarin.Forms;

namespace ElishAppMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<SignalrClient>();
            DependencyService.Register<ICategoryService, CategoryService>();
            DependencyService.Register<ISupplierService, SupplierService>();
            DependencyService.Register<ICustomerService, CustomerService>();
            DependencyService.Register<IProductService, ProductService>();
            DependencyService.Register<IPembelianService, PembelianService>();
            DependencyService.Register<IIncommingService, IncomingCheckService>();
            DependencyService.Register<IPenjualanService, PenjualanService>();

            MessagingCenter.Subscribe<MessageDialogData, string>(this, "message", async (sender, data)=> {
                await MainPage.DisplayAlert(sender.Title, sender.Message, sender.Ok);
            });

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
