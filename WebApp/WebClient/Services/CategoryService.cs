using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext dbContext;
        public CategoryService(ApplicationDbContext db)
        {
            dbContext = db;
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Category.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Category.Remove(existsModel);
               await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Category> Get(int id)
        {
            var result = dbContext.Category.Where(x => x.Id == id).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Category>> Get()
        {
            var results = dbContext.Category;
            return Task.FromResult(results.AsEnumerable());
        }

        public async Task<Category> Post(Category value)
        {
            try
            {
                dbContext.Category.Add(value);
                await dbContext.SaveChangesAsync();
                return value;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> Update(int id, Category value)
        {
            try
            {
                var existsModel = dbContext.Category.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Entry(existsModel).CurrentValues.SetValues(value);

                var updated = await dbContext.SaveChangesAsync();
                if (updated <= 0)
                    throw new SystemException("Data Not Saved !");
                return true;
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }

    }
}
