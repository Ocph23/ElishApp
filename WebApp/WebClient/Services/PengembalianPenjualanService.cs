using Microsoft.EntityFrameworkCore;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public class PengembalianPenjualanService : IPengembalianPenjualanService
    {
        private readonly ApplicationDbContext dbContext;
        public PengembalianPenjualanService(ApplicationDbContext db)
        {
            dbContext = db;
        }

        public Task<IEnumerable<Penjualanitem>> GetPenjualanByCustomerId(int customerId)
        {
            var result = dbContext.Penjualan
                .Include(x=>x.Items).ThenInclude(x=>x.Penjualan)
                .Include(x=>x.Items).ThenInclude(x=>x.Product).ThenInclude(x=>x.Category)
                .Include(x=>x.Items).ThenInclude(x=>x.Product).ThenInclude(x=>x.Merk)
                .Include(x=>x.Items).ThenInclude(x => x.Product).ThenInclude(x=>x.Units)
                .Where(x => x.Customer.Id == customerId).SelectMany(x=>x.Items).ToList();
            return Task.FromResult(result.OrderByDescending(x=>x.Id) as IEnumerable<Penjualanitem>);
        }

        public Task<PengembalianPenjualan> Post(PengembalianPenjualan model)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {

                dbContext.PengembalianPenjualan.Add(model);
                dbContext.Entry(model.Customer).State = EntityState.Unchanged;
                dbContext.Entry(model.Gudang).State = EntityState.Unchanged;
                foreach (var item in model.Items)
                {
                    dbContext.Entry(item.Penjualan).State = EntityState.Unchanged;
                    dbContext.Entry(item.Product).State = EntityState.Unchanged;
                    dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                }
                dbContext.SaveChanges();
                trans.Commit();
                return Task.FromResult(model);
            }
            catch (Exception ex)
            {

                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }

        public Task<PengembalianPenjualan> Put(int id, PengembalianPenjualan model)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var lastData= dbContext.PengembalianPenjualan.Where(x => x.Id == id).Include(x => x.Items)
                                 .FirstOrDefault();
                if (lastData == null)
                    throw new SystemException("Penjualan Not Found  !");
                dbContext.Entry(lastData).CurrentValues.SetValues(model);

                foreach (var item in model.Items)
                {
                    if (item.Id <= 0)
                    {
                        if (item.Product != null)
                        {
                            dbContext.Entry(item.Product).State = EntityState.Unchanged;
                            dbContext.Entry(item.Penjualan).State = EntityState.Unchanged;
                            dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                        }
                        dbContext.PengembalianPenjualanItem.Add(item);
                    }
                    else
                    {
                        var oldItem = lastData.Items.SingleOrDefault(x => x.Id == item.Id);
                        if (item.Product != null)
                        {
                            dbContext.Entry(item.Product).State = EntityState.Unchanged;
                            dbContext.Entry(item.Penjualan).State = EntityState.Unchanged;
                            dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                        }
                        dbContext.Entry(oldItem).CurrentValues.SetValues(item);
                    }
                }


                //remove

                foreach (var item in lastData.Items)
                {
                    var existsDb = model.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (existsDb == null)
                    {
                        dbContext.PengembalianPenjualanItem.Remove(item);
                    }
                }

                dbContext.SaveChanges();
                trans.Commit();
                return Task.FromResult(model);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }

    }
}
