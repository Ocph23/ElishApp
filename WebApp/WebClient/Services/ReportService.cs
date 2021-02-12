using Microsoft.EntityFrameworkCore;
using ShareModels;
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

        public Task<IEnumerable<Penjualan>> GetPiutang()
        {
            var penjualans = _dbContext.Penjualan
                 .Where(x => x.Status == PaymentStatus.None || x.Status == PaymentStatus.DownPayment)
                 .Include(x=>x.Items)
                 .Include(x => x.OrderPenjualan).ThenInclude(x=>x.Customer)
                 .Include(x => x.Pembayaranpenjualan);
            return Task.FromResult(penjualans.AsEnumerable());
        }
    }
}
