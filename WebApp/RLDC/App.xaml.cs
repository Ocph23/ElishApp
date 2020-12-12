using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocph.DAL;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace RLDC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();


        }

        private void ConfigureServices(ServiceCollection services)
        {

            // build config
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();
            services.AddSingleton(configuration);
            services.AddOcphService();
            services.AddSingleton<MainWindow>();
            services.AddDbContext<OcphDbContext>(options =>
                options.UseMySQL(configuration.GetConnectionString("DefaultConnection")));
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await Task.Delay(1000);
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
