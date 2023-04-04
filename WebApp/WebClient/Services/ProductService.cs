using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApsWebApp.Services
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

        public Task<Product> AddProduct(int supplierId, Product product)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                ValidateProduct(product);

                var exsitsProduct = dbContext.Product.Where(x => x.Id == product.Id || x.CodeName== product.CodeName).FirstOrDefault();
                    
                if(exsitsProduct!=null )
                    throw new SystemException($"Product '{product.CodeName} / {product.CodeArticle}' Exists !");


                dbContext.Entry(product.Supplier).State = EntityState.Unchanged;
                dbContext.Entry(product.Merk).State = EntityState.Unchanged;
                dbContext.Product.Add(product);
                dbContext.SaveChanges();
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
            try
            {
                ValidateUnit(unit);
                var product = dbContext.Product.Where(x => x.Id == productId).Include(x => x.Units).Include(x=>x.Merk)
                    .Include(x=>x.Category).FirstOrDefault();
                var lastUnit = product.Units.OrderBy(x => x.Level).LastOrDefault();
                unit.Level = lastUnit == null ? 0 : lastUnit.Level + 1;
                product.Units.Add(unit);
                dbContext.SaveChanges();
                if (unit.Id <= 0)
                    throw new SystemException($"Unit '{unit.Name}' Not Saved !");

                return Task.FromResult(unit);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Unit> UpdateUnit(int unitId, Unit unit)
        {

            try
            {
                var existsModel = dbContext.Unit.Where(x => x.Id == unitId).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException($"Unit '{unit.Name}' Not Found !");

                ValidateUnit(unit);


                dbContext.Entry(existsModel).CurrentValues.SetValues(unit);

                dbContext.SaveChanges();

                return Task.FromResult(unit);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


        public Task<bool> RemoveUnit(int unitId)
        {
            try
            {
                var oldUnits = dbContext.Unit.Where(x => x.Id == unitId).FirstOrDefault();
                if (oldUnits == null)
                    throw new SystemException("Data unit tidak ditemukan !");
                dbContext.Unit.Remove(oldUnits);
                var saveId = dbContext.SaveChanges();
                if (saveId <= 0)
                    throw new SystemException($"Unit Not Saved !");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        private void ValidateUnit(Unit unit)
        {
            if (string.IsNullOrEmpty(unit.Name))
            {
                throw new SystemException("Nama Unit Harus Disi");
            }

            if (unit.Buy >= unit.Sell)
            {
                throw new SystemException("Rugi Dong !");
            }
        }


        private void ValidateProduct(Product model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                throw new SystemException("Nama Product, Code Product Harus Disi");
            }

            if (model.Category== null )
            {
                throw new SystemException("Kategori Harus Disi");
            }
        }

        public Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Product.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Product.Remove(existsModel);
                dbContext.SaveChanges();
                return Task.FromResult(true);
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
               .Include(x => x.Units).Include(x=>x.Merk)
               .Include(x => x.Category)
               .AsNoTracking();
            return Task.FromResult(results.FirstOrDefault());
        }

        public Task<IEnumerable<Product>> Get()
        {
            var results = dbContext.Product
               .Include(x => x.ProductImage)
               .Include(x => x.Supplier)
               .Include(x => x.Units).Include(x=>x.Merk)
               .Include(x=>x.Category)
               .AsNoTracking();
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<IEnumerable<Product>> GetProductsBySupplier(int id)
        {
            var results = dbContext.Product.Where(x => x.Supplier.Id == id)
               .Include(x => x.ProductImage)
               .Include(x => x.Supplier)
               .Include(x => x.Units).Include(x=>x.Merk)
               .Include(x=>x.Category)
               .AsNoTracking();
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<Product> Post(Product value)
        {
            try
            {
                ValidateProduct(value);
                dbContext.Product.Add(value);
                dbContext.SaveChanges();
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
            try
            {
                ValidateProduct(value);
                var existsModel = dbContext.Product.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Entry(existsModel).CurrentValues.SetValues(value);
                existsModel.Category = value.Category;
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


        public Task<ProductImage> AddPhoto(ProductImage image)
        {
            try
            {
                image.FileName = Helper.CreateFileName("image");
                image.Thumb = Helper.CreateFileName("image");
                System.IO.File.WriteAllBytes(Helper.ImagePath + image.FileName, image.Buffer);
                System.IO.File.WriteAllBytes(Helper.ThumbPath + image.Thumb, Helper.CreateThumb(image.Buffer));
                dbContext.ProductImage.Add(image);
                dbContext.SaveChanges();
                if (image.Id <= 0)
                    throw new SystemException("Data Not Saved !");

                image.Buffer = null;
                return Task.FromResult(image);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> RemovePhoto(int id)
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
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


        #region Stock
        public Task<IEnumerable<ProductStock>> GetProductStock()
        {
            try
            {
                var result = dbContext.Product
                    .Include(x => x.ProductImage)
                    .Include(x=>x.Supplier)
                    .Include(x=>x.Merk)
                    .Include(x=>x.Category)
                    .Include(x => x.PenjualanItem)
                    .Include(x=> x.Units)
                    //.Include(x => x.Orderpenjualanitem)
                    //.ThenInclude(x => x.OrderPenjualan)
                    .Select(x => new ProductStock {
                        Category= x.Category,
                        Supplier = x.Supplier,
                        CodeArticle =x.CodeArticle,
                        CodeName = x.CodeName,
                        Description = x.Description,
                        Id = x.Id,
                        Merk = x.Merk,
                        Name = x.Name,      
                        Discount =x.Discount, Color=x.Color, 
                        Stock = x.PembelianItem.Sum(x=>x.Amount * x.Unit.Quantity)- x.PenjualanItem.Sum(x=>x.Quantity * x.Unit.Quantity),
                        Size = x.Size,
                        Units = x.Units, ProductImage= x.ProductImage,  
                        SelectedUnit= x.Units.FirstOrDefault(),
                    }).ToList();


                var orders = dbContext.OrderPenjualan.Where(x => x.Status == OrderStatus.Baru).Include(x => x.Items).ThenInclude(x=>x.Product).SelectMany(x=>x.Items).ToList();

                foreach (var item in result)
                {
                    var totalNewOrder = orders.Where(x => x.Product.Id == item.Id).Sum(x => x.Quantity * x.Unit.Quantity);
                    item.Stock += totalNewOrder;
                }

                return Task.FromResult(result.AsEnumerable());
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }
        public Task<IEnumerable<ProductStock>> GetProductStockByGudangId(int merkId ,int gudangId, bool withOrder=true)
        {
            try
            {

                IQueryable<Pembelian> pembelian = dbContext.Pembelian
                 .Include(x => x.Gudang)
                 .Include(x => x.Items).ThenInclude(x => x.Unit)
                 .Include(x => x.Items).ThenInclude(x => x.Product);

                IQueryable<Penjualan> penjualan = dbContext.Penjualan
               .Include(x => x.Gudang)
               .Include(x => x.Items).ThenInclude(x => x.Unit)
               .Include(x => x.Items).ThenInclude(x => x.Product);

                IQueryable<PengembalianPenjualan> pengembalianPenjualan = dbContext.PengembalianPenjualan
                .Include(x => x.Gudang)
                .Include(x => x.Items)
                .Include(x => x.Items).ThenInclude(x => x.Product);

                IQueryable<Pemindahan> pemindahans = dbContext.Pemindahan
                    .Include(x => x.Dari)
                    .Include(x => x.Tujuan)
                    .Include(x => x.Items).ThenInclude(x => x.Unit)
                    .Include(x => x.Items).ThenInclude(x => x.Product);

                IEnumerable<ProductStock> pemindahan = new List<ProductStock>();


                if (gudangId > 0)
                {
                    pembelian = pembelian.Where(x => x.Gudang.Id == gudangId);
                    penjualan = penjualan.Where(x => x.Gudang.Id == gudangId);
                    pengembalianPenjualan = pengembalianPenjualan.Where(x => x.Gudang.Id == gudangId);

                    pemindahan = (from x in pemindahans.Where(x => x.Dari.Id == gudangId).SelectMany(x => x.Items)
                                  select new ProductStock
                                  {
                                      Category = x.Product.Category,
                                      Supplier = x.Product.Supplier,
                                      CodeArticle = x.Product.CodeArticle,
                                      CodeName = x.Product.CodeName,
                                      Description = x.Product.Description,
                                      Id = x.Product.Id,
                                      Merk = x.Product.Merk,
                                      Name = x.Product.Name,
                                      Discount = x.Product.Discount,
                                      Color = x.Product.Color,
                                      Stock = -1 * x.Quantity * x.Unit.Quantity,
                                      Size = x.Product.Size,
                                      Units = x.Product.Units,
                                      ProductImage = x.Product.ProductImage,
                                      SelectedUnit = x.Unit

                                  }).ToList();

                    IEnumerable<ProductStock> pemindahan1 = (from x in pemindahans.Where(x => x.Tujuan.Id == gudangId).SelectMany(x => x.Items)
                                                             select new ProductStock
                                                             {
                                                                 Category = x.Product.Category,
                                                                 Supplier = x.Product.Supplier,
                                                                 CodeArticle = x.Product.CodeArticle,
                                                                 CodeName = x.Product.CodeName,
                                                                 Description = x.Product.Description,
                                                                 Id = x.Product.Id,
                                                                 Merk = x.Product.Merk,
                                                                 Name = x.Product.Name,
                                                                 Discount = x.Product.Discount,
                                                                 Color = x.Product.Color,
                                                                 Stock = x.Quantity * x.Unit.Quantity,
                                                                 Size = x.Product.Size,
                                                                 Units = x.Product.Units,
                                                                 ProductImage = x.Product.ProductImage,
                                                                 SelectedUnit = x.Unit

                                                             }).ToList();
                    pemindahan = pemindahan.Concat(pemindahan1);



                        


                }

                IEnumerable<ProductStock> results = from x in pembelian.SelectMany(x => x.Items)
                                                    select new ProductStock
                                                    {
                                                        Category = x.Product.Category,
                                                        Supplier = x.Product.Supplier,
                                                        CodeArticle = x.Product.CodeArticle,
                                                        CodeName = x.Product.CodeName,
                                                        Description = x.Product.Description,
                                                        Id = x.Product.Id,
                                                        Merk = x.Product.Merk,
                                                        Name = x.Product.Name,
                                                        Discount = x.Product.Discount,
                                                        Color = x.Product.Color,
                                                        Stock = x.Amount * x.Unit.Quantity,
                                                        Size = x.Product.Size,
                                                        Units = x.Product.Units,
                                                        ProductImage = x.Product.ProductImage,
                                                        SelectedUnit = x.Unit

                                                    };


                IEnumerable<ProductStock> penjualans = from x in penjualan.SelectMany(x => x.Items)
                                 select new ProductStock
                                 {
                                     Id = x.Product.Id,
                                     Stock = -1 * x.Quantity * x.Unit.Quantity,
                                     SelectedUnit = x.Unit,
                                 };
                results = results.Concat(penjualans);


                results = results.Concat(from x in pengembalianPenjualan.SelectMany(x => x.Items)
                                         select new ProductStock
                                         {
                                             Id = x.Product.Id,
                                             Stock = x.Quantity * x.Unit.Quantity,
                                             SelectedUnit = x.Unit,
                                         });

                if(pemindahan!=null)
                    results = results.Concat(pemindahan);

                var resultGroup = from x in results.GroupBy(x => x.Id)
                                  select new ProductStock
                                  {
                                      Category = x.First().Category,
                                      Supplier = x.First().Supplier,
                                      CodeArticle = x.First().CodeArticle,
                                      CodeName = x.First().CodeName,
                                      Description = x.First().Description,
                                      Id = x.First().Id,
                                      Merk = x.First().Merk,
                                      Name = x.First().Name,
                                      Discount = x.First().Discount,
                                      Color = x.First().Color,
                                      Stock = x.Sum(m => m.Stock),
                                      Size = x.First().Size,
                                      Units = x.First().Units,
                                      SelectedUnit = x.First().SelectedUnit
                                  };

                var finalResult = merkId <= 0 ? resultGroup.AsEnumerable() : resultGroup.Where(x => x.Merk.Id == merkId);

                return Task.FromResult( finalResult.AsEnumerable());


            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }


        #endregion
    }
}
