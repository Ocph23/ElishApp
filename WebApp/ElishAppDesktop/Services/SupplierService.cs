using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElishAppDesktop
{
    public class SupplierService : ISupplierService
    {
        private readonly string controller = "/api/supplier";

        public async Task<bool> Delete(int id)
        {
            try
            {
                using var res = new RestService();
                var response = await res.DeleteAsync($"{controller}/{id}");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<bool>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Supplier> GetSupplier(int id)
        {
            try
            {
                using var res = new RestService();
                var response = await res.GetAsync($"{controller}/{id}");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<Supplier>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<Supplier>> GetSuppliers()
        {
            try
            {
                using var res = new RestService();
                var response = await res.GetAsync($"{controller}");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<IEnumerable<Supplier>>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<Product>> GetProducts(int supplierId)
        {
            try
            {
                using var res = new RestService();
                var response = await res.GetAsync($"{controller}/GetProducts");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<IEnumerable<Product>>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }



        public async Task<Supplier> Post(Supplier value)
        {

            try
            {
                using var res = new RestService();
                var response = await res.PostAsync($"{controller}", res.GenerateHttpContent(value));
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<Supplier>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> Update(int id, Supplier value)
        {
            try
            {
                using var res = new RestService();
                var response = await res.PutAsync($"{controller}/{id}", res.GenerateHttpContent(value));
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<bool>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
    }
}
