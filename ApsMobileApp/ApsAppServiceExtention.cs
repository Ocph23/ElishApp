using ApsMobileApp.Services;
using ApsMobileApp.Views;
using ApsMobileApp.Views.CustomerViews;
using ApsMobileApp.Views.PenjualanViews;
using ApsMobileApp.Views.SalesmanView;
using ShareModels;
using CommunityToolkit.Maui;
using ApsMobileApp.Views.PembelianViews;

namespace ApsMobileApp;

public static class ApsAppServiceExtention
{
    public static MauiAppBuilder ApsAppService(this MauiAppBuilder builder)
    {
        //service
        builder.Services.AddSingleton<ElishDbStore>();
        builder.Services.AddSingleton<SignalrClient>();
        builder.Services.AddSingleton<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<IKaryawanService, KaryawanService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IPenjualanService, PenjualanService>();
        builder.Services.AddScoped<IPembelianService, PembelianService>();
        builder.Services.AddScoped<ISupplierService, SupplierService>();
        builder.Services.AddScoped<IIncommingService, IncomingCheckService>();
        builder.Services.AddScoped<IUserStateService, UserService>();



        //viewmodel
        builder.Services.AddTransient<CustomerPageViewModel>();
        builder.Services.AddTransient<CustomerProfileViewModel>();
        builder.Services.AddTransient<ProductViewViewModel>();
        builder.Services.AddTransient<ProductDetailViewModel>();
        builder.Services.AddTransient<OrdesrViewModel>();
        builder.Services.AddTransient<SalesOrderViewModel>();
        builder.Services.AddTransient<PenjualanViewModel>();
        builder.Services.AddTransient<PenjualanDetailViewModel>();
        builder.Services.AddTransient<InputBarcodeViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<MapPageViewModel>();
        builder.Services.AddTransient<HomeSalesmanViewModel>();
        builder.Services.AddTransient<PenjualanPembayaranViewModel>();
        builder.Services.AddTransient<PembayaranDialogViewModel>();
        builder.Services.AddTransient<PembelianViewModel>();
        builder.Services.AddTransient<PembelianDetailViewModel>();
        builder.Services.AddTransient<PembelianPembayaranViewModel>();
        builder.Services.AddTransient<PembelianPembayaranDialogViewModel>();



        //views
        builder.Services.AddTransient<PembelianDetailView>();
        builder.Services.AddTransient<PembelianView>();
        builder.Services.AddTransient<PembelianPembayaranView>();
        builder.Services.AddTransient<PembelianPembayaranDialogView>();
        builder.Services.AddTransient<PembayaranDialogView>();
        builder.Services.AddTransient<PenjualanPembayaranView>();
        builder.Services.AddTransient<SalesOrderView>();
        builder.Services.AddTransient<CustomerPageView>();
        builder.Services.AddTransient<ProductView>();
        builder.Services.AddTransient<ProductDetailView>();
        builder.Services.AddTransient<OrdesrView>();
        builder.Services.AddTransient<PenjualanView>();
        builder.Services.AddTransient<PenjualanDetailView>();
        builder.Services.AddTransient<InputBarcodeView>();
        builder.Services.AddTransient<HomeSalesman>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MapPageView>();
        builder.Services.AddTransient<CustomerProfileView>();

        return builder;
    }
}

