using Ocph.DAL.Provider.MySql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Ocph.DAL.DbContext
{
   public class DataContext
    {
        public static IDataTable<T> GetDatatable<T>(IDbConnection connection) where T : class
        {
            return new MySqlDbContext<T>(connection);
        }


        public static IDataTables GetContext<T>() where T : class
        {
            return ServiceLocator.Instance.GetRequiredService<IDataTables>();
        }

    }
}
