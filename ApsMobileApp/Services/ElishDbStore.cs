using ApsMobileApp.Models;
using ShareModels;
using ShareModels.ModelViews;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApsMobileApp.Services
{
    public class ElishDbStore
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(SQLiteDatabase.DatabasePath, SQLiteDatabase.Flags);
        });

        SQLiteAsyncConnection Database => lazyInitializer.Value;

        static bool initialized = false;

        public ElishDbStore()
        {
            InitializeAsync();//.SafeFireAndForget(false);
        }

        private void DeleteDatabase()
        {
            Database.DropTableAsync<SqlDataModelCategory>();
            Database.DropTableAsync<SqlDataModelCustomer>();
            Database.DropTableAsync<SqlDataModelStock>();
            Database.DropTableAsync<SqlDataModelSupplier>();
            Database.DropTableAsync<SqlDataModelOrder>();
            Database.DropTableAsync<SqlDataModelPenjualan>();
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                try
                {
                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(SqlDataModelStock).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(SqlDataModelStock)).ConfigureAwait(false);
                    }

                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(SqlDataModelSupplier).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(SqlDataModelSupplier)).ConfigureAwait(false);
                    }

                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(SqlDataModelCategory).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(SqlDataModelCategory)).ConfigureAwait(false);
                    }


                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(SqlDataModelCustomer).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(SqlDataModelCustomer)).ConfigureAwait(false);
                    }

                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(SqlDataModelOrder).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(SqlDataModelOrder)).ConfigureAwait(false);
                    }

                    if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(SqlDataModelPenjualan).Name))
                    {
                        await Database.CreateTablesAsync(CreateFlags.None, typeof(SqlDataModelPenjualan)).ConfigureAwait(false);
                    }
                    initialized = true;
                }
                catch (Exception ex)
                {

                    throw new SystemException(ex.Message);
                }

            }
        }

        public async Task Save<T, N>(IEnumerable<N> datas) where T : class
        {
            try
            {
                await Database.DeleteAllAsync<T>();
                var list = new List<T>();
                foreach (var item in datas)
                {
                    var model = (ISqlDataModel)Activator.CreateInstance(typeof(T));
                    model.Id = ((IEntity)item).Id;
                    model.Data = JsonSerializer.Serialize(item, Helper.JsonOption);
                    list.Add(model as T);
                }

                if (list.Count > 0)
                    await Database.InsertAllAsync(list);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        private object GenerateModel<N>(IEnumerable<N> datas)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<N>> GetAsync<T, N>()
        {
            var tname = typeof(T).Name;
            var data = await Database.QueryAsync<SqlDataModel>($"Select * From {tname};");
            if (data != null)
                return data.Select(x => JsonSerializer.Deserialize<N>(x.Data, Helper.JsonOption));
            return default;
        }

        public async Task<N> GetAsync<T, N>(int id)
        {
            var tname = typeof(T).Name;
            var data = await Database.QueryAsync<SqlDataModel>($"Select * From {tname} where id={id} limit 1;");
            if (data != null)
                return data.Select(x => JsonSerializer.Deserialize<N>(x.Data, Helper.JsonOption)).FirstOrDefault();
            return default;
        }
    }

    public static class TaskExtensions
    {
        // NOTE: Async void is intentional here. This provides a way
        // to call an async method from the constructor while
        // communicating intent to fire and forget, and allow
        // handling of exceptions
        public static async void SafeFireAndForget(this Task task,
            bool returnToCallingContext,
            Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(returnToCallingContext);
            }

            // if the provided action is not null, catch and
            // pass the thrown exception
            catch (Exception ex) when (onException != null)
            {
                onException(ex);
            }
        }
    }
}
