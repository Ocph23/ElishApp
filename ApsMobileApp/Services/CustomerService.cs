using ApsMobileApp.Models;
using ApsMobileApp.Services;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ApsMobileApp.Services
{
  
    public class CustomerService : ICustomerService
    {
        private readonly string controller = "/api/customer";
        private ElishDbStore localDb;

        public ObservableCollection<Customer> CustomerCollection { get; set; } = new ObservableCollection<Customer>();

        public CustomerService(ElishDbStore _localDb)
        {
            localDb = _localDb;
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                using var res = new RestService();
                var response = await res.DeleteAsync($"{controller}/{id}");
                if (!response.IsSuccessStatusCode)
                    throw new SystemException(await res.Error(response));
                return await response.GetResult<bool>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Customer> Get(int id)
        {
            try
            {
                if (CustomerCollection.Any())
                    return CustomerCollection.Where(x => x.Id == id).FirstOrDefault();

                using var res = new RestService();
                var response = await res.GetAsync($"{controller}/{id}");
                if (!response.IsSuccessStatusCode)
                    throw new SystemException(await res.Error(response));
                return await response.GetResult<Customer>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<Customer>> Get()
        {
            try
            {
                if (!CustomerCollection.Any())
                {
                    var connection = Helper.CheckInterNetConnection();
                    if (connection.Item1)
                    {
                        using var res = new RestService();
                        var response = await res.GetAsync($"{controller}");
                        if (!response.IsSuccessStatusCode)
                            throw new SystemException(await res.Error(response));
                        var results = await response.GetResult<IEnumerable<Customer>>();
                        CustomerCollection.Clear();
                        foreach (var item in results)
                        {
                            CustomerCollection.Add(item);
                        }

                        _ = localDb.Save<SqlDataModelCustomer, Customer>(results);

                    }
                    else
                    {
                        var datas = await localDb.GetAsync<SqlDataModelCustomer, Customer>();
                        CustomerCollection.Clear();
                        foreach (var item in datas)
                        {
                            CustomerCollection.Add(item);
                        }
                    }
                }
                return CustomerCollection.ToList();



            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<Customer>> GetBySales(int id)
        {
            try
            {
                if (!CustomerCollection.Any())
                {
                    var connection = Helper.CheckInterNetConnection();
                    var db = DependencyService.Get<ElishDbStore>();
                    if (connection.Item1)
                    {
                        using var res = new RestService();
                        var response = await res.GetAsync($"{controller}/BySales/{id}");
                        if (!response.IsSuccessStatusCode)
                            throw new SystemException(await res.Error(response));
                        var results = await response.GetResult<IEnumerable<Customer>>();
                        CustomerCollection.Clear();
                        foreach (var item in results)
                        {
                            CustomerCollection.Add(item);
                        }

                        _ = db.Save<SqlDataModelCustomer, Customer>(results);

                    }
                    else
                    {
                        var datas = await db.GetAsync<SqlDataModelCustomer, Customer>();
                        CustomerCollection.Clear();
                        foreach (var item in datas)
                        {
                            CustomerCollection.Add(item);
                        }
                    }
                }
                return CustomerCollection.ToList();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Customer> Post(Customer value)
        {

            try
            {
                using var res = new RestService();
                var response = await res.PostAsync($"{controller}", res.GenerateHttpContent(value));
                if (!response.IsSuccessStatusCode)
                    throw new SystemException(await res.Error(response));
                var data= await response.GetResult<Customer>();
                if (data != null)
                {
                    CustomerCollection.Add(data);
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> Update(int id, Customer value)
        {
            try
            {
                using var res = new RestService();
                var response = await res.PutAsync($"{controller}/{id}", res.GenerateHttpContent(value));
                if (!response.IsSuccessStatusCode)
                    throw new SystemException(await res.Error(response));
                return await response.GetResult<bool>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> UpdateLocation(Customer cust)
        {
            try
            {
                using var res = new RestService();
                var response = await res.PutAsync($"{controller}/location/{cust.Id}", res.GenerateHttpContent(cust));
                if (!response.IsSuccessStatusCode)
                    throw new SystemException(await res.Error(response));
                return await response.GetResult<bool>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
    }
}
