using ApsMobileApp.Models;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApsMobileApp.Services
{
    public class ProductService : IProductService
    {
        public ProductService(ElishDbStore _localDb)
        {
            localDb = _localDb;
        }

        readonly string controller = "/api/product";
        private readonly ElishDbStore localDb;
        private IEnumerable<Product> products;

        public async Task<Product> AddProduct(int supplierId, Product product)
        {
            try
            {
                using var res = new RestService();
                var response = await res.PostAsync($"{controller}/addproduct/{supplierId}", res.GenerateHttpContent(product));
                if (!response.IsSuccessStatusCode)
                    throw new SystemException(await res.Error(response));
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
                    throw new SystemException(await res.Error(response));
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
                    throw new SystemException(await res.Error(response));
                return await response.GetResult<IEnumerable<Product>>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


        List<ProductStock> _productStocks;

        public async Task<IEnumerable<ProductStock>> GetProductStock()
        {
            try
            {
                if(_productStocks == null || _productStocks.Count<=0)
                {
                    var connection = Helper.CheckInterNetConnection();

                    if (connection.Item1)
                    {
                        using var res = new RestService();
                        var response = await res.GetAsync($"{controller}/stock");
                        if (!response.IsSuccessStatusCode)
                            throw new SystemException(await res.Error(response));
                        var datas = await response.GetResult<IEnumerable<ProductStock>>();
                        _ = localDb.Save<SqlDataModelStock, ProductStock>(datas);
                        _productStocks= datas.ToList();
                    }
                    else
                    {
                        var datas = await localDb.GetAsync<SqlDataModelStock, ProductStock>();
                        _productStocks= datas.ToList();
                    }
                }
                return _productStocks;
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
                    throw new SystemException(await res.Error(response));
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
                    throw new SystemException(await res.Error(response));
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
                    throw new SystemException(await res.Error(response));
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
                    throw new SystemException(await res.Error(response));
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
                    throw new SystemException(await res.Error(response));
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
                    throw new SystemException(await res.Error(response));
                return await response.GetResult<bool>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<ProductImage> AddPhoto(ProductImage image)
        {
            try
            {
                using var res = new RestService();
                var response = await res.PostAsync($"{controller}/addphoto", res.GenerateHttpContent(image));
                if (!response.IsSuccessStatusCode)
                     throw new SystemException(await res.Error(response));
                return await response.GetResult<ProductImage>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> RemovePhoto(int id)
        {
            try
            {
                using var res = new RestService();
                var response = await res.DeleteAsync($"{controller}/removephoto/{id}");
                if (!response.IsSuccessStatusCode)
                    throw new SystemException(await res.Error(response));
                return await response.GetResult<bool>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> RemoveUnit(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductStock>> GetProductStockByGudangId(int merkId, int gudangid, bool withOrder)
        {
            throw new NotImplementedException();
        }
    }
}
