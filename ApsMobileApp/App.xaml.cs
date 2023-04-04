using ApsMobileApp.Helpers;
using ApsMobileApp.Services;
using ShareModels;

namespace ApsMobileApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new Views.AboutPage();
        _ = Load();
       
    }

    private async Task Load()
    {
        //DependencyService.Register<SignalrClient>();
        //DependencyService.Register<ICategoryService, CategoryService>();
        //DependencyService.Register<ISupplierService, SupplierService>();
        //DependencyService.Register<ICustomerService, CustomerService>();
        //DependencyService.Register<IProductService, ProductService>();
        //DependencyService.Register<IPembelianService, PembelianService>();
        //DependencyService.Register<IIncommingService, IncomingCheckService>();
        //DependencyService.Register<IPenjualanService, PenjualanService>();
        //DependencyService.Register<IUserStateService, UserService>();
        //DependencyService.Register<ElishDbStore>();

        //var config = new FFImageLoading.Config.Configuration()
        //{
        //    ExecuteCallbacksOnUIThread = true,
        //    HttpClient = new System.Net.Http.HttpClient(RestService.GetHandler())
        //};

        //ImageService.Instance.Initialize(config);
        MessagingCenter.Subscribe<MessageDataCenter>(this, "message", async (sender) => {
            await MainPage.DisplayAlert(sender.Title, sender.Message, sender.Cancel = "Close");
        });

        //MainPage = new Views.LoginPage();

        if (Account.UserIsLogin)
        {
            if (await Account.UserInRole("Sales"))
            {
                MainPage = new SalesShell();
                return;
            }

            if (await Account.UserInRole("Customer"))
            {
                MainPage = new SalesShell();
                return;
            }

            MainPage = new AppShell();
        }
        else
        {
            MainPage = new AccountShell();
        }
    }
}