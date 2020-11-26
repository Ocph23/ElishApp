using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Services
{

    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetSuppliers();
        Task<Supplier> GetSupplier(int supplierId);
        Task<Supplier> Post(Supplier value);
        Task<bool> Update(int id, Supplier value);
        Task<bool> Delete(int id);
        Task<IEnumerable<Product>> GetProducts(int supplierId);
    }

    public class SupplierService : ISupplierService
    {
        private OcphDbContext dbContext;

        public SupplierService(OcphDbContext db)
        {
            dbContext = db;
        }

        public Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Suppliers.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                var deleted = dbContext.Suppliers.Delete(x => x.Id == id);
                if (deleted)
                    throw new SystemException("Data Not Deleted !");
                return Task.FromResult(deleted);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<Product>> GetProducts(int supplierId)
        {
            var results = from sp in dbContext.SupplierProducts.Where(x => x.SupplierId == supplierId)
                          join p in dbContext.Products.Select() on sp.ProductId equals p.Id
                          join c in dbContext.Categories.Select() on p.CategoryId equals c.Id
                          select new Product
                          {
                              Id = p.Id,
                              Merk = p.Merk,
                              CategoryId = p.CategoryId,
                              CodeArticle = p.CodeArticle,
                              CodeName = p.CodeName,
                              Name = p.Name,
                              Category = c

                          };
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<Supplier> GetSupplier(int supplierId)
        {
            var result = dbContext.Suppliers.Where(x => x.Id == supplierId).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Supplier>> GetSuppliers()
        {
            var results = dbContext.Suppliers.Select().ToList();
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<Supplier> Post(Supplier value)
        {
            try
            {
                value.Id = dbContext.Suppliers.InsertAndGetLastID(value);
                if (value.Id <= 0)
                    throw new SystemException("Data Not Saved !");
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> Update(int id, Supplier value)
        {
            var existsModel = dbContext.Suppliers.Where(x => x.Id == id).FirstOrDefault();
            if (existsModel == null)
                throw new SystemException("Data Not Found !");

            var updated = dbContext.Suppliers.Update(x => new { x.Address, x.ContactPerson, x.ContactPersonName, x.Email, x.Nama, x.NPWP, x.Telepon },
                value, x => x.Id == id);
            if (!updated)
                throw new SystemException("Data Not Saved !");
            return Task.FromResult(updated);
        }
    }
}
