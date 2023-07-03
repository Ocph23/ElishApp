using ApsWebApp.Data;
using Microsoft.EntityFrameworkCore;
using ShareModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApsWebApp.Services
{
    public class ReportService : IReportService
    {
        private ApplicationDbContext _dbContext;
        public ReportService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<IEnumerable<ShareModels.Reports.PiutangData>> GetPiutang()
        {
            var penjualans = _dbContext.Penjualan
                 .Where(x => x.Status == PaymentStatus.Belum || x.Status == PaymentStatus.Panjar)
                 .Include(x => x.Items)
                 .Include(x => x.Customer)
                 .Include(x => x.Salesman)
                 .Include(x => x.PembayaranPenjualan).ToList();


            var result = penjualans.Select(x => new ShareModels.Reports.PiutangData
            {
                PenjualanId = x.Id,
                Nomor = x.Nomor,
                Customer = x.Customer.Name,
                Sales = x.Salesman.Name,
                JatuhTempo = x.CreateDate.AddDays(x.DeadLine),
                Panjar = x.PembayaranPenjualan == null ? 0 : x.PembayaranPenjualan.Sum(x => x.PayValue),
                Tagihan = x.Total,
            });
            return Task.FromResult(result.AsEnumerable());
        }


        public Task<IEnumerable<ShareModels.Reports.PiutangData>> GetUtang()
        {
            var penjualans = _dbContext.Pembelian
                 .Where(x => x.Status == PaymentStatus.Belum || x.Status == PaymentStatus.Panjar)
                 .Include(x => x.Items)
                 .Include(x => x.OrderPembelian).ThenInclude(x => x.Supplier)
                 .Include(x => x.PembayaranPembelian).ToList();


            var result = penjualans.Select(x => new ShareModels.Reports.PiutangData
            {
                PenjualanId = x.Id,
                Nomor = x.Nomor,
                Customer = x.OrderPembelian.Supplier.Nama,
                JatuhTempo = x.PayDeadLine,
                Panjar = x.PembayaranPembelian== null ? 0 : x.PembayaranPembelian.Sum(x => x.PayValue),
                Tagihan = x.Total,
            });
            return Task.FromResult(result.AsEnumerable());
        }

    }
}
