using Microsoft.Extensions.DependencyInjection;
using Ocph.DAL.DbContext;
using Ocph.DAL.Provider.MySql;
using System;
using System.Data;

namespace Ocph.DAL
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddOcphService(this IServiceCollection services)
        {
            services.AddScoped<IDbConnection, MySqlDbConnection>();
            services.AddScoped<IDataTables, MySqlDbContexs>();
            return services;
        }
    }




    public static class ServiceLocator
    {
        public static IServiceProvider Instance { get; set; }
    }
}
