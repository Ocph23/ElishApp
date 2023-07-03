using ApsMobileApp.Services;
using BarcodeScanner.Mobile;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace ApsMobileApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ApsAppService()
            .UseMauiMaps()
            .UseMauiCommunityToolkit()
            .ConfigureMauiHandlers(handler => {
                handler.AddBarcodeScannerHandler();
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        
       


#if DEBUG
            builder.Logging.AddDebug();
#endif

        var app= builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            var localdb = scope.ServiceProvider.GetService<ElishDbStore>();
           // localdb.InitializeAsync();
        }
        return app;
    }
}


