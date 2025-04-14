using FluentValidation;
using Microsoft.AspNetCore.Identity;
using ShareModels;

namespace ApsWebApp.Validations
{
    public static class AppValidatorServices
    {
        public static IServiceCollection AddValidatorServices(this IServiceCollection services)
        {
            services.AddScoped<IValidator<Category>, CategoryValidator>();
            services.AddScoped<IValidator<Merk>, MerkValidator>();
            services.AddScoped<IValidator<Supplier>, SupplierValidator>();
            services.AddScoped<IValidator<Gudang>, GudangValidator>();
            services.AddScoped<IValidator<Karyawan>, KaryawanValidator>();
            services.AddScoped<IValidator<Product>, ProductValidator>();
            services.AddScoped<IValidator<Unit>, UnitValidator>();

            return services;
        }

    }
}
