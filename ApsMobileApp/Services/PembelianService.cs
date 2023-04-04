using Microsoft.Maui.Media;
using ShareModels;
using ShareModels.ModelViews;
using ShareModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApsMobileApp.Services
{
    public class PembelianService : IPembelianService
    {
        readonly string controller = "/api/pembelian";
        private IEnumerable<PembelianDataModel> pembelians;
        private ElishDbStore localDb;

        public PembelianService(ElishDbStore _localDb)
        {
            localDb = _localDb;
        }
        public Task<OrderPembelian> CreateOrder(OrderPembelian order)
        {
            throw new NotImplementedException();
        }

        public Task<PembayaranPembelian> CreatePembayaran(int pembelianId, PembayaranPembelian model, bool forced)
        {
            throw new NotImplementedException();
        }

        public async Task<PembayaranPembelian> CreatePembayaran(int pembelianId, PembayaranPembelian model)
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();

                if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.PostAsync($"{controller}/cretaepembayaran/{pembelianId}", res.GenerateHttpContent(model));
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return await response.GetResult<PembayaranPembelian>();
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

        public Task<Pembelian> CreatePembelian(int orderid)
        {
            throw new NotImplementedException();
        }

        public Task<Pembelian> CreatePembelian(int orderid, int gudangId)
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

        public Task<Pembelian> GetPembelian(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PembelianDataModel>> GetPembelians()
        {
            try
            {
                 if(pembelians==null || pembelians.Count()>0)
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    pembelians  = await response.GetResult<IEnumerable<PembelianDataModel>>();
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

        public Task<bool> UpdatePembayaran(PembayaranPembelian model)
        {
            throw new NotImplementedException();
        }

        public Task<Pembelian> UpdatePembelian(int pembelianId, Pembelian order)
        {
            throw new NotImplementedException();
        }

       public async Task<IEnumerable<PembayaranPembelian>> GetPembayaran(int pembelianId)
        {
            try
            {
                var connection = Helper.CheckInterNetConnection();

                if (connection.Item1)
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}/pembayaranbypembelianid/{pembelianId}");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    var results = await response.GetResult<IEnumerable<PembayaranPembelian>>();
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
    }
}
