using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public class CategoryService : ICategoryService
    {
        private OcphDbContext dbContext;
        private IUserService userService;
        public CategoryService(OcphDbContext db, IUserService _userService)
        {
            dbContext = db;
            userService = _userService;
        }
        public Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Categories.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                var deleted = dbContext.Categories.Delete(x => x.Id == id);
                if (deleted)
                    throw new SystemException("Data Not Deleted !");
                return Task.FromResult(deleted);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Category> Get(int id)
        {
            var result = dbContext.Categories.Where(x => x.Id == id).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Category>> Get()
        {
            var results = dbContext.Categories.Select().ToList();
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<Category> Post(Category value)
        {
            try
            {
                value.Id = dbContext.Categories.InsertAndGetLastID(value);
                if (value.Id <=0 )
                    throw new SystemException("Data Not Saved !");
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> Update(int id, Category value)
        {
            var existsModel = dbContext.Categories.Where(x => x.Id == id).FirstOrDefault();
            if (existsModel == null)
                throw new SystemException("Data Not Found !");

            var updated = dbContext.Categories.Update(x => new { x.Name, x.Description},
                value, x => x.Id == id);
            if (!updated)
                throw new SystemException("Data Not Saved !");
            return Task.FromResult(updated);
        }

    }
}
