using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ocph.DAL;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    
    public class PemindahanService : IPemindahanService
    {
        private readonly ApplicationDbContext dbContext;
    //    private readonly IHttpContextAccessor auth;
        private readonly ILogger _logger;

        public PemindahanService(ILogger<PemindahanService> logger, ApplicationDbContext db)
        {
            dbContext = db;
       //     auth = httpContextAccessor;
            _logger = logger;
        }

        public Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Pemindahan.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Pemindahan.Remove(existsModel);
                dbContext.SaveChanges();

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Pemindahan> Get(int id)
        {
            var result = dbContext.Pemindahan
                .Include(x=>x.Dari)
                .Include(x=>x.Tujuan)
                .Include(x=>x.Items).ThenInclude(x=>x.Product)
                .Include(x=>x.Items).ThenInclude(x=>x.Unit)
                .Where(x => x.Id == id).FirstOrDefault();
            return Task.FromResult(result);
        }

     
        public Task<IEnumerable<Pemindahan>> Get()
        {
            var results = dbContext.Pemindahan.Include(x => x.Dari)
                .Include(x => x.Tujuan)
                .Include(x => x.Items).ThenInclude(x => x.Product)
                .Include(x => x.Items).ThenInclude(x => x.Unit).Include(x => x.Items);
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<Pemindahan> Post(Pemindahan value)
        {
            try
            {
                dbContext.ChangeTracker.Clear();
                var changeTracker = dbContext.ChangeTracker.Entries<Product>();
                Validate(value);
                dbContext.Entry(value.Dari).State = EntityState.Detached;
                dbContext.Entry(value.Tujuan).State = EntityState.Detached;

                foreach (var item in value.Items)
                {
                    dbContext.Entry(item.Pemindahan).State = EntityState.Unchanged;
                    dbContext.Entry(item.Product).State = EntityState.Unchanged;
                    dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                }
                dbContext.Pemindahan.Add(value);
                dbContext.SaveChanges();
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        private void Validate(Pemindahan value)
        {
            if (value.Dari == null || value.Tujuan == null)
                throw new SystemException("Asal Pemindahan Dan Tujuan Pemindahan Tidak Boleh Kosong !");

            if (value.Dari != null && value.Tujuan != null && value.Dari.Id == value.Tujuan.Id)
                throw new SystemException("Asal Pemindahan Dan Tujuan Pemindahan Tidak Boleh Sama !");

            if (value.Items==null || value.Items.Count <=0)
                throw new SystemException("Data Pemindahan Tidak Boleh Kosong !");


        }

        public Task<bool> Put(int id, Pemindahan value)
        {
            try
            {
                var existsModel = dbContext.Pemindahan.Include(x => x.Items).Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Entry(existsModel).CurrentValues.SetValues(value);
                foreach (var item in value.Items)
                {
                    if (item.Id <= 0)
                    {
                        dbContext.PemindahanItem.Add(item);
                    }
                    else
                    {
                        dbContext.Entry(item).State = EntityState.Modified;
                    }


                }
                var updated = dbContext.SaveChanges();
                if (updated <= 0)
                    throw new SystemException("Data Not Saved !");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }
       
    }
}