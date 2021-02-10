using ElishAppMobile.Models;
using Newtonsoft.Json;
using ShareModels;
using ShareModels.ModelViews;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ElishAppMobile.Services
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
         //   DeleteDatabase();
            InitializeAsync().Wait();//.SafeFireAndForget(false);
        }

        private void DeleteDatabase()
        {
            Database.DropTableAsync<SqlDataModelCategory>();
            Database.DropTableAsync<SqlDataModelCustomer>();
            Database.DropTableAsync<SqlDataModelStock>();
            Database.DropTableAsync<SqlDataModelSupplier>();
            Database.DropTableAsync<SqlDataModelOrder>();
        }

        async Task InitializeAsync()
        {
            if (!initialized)
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


                initialized = true;
            }
        }

        public async Task Save<T,N>(IEnumerable<N> datas) where T:class
        {
            try
            {
                await Database.DeleteAllAsync<T>();
                var list = new List<T>();
                foreach (var item in datas)
                {
                    var model = (ISqlDataModel)Activator.CreateInstance(typeof(T));
                    model.Id = ((IEntity)item).Id;
                    model.Data= JsonConvert.SerializeObject(item, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    list.Add(model as T);
                }

                if(list.Count>0)
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

        public async Task<IEnumerable<N>> Get<T,N>() 
        {
            var tname = typeof(T).Name;
            var data = await Database.QueryAsync<SqlDataModel>($"Select * From {tname};");
            if (data != null)
                return data.Select(x => JsonConvert.DeserializeObject<N>(x.Data, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
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
