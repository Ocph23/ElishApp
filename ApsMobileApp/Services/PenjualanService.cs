using ApsMobileApp.Models;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApsMobileApp.Services
{
    public class PenjualanService : IPenjualanService
    {
        readonly string controller = "/api/penjualan";
        private readonly ElishDbStore localDb;
        private IEnumerable<PenjualanAndOrderModel> orders;
        public PenjualanService(ElishDbStore _localDb)
        {
            localDb = _localDb;
        }

        public async Task<OrderPenjualan> CreateOrder(OrderPenjualan order)
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();
              
                if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.PostAsync($"{controller}/order", res.GenerateHttpContent(order));
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return await response.GetResult<OrderPenjualan>();
                }
                else
                {
                    var datas = (await localDb.GetAsync<SqlDataModelOrder, OrderPenjualan>()).ToList();
                    if (datas == null)
                        datas = new List<OrderPenjualan>();
                    datas.Add(order);
                    _ = localDb.Save<SqlDataModelOrder, OrderPenjualan>(datas.AsEnumerable());
                    return order;
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<PembayaranPenjualan> CreatePembayaran(int penjualanId, PembayaranPenjualan model, bool forced)
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();

                if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.PostAsync($"{controller}/cretaepembayaran/{penjualanId}", res.GenerateHttpContent(model));
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return await response.GetResult<PembayaranPenjualan>();
                }
                else
                {
                    throw new SystemException("Maaf Anda Tidak Memiliki Koneksi internet !");
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Penjualan> CreatePenjualan(int orderid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePenjualan(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderPenjualan> GetOrder(int id)
        {
            try
            {
                using var res = new RestService();
                var url = $"{controller}/order/{id}";
                var response = await res.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    throw new SystemException(await res.Error(response));
                return await response.GetResult<OrderPenjualan>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<PenjualanAndOrderModel>> GetOrders()
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();
               
                if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}/order");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    var results= await response.GetResult<IEnumerable<PenjualanAndOrderModel>>();
                    foreach (var item in results)
                    {
                        item.Id = item.OrderId;
                    }
                    _ = localDb.Save<SqlDataModelOrder, PenjualanAndOrderModel>(results);
                    return results;
                }
                else
                {
                    var datas = await localDb.GetAsync<SqlDataModelOrder, PenjualanAndOrderModel>();
                    orders = datas;
                    return orders;
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


        private async Task SyncOrders()
        {
            try
            {
               
                var datas = (await localDb.GetAsync<SqlDataModelOrder, OrderPenjualan>()).ToList();
                if (datas != null && datas.Count>0)
                {
                    using var res = new RestService();
                    foreach (OrderPenjualan item in datas.ToList())
                    {
                        var response = await res.PostAsync($"{controller}/order", res.GenerateHttpContent(item));
                        if (!response.IsSuccessStatusCode)
                            throw new SystemException(await res.Error(response));
                        var model= await response.GetResult<OrderPenjualan>();
                        if (model != null)
                        {
                            datas.Remove(item);
                        }
                    }

                   _= localDb.Save<SqlDataModelOrder, OrderPenjualan>(datas.AsEnumerable());
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<PembayaranPenjualan>> GetPembayaran(int penjualanId)
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();

                if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}/pembayaranbypenjualanid/{penjualanId}");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    var results = await response.GetResult<IEnumerable<PembayaranPenjualan>>();
                    return results;
                }
                else
                {
                    throw new SystemException("No Internet Connection !");
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Penjualan> GetPenjualan(int id)
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();
            
                if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}/{id}");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return await response.GetResult<Penjualan>();
                }
                else
                {
                    throw new SystemException("Tidak Ada Koneksi Internet !");
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualans()
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();
              
                if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    var results = await response.GetResult<IEnumerable<PenjualanAndOrderModel>>();
                    foreach (var item in results)
                    {
                        item.Id = item.PenjualanId;
                    }
                    _ = localDb.Save<SqlDataModelPenjualan, PenjualanAndOrderModel>(results);
                    return results;
                }
                else
                {
                    var datas = await localDb.GetAsync<SqlDataModelPenjualan, PenjualanAndOrderModel>();
                    return datas;
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualans(DateTime start, DateTime end)
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();
                             if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}/ByDate/{start}/{end}");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return await response.GetResult<IEnumerable<PenjualanAndOrderModel>>();
                }
                else
                {
                    var datas = await localDb.GetAsync<SqlDataModelOrder, PenjualanAndOrderModel>();
                    return datas;
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        //public async Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansByCustomerId(int id)
        //{
        //    try
        //    {
        //        var connection = Helper.CheckInterNetConnection();
        //        var db = DependencyService.Get<ElishDbStore>();
        //        if (connection.Item1)
        //        {
        //            using var res = new RestService();
        //            var response = await res.GetAsync($"{controller}/ByCustomerId/{id}");
        //            if (!response.IsSuccessStatusCode)
        //                throw new SystemException(await res.Error(response));
        //            return await response.GetResult<IEnumerable<PenjualanAndOrderModel>>();
        //        }
        //        else
        //        {
        //            var datas = await db.Get<SqlDataModelOrder, PenjualanAndOrderModel>();
        //            return datas;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new SystemException(ex.Message);
        //    }
        //}

        //public async Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansBySalesId(int id)
        //{
        //    try
        //    {
        //        var connection = Helper.CheckInterNetConnection();
        //        var db = DependencyService.Get<ElishDbStore>();
        //        if (connection.Item1)
        //        {
        //            using var res = new RestService();
        //            var response = await res.GetAsync($"{controller}/BySalesId/{id}");
        //            if (!response.IsSuccessStatusCode)
        //                throw new SystemException(await res.Error(response));
        //            return await response.GetResult<IEnumerable<PenjualanAndOrderModel>>();
        //        }
        //        else
        //        {
        //            var datas = await db.Get<SqlDataModelOrder, PenjualanAndOrderModel>();
        //            return datas;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new SystemException(ex.Message);
        //    }
        //}

        public async Task<OrderPenjualan> UpdateOrder(int orderId, OrderPenjualan order)
        {
            try
            {
                using var res = new RestService();
                var response = await res.PutAsync($"{controller}/order/{orderId}", res.GenerateHttpContent(order));
                if (!response.IsSuccessStatusCode)
                    throw new SystemException(await res.Error(response));
                return await response.GetResult<OrderPenjualan>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
    

        public Task<Penjualan> UpdatePenjualan(int penjualanId, Penjualan order)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetOrdersBySalesId(int supplierId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetOrdersByCustomerId(int supplierId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansByCustomerId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansBySalesId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePembayaran(PembayaranPenjualan model)
        {
            throw new NotImplementedException();
        }

        public Task<Penjualan> CreatePenjualan(int orderid, Penjualan model)
        {
            throw new NotImplementedException();
        }

      

       
    }
}
