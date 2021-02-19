using ElishAppMobile.Services;
using FFImageLoading;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ShareModels;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ElishAppMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
           
            AppCenter.Start("android=b91e5f5c-614e-4fed-b3c2-eee28fd06464",
                  typeof(Analytics), typeof(Crashes));

            DependencyService.Register<SignalrClient>();
            DependencyService.Register<ICategoryService, CategoryService>();
            DependencyService.Register<ISupplierService, SupplierService>();
            DependencyService.Register<ICustomerService, CustomerService>();
            DependencyService.Register<IProductService, ProductService>();
            DependencyService.Register<IPembelianService, PembelianService>();
            DependencyService.Register<IIncommingService, IncomingCheckService>();
            DependencyService.Register<IPenjualanService, PenjualanService>();
            DependencyService.Register<IUserStateService, UserService>();
            DependencyService.Register<ElishDbStore>();
           

            Load();
        }

        private async void Load()
        {

            var config = new FFImageLoading.Config.Configuration()
            {
                ExecuteCallbacksOnUIThread = true,
                 HttpClient= new System.Net.Http.HttpClient(RestService.GetHandler())
            };

            ImageService.Instance.Initialize(config);
            MessagingCenter.Subscribe<MessageDataCenter>(this, "message", async (sender) => {
                await MainPage.DisplayAlert(sender.Title, sender.Message, sender.Cancel = "Close");
            });


            MainPage = new Views.LoginPage();

            if (Account.UserIsLogin)
            {
                if (await Account.UserInRole("Administrator"))
                {
                    MainPage = new AppShell();
                }
                else if (await Account.UserInRole("Sales"))
                {
                    MainPage = new SalesShell();
                }
                else
                {
                    MainPage = new CustomerShell();
                }
            }
            else
            {
                MainPage = new Views.LoginPage();
            }
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
