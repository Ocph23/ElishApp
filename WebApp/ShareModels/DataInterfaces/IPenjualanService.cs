using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IPenjualanService
    {
        Task<OrderPenjualan> CreateOrder(OrderPenjualan order);
        Task<IEnumerable<PenjualanAndOrderModel>> GetOrdersBySalesId(int supplierId);
        Task<IEnumerable<PenjualanAndOrderModel>> GetOrdersByCustomerId(int supplierId);
        Task<IEnumerable<PenjualanAndOrderModel>> GetOrders();
        Task<OrderPenjualan> GetOrder(int id);
        Task<OrderPenjualan> UpdateOrder(int orderId, OrderPenjualan order);
        Task<bool> DeleteOrder(int id);

        Task<Penjualan> CreatePenjualan(int orderid, Penjualan model);
        Task<Penjualan> UpdatePenjualan(int penjualanId, Penjualan order);
        Task<Penjualan> GetPenjualan(int id);
        Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualans();
        Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualans(DateTime start, DateTime end);
        Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansByCustomerId(int id);
        Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansBySalesId(int id);
        Task<bool> DeletePenjualan(int id);

        #region Pembayaran
        Task<IEnumerable<PembayaranPenjualan>> GetPembayaran(int penjualanId);
        Task<PembayaranPenjualan> CreatePembayaran(int penjualanId, PembayaranPenjualan model, bool forced);
        Task<bool> UpdatePembayaran(PembayaranPenjualan model);
        #endregion
    }








}
