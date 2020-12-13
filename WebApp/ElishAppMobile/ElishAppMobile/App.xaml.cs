using ElishAppMobile.Services;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ShareModels;
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

            MessagingCenter.Subscribe<MessageDataCenter, string>(this, "message", async (sender, data) => {

                await MainPage.DisplayAlert(sender.Title, sender.Message, sender.Cancel ?? "Close");
            });

            MessagingCenter.Subscribe<MessageDataCenter, string>(this, "dialog", async (sender, data) => {
                await MainPage.DisplayAlert(sender.Title, sender.Message, sender.Ok ?? "Ok", sender.Cancel ?? "Cancel");
            });


            if (Account.UserIsLogin)
            {
                Task.Run( async () => {

                    if(await Account.UserInRole("Administrator"))
                    {
                        MainPage = new AppShell();
                    }else if(await Account.UserInRole("Sales"))
                    {
                        MainPage = new SalesShell();
                    }
                    else
                    {
                        MainPage = new AppShell();
                    }
                });
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
