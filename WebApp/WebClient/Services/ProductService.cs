using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public class ProductService : IProductService
    {
        readonly ApplicationDbContext dbContext;
        private readonly ILogger<ProductService> logger;

        public ProductService(ApplicationDbContext db, ILogger<ProductService> log)
        {
            dbContext = db;
            logger = log;
        }

        public async Task<Product> AddProduct(int supplierId, Product product)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var exsitsProduct = dbContext.Product.Where(x => x.Id == product.Id || x.CodeArticle == product.CodeArticle || x.CodeName == product.CodeName).FirstOrDefault();
                if (exsitsProduct == null)
                {
                    dbContext.Product.Add(product);
                    if (product.Id <= 0)
                        throw new SystemException("Product Not Created !");

                }  else
                    throw new SystemException($"Product '{product.CodeName} / {product.CodeArticle}' Exists !");

                product.SupplierId = supplierId;
                dbContext.Product.Add(product);
               await dbContext.SaveChangesAsync();
                trans.Commit();
                return product;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new SystemException(ex.Message);
            }

        }

        public async Task<Unit> AddUnit(int productId, Unit unit)
        {
            try
            {
                var units = dbContext.Unit.Where(x => x.ProductId == productId).OrderBy(x => x.Level);
                var lastUnit = units.LastOrDefault();
                unit.Level = lastUnit == null ? 0 : lastUnit.Level + 1;
                dbContext.Unit.Add(unit);
                await dbContext.SaveChangesAsync();
                if (unit.Id <= 0)
                    throw new SystemException($"Unit '{unit.Name}' Not Saved !");

                return unit;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Unit> UpdateUnit(int unitId, Unit unit)
        {

            try
            {
                var existsModel = dbContext.Unit.Where(x => x.Id == unitId).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException($"Unit '{unit.Name}' Not Found !");


                dbContext.Entry(existsModel).CurrentValues.SetValues(unit);

                await dbContext.SaveChangesAsync();

                return unit;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Product.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Product.Remove(existsModel);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Product> Get(int id)
        {
            var results = dbContext.Product.Where(x=>x.Id==id)
                 .Include(x => x.ProductImage)
               .Include(x => x.Supplier)
               .Include(x => x.Units)
               .Include(x => x.Category).AsNoTracking();
            return Task.FromResult(results.FirstOrDefault());
        }

        public Task<IEnumerable<Product>> Get()
        {
            var results = dbContext.Product
               .Include(x => x.ProductImage)
               .Include(x => x.Supplier)
               .Include(x => x.Units)
               .Include(x => x.Category).AsNoTracking();
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<IEnumerable<Product>> GetProductsBySupplier(int id)
        {
            var results = dbContext.Product.Where(x => x.SupplierId == id)
               .Include(x => x.ProductImage)
               .Include(x => x.Supplier)
               .Include(x => x.Units)
               .Include(x => x.Category).AsNoTracking();
            return Task.FromResult(results.AsEnumerable());
        }

        public async Task<Product> Post(Product value)
        {
            try
            {
                dbContext.Product.Add(value);
                await dbContext.SaveChangesAsync();
                if (value.Id <= 0)
                    throw new SystemException("Data Not Saved !");
                return (value);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> Update(int id, Product value)
        {
            try
            {
                var existsModel = dbContext.Product.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Entry(existsModel).CurrentValues.SetValues(value);
                dbContext.Entry(existsModel).CurrentValues.SetValues(value);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<ProductStock>> GetProductStock()
        {
            try
            {
                var result = dbContext.Product
                    .Include(x => x.ProductImage)
                    .Include(x=>x.Supplier)
                    .Include(x=>x.Category)
                    .Include(x => x.PembelianItem)
                    .Include(x => x.PenjualanItem)
                    .Include(x=> x.Units)
                    .Include(x => x.Orderpenjualanitem)
                    .ThenInclude(x => x.OrderPenjualan).Select(x => new ProductStock {
                        Category = x.Category,
                        Supplier = x.Supplier,
                        CategoryId = x.CategoryId,
                        CodeArticle =x.CodeArticle,
                        CodeName = x.CodeName,
                        Description = x.Description,
                        Id = x.Id,  SupplierId=x.SupplierId, 
                        Merk = x.Merk,
                        Name = x.Name,
                        Pembelian = x.PembelianItem.Sum(x=>x.Amount * x.Unit.Amount),
                        Penjualan = x.PenjualanItem.Sum(x=>x.Amount * x.Unit.Amount),
                        Size = x.Size,
                        Units = x.Units, ProductImage= x.ProductImage,  
                        SelectedUnit= x.Units.FirstOrDefault(),
                    }).ToList();


                var orders = dbContext.Orderpenjualan.Where(x => x.Status == OrderStatus.New).Include(x => x.Items).SelectMany(x=>x.Items).ToList();

                foreach (var item in result)
                {
                    var totalNewOrder = orders.Where(x => x.ProductId == item.Id).Sum(x => x.Amount * x.Unit.Amount);
                    item.Penjualan += totalNewOrder;
                }

                return Task.FromResult(result.AsEnumerable());
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        public async Task<ProductImage> AddPhoto(ProductImage image)
        {
            try
            {
                image.FileName = Helper.CreateFileName("image");
                image.Thumb = Helper.CreateFileName("image");
                System.IO.File.WriteAllBytes(Helper.ImagePath + image.FileName, image.Buffer);
                System.IO.File.WriteAllBytes(Helper.ThumbPath + image.Thumb, Helper.CreateThumb(image.Buffer));
                dbContext.ProductImage.Add(image);
                await dbContext.SaveChangesAsync();
                if (image.Id <= 0)
                    throw new SystemException("Data Not Saved !");

                image.Buffer = null;
                return (image);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> RemovePhoto(int id)
        {
            try
            {
                var data = dbContext.ProductImage.SingleOrDefault(x => x.Id == id);
                if (data == null)
                    throw new SystemException("Data Not Found !");

                var file = Helper.ImagePath + data.FileName;
                var thumb = Helper.ThumbPath + data.Thumb;

                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                if (System.IO.File.Exists(thumb))
                {
                    System.IO.File.Delete(thumb);
                }

                dbContext.ProductImage.Remove(data);
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
    }
}
