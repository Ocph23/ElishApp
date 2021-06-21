using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public class GudangService : IGudangService
    {
        private readonly ApplicationDbContext dbContext;
        public GudangService(ApplicationDbContext db)
        {
            dbContext = db;
        }
        public  Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Gudang.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Gudang.Remove(existsModel);
                dbContext.SaveChanges();

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Gudang> Get(int id)
        {
            var result = dbContext.Gudang.Where(x => x.Id == id).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Gudang>> Get()
        {
            var results = dbContext.Gudang;
            return Task.FromResult(results.AsEnumerable());
        }

        public  Task<Gudang> Post(Gudang value)
        {
            try
            {
                dbContext.Gudang.Add(value);
                 dbContext.SaveChanges();
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public  Task<bool> Update(int id, Gudang value)
        {
            try
            {
                var existsModel = dbContext.Gudang.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Entry(existsModel).CurrentValues.SetValues(value);

                var updated =  dbContext.SaveChanges();
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
