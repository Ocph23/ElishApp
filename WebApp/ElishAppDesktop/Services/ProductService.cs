using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElishAppDesktop
{
    public class ProductService : IProductService
    {
        readonly string controller = "/api/product";
        private IEnumerable<Product> products;

        public async Task<Product> AddProduct(int supplierId, Product product)
        {
            try
            {
                using var res = new RestService();
                var response = await res.PostAsync($"{controller}/addproduct/{supplierId}", res.GenerateHttpContent(product));
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<Product>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Unit> AddUnit(int productId, Unit unit)
        {
            try
            {
                using RestService res = new RestService();
                var response = await res.PostAsync($"{controller}/addunit/{productId}", res.GenerateHttpContent(unit));
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<Unit>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

      

        public async Task<IEnumerable<Product>> GetProductsBySupplier(int id)
        {
            try
            {
                using var res = new RestService();
                var response = await res.GetAsync($"{controller}/GetProductsBySupplier/{id}");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<IEnumerable<Product>>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<ProductStock>> GetProductStock()
        {
            try
            {
                using var res = new RestService();
                var response = await res.GetAsync($"{controller}/stock");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<IEnumerable<ProductStock>>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Unit> UpdateUnit(int unitId, Unit unit)
        {
            try
            {
                using var res = new RestService();
                var response = await res.PutAsync($"{controller}/UpdateUnit/{unitId}", res.GenerateHttpContent(unit));
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<Unit>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


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

        public async Task<Product> Get(int id)
        {
            try
            {
                using var res = new RestService();
                var response = await res.GetAsync($"{controller}/{id}");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<Product>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<Product>> Get()
        {
            if (products != null && products.Count() > 0)
                return products;
            try
            {
                using var res = new RestService();
                var response = await res.GetAsync($"{controller}");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                products = await response.GetResult<IEnumerable<Product>>();

                return products;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Product> Post(Product value)
        {

            try
            {
                using var res = new RestService();
                var response = await res.PostAsync($"{controller}", res.GenerateHttpContent(value));
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<Product>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> Update(int id, Product value)
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
