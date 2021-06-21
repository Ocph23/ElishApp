using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElishAppMobile.Services
{
    public class PembelianService : IPembelianService
    {
        readonly string controller = "/api/pembelian";
        private IEnumerable<Pembelian> pembelians;

        public Task<OrderPembelian> CreateOrder(OrderPembelian order)
        {
            throw new NotImplementedException();
        }

        public Task<Pembayaranpembelian> CreatePembayaran(int pembelianId, Pembayaranpembelian model, bool forced)
        {
            throw new NotImplementedException();
        }

        public Task<Pembelian> CreatePembelian(int orderid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePembelian(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OrderPembelian> GetOrder(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderPembelian>> GetOrders()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderPembelian>> GetOrdersBySupplierId(int supplierId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Pembayaranpembelian>> GetPembayaran(int pembayaranId)
        {
            throw new NotImplementedException();
        }

        public Task<Pembelian> GetPembelian(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Pembelian>> GetPembelians()
        {
            try
            {
                 if(pembelians==null || pembelians.Count()>0)
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    pembelians = await response.GetResult<IEnumerable<Pembelian>>();
                }
                return pembelians;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<Pembelian>> GetPembeliansBySupplierId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OrderPembelian> UpdateOrder(int orderId, OrderPembelian order)
        {
            throw new NotImplementedException();
        }

        public Task<Pembelian> UpdatePembelian(int pembelianId, Pembelian order)
        {
            throw new NotImplementedException();
        }
    }
}
