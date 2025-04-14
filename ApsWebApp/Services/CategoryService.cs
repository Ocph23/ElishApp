using ApsWebApp.Data;
using ApsWebApp.Validations;
using FluentValidation;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApsWebApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IValidator<Category> categoriValidator;

        public CategoryService(ApplicationDbContext db, IValidator<Category> _categoriValidator)
        {
            dbContext = db;
            categoriValidator = _categoriValidator;
        }
        public  Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Category.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Category.Remove(existsModel);
               dbContext.SaveChanges();

                return Task.FromResult(true);
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

        public  Task<Category> Post(Category value)
        {
            try
            {
                var validateResult = categoriValidator.Validate(value);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));
                dbContext.Category.Add(value);
                dbContext.SaveChanges();
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public  Task<bool> Update(int id, Category value)
        {
            try
            {
                var validateResult = categoriValidator.Validate(value);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));
                var existsModel = dbContext.Category.Where(x => x.Id == id).FirstOrDefault();
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
