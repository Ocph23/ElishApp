using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public class MerkService : IMerkService
    {
        private readonly ApplicationDbContext dbContext;
        public MerkService(ApplicationDbContext db)
        {
            dbContext = db;
        }
        public  Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Merk.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Merk.Remove(existsModel);
                dbContext.SaveChanges();

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Merk> Get(int id)
        {
            var result = dbContext.Merk.Where(x => x.Id == id).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<Merk> GetBySupplier(int  supplerId)
        {
            var result = dbContext.Merk.Where(x => x.Id == supplerId).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Merk>> Get()
        {
            var results = dbContext.Merk;
            return Task.FromResult(results.AsEnumerable());
        }

        public  Task<Merk> Post(Merk value)
        {
            try
            {
                dbContext.Merk.Add(value);
                 dbContext.SaveChanges();
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public  Task<bool> Update(int id, Merk value)
        {
            try
            {
                var existsModel = dbContext.Merk.Where(x => x.Id == id).FirstOrDefault();
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
