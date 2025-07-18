﻿using ApsWebApp.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Radzen;
using ShareModels;

namespace ApsWebApp.Services
{
    public static class AppServiceCollection
    {
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {

            services.AddScoped<HttpClient>();
            services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
    .AddIdentityCookies();
            //services.AddScoped<ApplicationDbContext>();
            services.AddSingleton<IIncommingService, IncommingService>();
            services.AddScoped<IUserStateService, UserStateService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPembelianService, PembelianService>();
            services.AddScoped<IPenjualanService, PenjualanService>();
            services.AddScoped<IGudangService, GudangService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IKaryawanService, KaryawanService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IMerkService, MerkService>();


            services.AddScoped<IPengembalianPenjualanService, PengembalianPenjualanService>();
            services.AddScoped<IPemindahanService, PemindahanService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();

            return services;
        }

    }
}
