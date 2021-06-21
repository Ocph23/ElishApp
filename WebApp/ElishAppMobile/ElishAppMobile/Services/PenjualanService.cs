using ElishAppMobile.Models;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElishAppMobile.Services
{
    public class PenjualanService : IPenjualanService
    {
        readonly string controller = "/api/penjualan";
        private IEnumerable<PenjualanAndOrderModel> orders;

        public async Task<OrderPenjualan> CreateOrder(OrderPenjualan order)
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();
                var db = Xamarin.Forms.DependencyService.Get<ElishDbStore>();

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
                    var datas = (await db.Get<SqlDataModelOrder, OrderPenjualan>()).ToList();
                    if (datas == null)
                        datas = new List<OrderPenjualan>();
                    datas.Add(order);
                    _ = db.Save<SqlDataModelOrder, OrderPenjualan>(datas.AsEnumerable());
                    return order;
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Pembayaranpenjualan> CreatePembayaran(int pembelianId, Pembayaranpenjualan model, bool forced)
        {
            throw new NotImplementedException();
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
                var db = Xamarin.Forms.DependencyService.Get<ElishDbStore>();
                if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}/order");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return await response.GetResult<IEnumerable<PenjualanAndOrderModel>>();
                }
                else
                {
                    var datas = await db.Get<SqlDataModelOrder, PenjualanAndOrderModel>();
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
                var db = Xamarin.Forms.DependencyService.Get<ElishDbStore>();
                var datas = (await db.Get<SqlDataModelOrder, OrderPenjualan>()).ToList();
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

                   _= db.Save<SqlDataModelOrder, OrderPenjualan>(datas.AsEnumerable());
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<Pembayaranpenjualan>> GetPembayaran(int pembelianId)
        {
            throw new NotImplementedException();
        }

        public async Task<Penjualan> GetPenjualan(int id)
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();
                var db = Xamarin.Forms.DependencyService.Get<ElishDbStore>();
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
                var db = Xamarin.Forms.DependencyService.Get<ElishDbStore>();
                if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return await response.GetResult<IEnumerable<PenjualanAndOrderModel>>();
                }
                else
                {
                    var datas = await db.Get<SqlDataModelOrder, PenjualanAndOrderModel>();
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
                var db = Xamarin.Forms.DependencyService.Get<ElishDbStore>();
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
                    var datas = await db.Get<SqlDataModelOrder, PenjualanAndOrderModel>();
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
        //        var db = Xamarin.Forms.DependencyService.Get<ElishDbStore>();
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
        //        var db = Xamarin.Forms.DependencyService.Get<ElishDbStore>();
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

        public Task<bool> UpdatePembayaran(Pembayaranpenjualan model)
        {
            throw new NotImplementedException();
        }
    }
}
