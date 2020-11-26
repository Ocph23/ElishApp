using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{

    public interface IProductService : IService<Product>
    {
        Task<IEnumerable<Product>> GetProductsBySupplier(int id);
        Task<Product> AddProduct(int supplierId, Product product);
        Task<Unit> AddUnit(int productId, Unit unit);
        Task<Unit> UpdateUnit(int unitId, Unit unit);
    }

    public class ProductService : IProductService
    {
        private OcphDbContext dbContext;

        public ProductService(OcphDbContext db)
        {
            dbContext = db;
        }

        public Task<Product> AddProduct(int supplierId, Product product)
        {
            var trans = dbContext.BeginTransaction();
            try
            {
                var exsitsProduct = dbContext.Products.Where(x => x.Id == product.Id || x.CodeArticle == product.CodeArticle || x.CodeName == product.CodeName).FirstOrDefault();
                if (exsitsProduct == null)
                {
                    product.Id = dbContext.Products.InsertAndGetLastID(product);
                    if (product.Id <= 0)
                        throw new SystemException("Product Not Created !");
                    exsitsProduct = product;
                }

                var supProd = dbContext.SupplierProducts.Where(x => x.SupplierId == supplierId && x.ProductId == product.Id).FirstOrDefault();
                if (supProd != null)
                    throw new SystemException($"Product '{product.CodeName} / {product.CodeArticle}' Exists !");

                supProd = new Supplierproduct { ProductId = exsitsProduct.Id, SupplierId = supplierId };

                var supProdSaved = dbContext.SupplierProducts.Insert(supProd);
                if (!supProdSaved)
                    throw new SystemException($"Product '{product.CodeName} / {product.CodeArticle}' Not Added !");

                trans.Commit();
                return Task.FromResult(product);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new SystemException(ex.Message);
            }

        }

        public Task<Unit> AddUnit(int productId, Unit unit)
        {
            var units = dbContext.Units.Where(x => x.ProductId == productId).OrderBy(x => x.Level);
            var lastUnit = units.LastOrDefault();
            unit.Level = lastUnit == null ? 0 : lastUnit.Level + 1;
            unit.Id = dbContext.Units.InsertAndGetLastID(unit);
            if (unit.Id <= 0)
                throw new SystemException($"Unit '{unit.Name}' Not Saved !");

            return Task.FromResult(unit);
        }

        public Task<Unit> UpdateUnit(int unitId, Unit unit)
        {

            var existsModel = dbContext.Units.Where(x => x.Id == unitId).FirstOrDefault();
            if (existsModel == null)
                throw new SystemException($"Unit '{unit.Name}' Not Found !");

            var updated = dbContext.Units.Update(x => new { x.Amount, x.Buy, x.Name, x.Sell, x.Level }, unit, x => x.Id == unitId);
            if (!updated)
                throw new SystemException($"Unit '{unit.Name}' Not Saved !");

            return Task.FromResult(unit);
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

        public Task<Product> Get(int id)
        {
            var results = from p in dbContext.Products.Where(x => x.Id == id)
                          join c in dbContext.Categories.Select() on p.CategoryId equals c.Id
                          join u in dbContext.Units.Select() on p.Id equals u.ProductId into ugr
                          from u in ugr.DefaultIfEmpty()
                          select new Product
                          {
                              Category = c,
                              CategoryId = p.CategoryId,
                              CodeArticle = p.CodeArticle,
                              Description = p.Description,
                              Size = p.Size,
                              CodeName = p.CodeName,
                              Merk = p.Merk,
                              Id = p.Id,
                              Name = p.Name,
                              Units = ugr.ToList()
                          };
            return Task.FromResult(results.FirstOrDefault());
        }

        public Task<IEnumerable<Product>> Get()
        {
            var results = from p in dbContext.Products.Select()
                          join c in dbContext.Categories.Select() on p.CategoryId equals c.Id
                          join u in dbContext.Units.Select() on p.Id equals u.ProductId into ugr
                          from u in ugr.DefaultIfEmpty()
                          select new Product
                          {
                              Category = c,
                              CategoryId = p.CategoryId,
                              CodeArticle = p.CodeArticle,
                              Description = p.Description,
                              Size = p.Size,
                              CodeName = p.CodeName,
                              Merk = p.Merk,
                              Id = p.Id,
                              Name = p.Name,
                              Units = ugr.ToList()
                          };
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<IEnumerable<Product>> GetProductsBySupplier(int id)
        {
            var results = from s in dbContext.SupplierProducts.Where(x => x.SupplierId == id)
                          join p in dbContext.Products.Select() on s.ProductId equals p.Id
                          join c in dbContext.Categories.Select() on p.CategoryId equals c.Id
                          join u in dbContext.Units.Select() on p.Id equals u.ProductId into ugr
                          select new Product
                          {
                              Category = c,
                              CategoryId = p.CategoryId,
                              CodeArticle = p.CodeArticle,
                              Description = p.Description,
                              Size = p.Size,
                              CodeName = p.CodeName,
                              Merk = p.Merk,
                              Id = p.Id,
                              Name = p.Name,
                              Units = (from prod2 in ugr
                                       select prod2).ToList()
                          };
            var datas = results.ToList();
            return Task.FromResult(datas.AsEnumerable());
        }

        public Task<Product> Post(Product value)
        {
            try
            {
                value.Id = dbContext.Products.InsertAndGetLastID(value);
                if (value.Id <= 0)
                    throw new SystemException("Data Not Saved !");
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> Update(int id, Product value)
        {
            var existsModel = dbContext.Products.Where(x => x.Id == id).FirstOrDefault();
            if (existsModel == null)
                throw new SystemException("Data Not Found !");

            var updated = dbContext.Products.Update(x => new { x.CategoryId, x.Merk, x.CodeArticle, x.CodeName, x.Name, x.Description, x.Size },
                value, x => x.Id == id);
            if (!updated)
                throw new SystemException("Data Not Saved !");
            return Task.FromResult(updated);
        }


    }
}
