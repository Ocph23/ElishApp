using ShareModels.ModelViews;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IPembelianService
    {
        #region 
        Task<OrderPembelian> CreateOrder(OrderPembelian order);
        Task<IEnumerable<OrderPembelian>> GetOrdersBySupplierId(int supplierId);
        Task<IEnumerable<OrderPembelian>> GetOrders();
        Task<OrderPembelian> GetOrder(int id);
        Task<OrderPembelian> UpdateOrder(int orderId, OrderPembelian order);
        Task<bool> DeleteOrder(int id);
        #endregion


        #region Pembelian
        Task<Pembelian> CreatePembelian(int orderid, int gudangId);
        Task<Pembelian> UpdatePembelian(int pembelianId, Pembelian order);
        Task<Pembelian> GetPembelian(int id);
        Task<IEnumerable<PembelianDataModel>> GetPembelians();
        Task<IEnumerable<Pembelian>> GetPembeliansBySupplierId(int id);
        Task<bool> DeletePembelian(int id);
        #endregion

        #region Pembayaran
        Task<IEnumerable<PembayaranPembelian>> GetPembayaran(int pembayaranId);
        Task<PembayaranPembelian> CreatePembayaran(int pembelianId, PembayaranPembelian model);
        Task<bool> UpdatePembayaran(PembayaranPembelian model);

        #endregion
    }








}
