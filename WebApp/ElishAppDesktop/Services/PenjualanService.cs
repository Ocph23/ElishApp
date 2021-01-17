using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElishAppDesktop.Services
{
    public class PenjualanService : IPenjualanService
    {
        readonly string controller = "/api/penjualan";
        public async Task<Orderpenjualan> CreateOrder(Orderpenjualan order)
        {
            try
            {
                using var res = new RestService();
                var response = await res.PostAsync($"{controller}/order", res.GenerateHttpContent(order));
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<Orderpenjualan>();
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

        public Task<IEnumerable<Orderpenjualan>> GetOrders()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Orderpenjualan>> GetOrdersByCustomerId(int supplierId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Orderpenjualan>> GetOrdersBySalesId(int id)
        {
            try
            {
                using var res = new RestService();
                var response = await res.GetAsync($"{controller}/OrderBySales/{id}");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                return await response.GetResult<IEnumerable<Orderpenjualan>>();
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

        public Task<IEnumerable<PenjualanViewModel>> GetPenjualans(DateTime start, DateTime end)
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
