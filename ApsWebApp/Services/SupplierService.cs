using ApsWebApp.Data;
using ApsWebApp.Validations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApsWebApp.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IValidator<Supplier> supplierValidator;

        public SupplierService(ApplicationDbContext db, IValidator<Supplier> _supplierValidator)
        {
            dbContext = db;
            supplierValidator = _supplierValidator;
        }

        public  Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Supplier.SingleOrDefault(x => x.Id == id);
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Supplier.Remove(existsModel);
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<Product>> GetProducts(int supplierId)
        {
            var results = dbContext.Product.Where(x => x.Supplier.Id == supplierId)
                        .Include(x => x.Category);
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<Supplier> GetSupplier(int supplierId)
        {
            var result = dbContext.Supplier.SingleOrDefault(x => x.Id == supplierId);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Supplier>> GetSuppliers()
        {
            return Task.FromResult(dbContext.Supplier.AsEnumerable());
        }

        public  Task<Supplier> Post(Supplier value)
        {
            try
            {
                var validateResult = supplierValidator.Validate(value);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));
                dbContext.Supplier.Add(value);
                dbContext.SaveChanges();
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public  Task<bool> Update(int id, Supplier value)
        {
            try
            {
                var validateResult = supplierValidator.Validate(value);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));
                var existsModel = dbContext.Supplier.SingleOrDefault(x => x.Id == id);
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Entry(existsModel).CurrentValues.SetValues(value);
               dbContext.SaveChanges();
                return Task.FromResult(true);

            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
    }
}
