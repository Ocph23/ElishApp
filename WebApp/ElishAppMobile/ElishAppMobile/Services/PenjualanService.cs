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
        private IEnumerable<Orderpenjualan> orders;

        public async Task<Orderpenjualan> CreateOrder(Orderpenjualan order)
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
                        await res.Error(response);
                    return await response.GetResult<Orderpenjualan>();
                }
                else
                {
                    var datas = (await db.Get<SqlDataModelOrder, Orderpenjualan>()).ToList();
                    if (datas == null)
                        datas = new List<Orderpenjualan>();
                    datas.Add(order);
                    _ = db.Save<SqlDataModelOrder, Orderpenjualan>(datas.AsEnumerable());
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

        public async Task<Orderpenjualan> GetOrder(int id)
        {
            try
            {
                using var res = new RestService();
                var response = await res.GetAsync($"{controller}/order/{id}");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<Orderpenjualan>();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<Orderpenjualan>> GetOrders()
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
                        await res.Error(response);
                    return await response.GetResult<IEnumerable<Orderpenjualan>>();
                }
                else
                {
                    var datas = await db.Get<SqlDataModelOrder, Orderpenjualan>();
                    orders = datas;
                    return orders;
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<Orderpenjualan>> GetOrdersByCustomerId(int supplierId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Orderpenjualan>> GetOrdersBySalesId(int id)
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();
                var db = Xamarin.Forms.DependencyService.Get<ElishDbStore>();
                if (connection.Item1)
                {
                    await SyncOrders();
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}/OrderBySales/{id}");
                    if (!response.IsSuccessStatusCode)
                        await res.Error(response);
                    return await response.GetResult<IEnumerable<Orderpenjualan>>();
                }
                else
                {
                    var datas = await db.Get<SqlDataModelOrder, Orderpenjualan>();
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
                var datas = (await db.Get<SqlDataModelOrder, Orderpenjualan>()).ToList();
                if (datas != null && datas.Count>0)
                {
                    using var res = new RestService();
                    foreach (Orderpenjualan item in datas.ToList())
                    {
                        var response = await res.PostAsync($"{controller}/order", res.GenerateHttpContent(item));
                        if (!response.IsSuccessStatusCode)
                            throw new SystemException(await res.Error(response));
                        var model= await response.GetResult<Orderpenjualan>();
                        if (model != null)
                        {
                            datas.Remove(item);
                        }
                    }

                   _= db.Save<SqlDataModelOrder, Orderpenjualan>(datas.AsEnumerable());
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

        public Task<Penjualan> GetPenjualan(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Penjualan>> GetPenjualans()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Penjualan>> GetPenjualans(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Penjualan>> GetPenjualansByCustomerId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Penjualan>> GetPenjualansBySalesId(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Orderpenjualan> UpdateOrder(int orderId, Orderpenjualan order)
        {
            try
            {
                using var res = new RestService();
                var response = await res.PutAsync($"{controller}/order/{orderId}", res.GenerateHttpContent(order));
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<Orderpenjualan>();
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
    }
}
