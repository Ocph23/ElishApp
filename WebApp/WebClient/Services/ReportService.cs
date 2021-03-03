using Microsoft.EntityFrameworkCore;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
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
                 .Include(x => x.OrderPenjualan).ThenInclude(x => x.Customer)
                 .Include(x => x.OrderPenjualan).ThenInclude(x => x.Sales)
                 .Include(x => x.Pembayaranpenjualan).ToList();


            var result = penjualans.Select(x => new ShareModels.Reports.PiutangData
            {
                PenjualanId = x.Id,
                Nomor = x.Nomor,
                Customer = x.OrderPenjualan.Customer.Name,
                Sales = x.OrderPenjualan.Sales.Name,
                JatuhTempo = x.CreateDate.AddDays(x.PayDeadLine),
                Discount = x.Discount,
                Panjar = x.Pembayaranpenjualan == null ? 0 : x.Pembayaranpenjualan.Sum(x => x.PayValue),
                Tagihan = x.Total,
            });
            return Task.FromResult(result.AsEnumerable());
        }
    }
}
