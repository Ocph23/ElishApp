using ApsWebApp.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ShareModels;
using ShareModels.ModelViews;

namespace ApsWebApp.Services
{
    public class ProductService : IProductService
    {
        readonly ApplicationDbContext dbContext;
        private readonly ILogger<ProductService> logger;
        private readonly IValidator<Unit> unitValidator;
        private readonly IValidator<Product> productValidator;

        public ProductService(ApplicationDbContext db, ILogger<ProductService> log,
            IValidator<Unit> _unitValidator,
            IValidator<Product> _productValidator)
        {
            dbContext = db;
            logger = log;
            unitValidator = _unitValidator;
            productValidator = _productValidator;
        }

        public async Task<Product> AddProduct(int supplierId, Product product)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {

                var validateResult = await productValidator.ValidateAsync(product);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));
                var exsitsProduct = dbContext.Product.Where(x => x.Id == product.Id || x.CodeName == product.CodeName).FirstOrDefault();
                if (exsitsProduct != null)
                    throw new SystemException($"Product '{product.CodeName} / {product.CodeArticle}' Exists !");

                dbContext.Entry(product.Supplier).State = EntityState.Unchanged;
                dbContext.Entry(product.Merk).State = EntityState.Unchanged;
                dbContext.Product.Add(product);
                dbContext.SaveChanges();
                trans.Commit();
                return await Task.FromResult(product);
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

                var validateResult = await unitValidator.ValidateAsync(unit);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));
                var product = dbContext.Product.Where(x => x.Id == productId).Include(x => x.Units).Include(x => x.Merk)
                    .Include(x => x.Category).FirstOrDefault();
                var lastUnit = product.Units.OrderBy(x => x.Level).LastOrDefault();
                unit.Level = lastUnit == null ? 0 : lastUnit.Level + 1;
                product.Units.Add(unit);
                dbContext.SaveChanges();
                if (unit.Id <= 0)
                    throw new SystemException($"Unit '{unit.Name}' Not Saved !");

                return await Task.FromResult(unit);
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
                var validateResult = await unitValidator.ValidateAsync(unit);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));

                var existsModel = dbContext.Unit.Where(x => x.Id == unitId).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException($"Unit '{unit.Name}' Not Found !");


                dbContext.Entry(existsModel).CurrentValues.SetValues(unit);

                dbContext.SaveChanges();

                return await Task.FromResult(unit);
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
            var results = dbContext.Product.Where(x => x.Id == id)
                 .Include(x => x.ProductImage)
               .Include(x => x.Supplier)
               .Include(x => x.Units).Include(x => x.Merk)
               .Include(x => x.Category)
               .AsNoTracking();
            return Task.FromResult(results.FirstOrDefault());
        }

        public Task<IEnumerable<Product>> Get()
        {
            var results = dbContext.Product
               .Include(x => x.ProductImage)
               .Include(x => x.Supplier)
               .Include(x => x.Units).Include(x => x.Merk)
               .Include(x => x.Category)
               .AsNoTracking();
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<IEnumerable<Product>> GetProductsBySupplier(int id)
        {
            var results = dbContext.Product.Where(x => x.Supplier.Id == id)
               .Include(x => x.ProductImage)
               .Include(x => x.Supplier)
               .Include(x => x.Units).Include(x => x.Merk)
               .Include(x => x.Category)
               .AsNoTracking();
            return Task.FromResult(results.AsEnumerable());
        }

        public async Task<Product> Post(Product value)
        {
            try
            {
                var validateResult = await productValidator.ValidateAsync(value);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));

                dbContext.Product.Add(value);
                dbContext.SaveChanges();
                if (value.Id <= 0)
                    throw new SystemException("Data Not Saved !");
                return await Task.FromResult(value);
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
                var validateResult = await productValidator.ValidateAsync(value);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));

                var existsModel = dbContext.Product
                    .Include(x => x.Category)
                    .Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Entry(existsModel).CurrentValues.SetValues(value);
                if (value.Category.Id != existsModel.Category.Id)
                {
                    existsModel.Category = value.Category;
                    dbContext.Entry(existsModel.Category).State = EntityState.Unchanged;
                }

                dbContext.SaveChanges();
                return await Task.FromResult(true);
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
        //public Task<IEnumerable<ProductStock>> GetProductStock()
        //{
        //    try
        //    {
        //        IQueryable<Pembelian> pembelian = dbContext.Pembelian
        //         .Include(x => x.Gudang)
        //         .Include(x => x.Items).ThenInclude(x => x.Unit)
        //         .Include(x => x.Items).ThenInclude(x => x.Product);

        //        IQueryable<Penjualan> penjualan = dbContext.Penjualan
        //       .Include(x => x.Gudang)
        //       .Include(x => x.Items).ThenInclude(x => x.Unit)
        //       .Include(x => x.Items).ThenInclude(x => x.Product);

        //        IQueryable<PengembalianPenjualan> pengembalianPenjualan = dbContext.PengembalianPenjualan
        //        .Include(x => x.Gudang)
        //        .Include(x => x.Items)
        //        .Include(x => x.Items).ThenInclude(x => x.Product);

        //        IQueryable<Pemindahan> pemindahans = dbContext.Pemindahan
        //            .Include(x => x.Dari)
        //            .Include(x => x.Tujuan)
        //            .Include(x => x.Items).ThenInclude(x => x.Unit)
        //            .Include(x => x.Items).ThenInclude(x => x.Product);

        //        IEnumerable<ProductStock> pemindahan = new List<ProductStock>();

        //        pemindahan = (from x in pemindahans.SelectMany(x => x.Items)
        //                      select new ProductStock
        //                      {
        //                          Category = x.Product.Category,
        //                          Supplier = x.Product.Supplier,
        //                          CodeArticle = x.Product.CodeArticle,
        //                          CodeName = x.Product.CodeName,
        //                          Description = x.Product.Description,
        //                          Id = x.Product.Id,
        //                          Merk = x.Product.Merk,
        //                          Name = x.Product.Name,
        //                          Discount = x.Product.Discount,
        //                          Color = x.Product.Color,
        //                          Stock = -1 * x.Quantity * x.Unit.Quantity,
        //                          Size = x.Product.Size,
        //                          Units = x.Product.Units,
        //                          ProductImage = x.Product.ProductImage,
        //                          SelectedUnit = x.Unit

        //                      }).ToList();

        //        IEnumerable<ProductStock> pemindahan1 = (from x in pemindahans.SelectMany(x => x.Items)
        //                                                 select new ProductStock
        //                                                 {
        //                                                     Category = x.Product.Category,
        //                                                     Supplier = x.Product.Supplier,
        //                                                     CodeArticle = x.Product.CodeArticle,
        //                                                     CodeName = x.Product.CodeName,
        //                                                     Description = x.Product.Description,
        //                                                     Id = x.Product.Id,
        //                                                     Merk = x.Product.Merk,
        //                                                     Name = x.Product.Name,
        //                                                     Discount = x.Product.Discount,
        //                                                     Color = x.Product.Color,
        //                                                     Stock = x.Quantity * x.Unit.Quantity,
        //                                                     Size = x.Product.Size,
        //                                                     Units = x.Product.Units,
        //                                                     ProductImage = x.Product.ProductImage,
        //                                                     SelectedUnit = x.Unit

        //                                                 }).ToList();
        //        pemindahan = pemindahan.Concat(pemindahan1);

        //        IEnumerable<ProductStock> results = from x in pembelian.SelectMany(x => x.Items)
        //                                            select new ProductStock
        //                                            {
        //                                                Category = x.Product.Category,
        //                                                Supplier = x.Product.Supplier,
        //                                                CodeArticle = x.Product.CodeArticle,
        //                                                CodeName = x.Product.CodeName,
        //                                                Description = x.Product.Description,
        //                                                Id = x.Product.Id,
        //                                                Merk = x.Product.Merk,
        //                                                Name = x.Product.Name,
        //                                                Discount = x.Product.Discount,
        //                                                Color = x.Product.Color,
        //                                                Stock = x.Amount * x.Unit.Quantity,
        //                                                Size = x.Product.Size,
        //                                                Units = x.Product.Units,
        //                                                ProductImage = x.Product.ProductImage,
        //                                                SelectedUnit = x.Unit

        //                                            };


        //        IEnumerable<ProductStock> penjualans = from x in penjualan.SelectMany(x => x.Items)
        //                                               select new ProductStock
        //                                               {
        //                                                   Id = x.Product.Id,
        //                                                   Stock = -1 * x.Quantity * x.Unit.Quantity,
        //                                                   SelectedUnit = x.Unit,
        //                                               };
        //        results = results.Concat(penjualans);


        //        results = results.Concat(from x in pengembalianPenjualan.SelectMany(x => x.Items)
        //                                 select new ProductStock
        //                                 {
        //                                     Id = x.Product.Id,
        //                                     Stock = x.Quantity * x.Unit.Quantity,
        //                                     SelectedUnit = x.Unit,
        //                                 });

        //        if (pemindahan != null)
        //            results = results.Concat(pemindahan);

        //        var resultGroup = from x in results.GroupBy(x => x.Id)
        //                          select new ProductStock
        //                          {
        //                              Category = x.First().Category,
        //                              Supplier = x.First().Supplier,
        //                              CodeArticle = x.First().CodeArticle,
        //                              CodeName = x.First().CodeName,
        //                              Description = x.First().Description,
        //                              Id = x.First().Id,
        //                              Merk = x.First().Merk,
        //                              Name = x.First().Name,
        //                              Discount = x.First().Discount,
        //                              Color = x.First().Color,
        //                              Stock = x.Sum(m => m.Stock),
        //                              Size = x.First().Size,
        //                              Units = x.First().Units,
        //                              SelectedUnit = x.First().SelectedUnit
        //                          };

        //        var finalResult = resultGroup.AsEnumerable();

        //        return Task.FromResult(finalResult.AsEnumerable());


        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.Message);
        //        throw new SystemException(ex.Message);
        //    }
        //}

        //public Task<IEnumerable<ProductStock>> GetProductStockByGudangId(int merkId, int gudangId)
        //{
        //    try
        //    {

        //        IQueryable<Pembelian> pembelian = dbContext.Pembelian
        //         .Include(x => x.Gudang)
        //         .Include(x => x.Items).ThenInclude(x => x.Unit)
        //         .Include(x => x.Items).ThenInclude(x => x.Product);

        //        IQueryable<Penjualan> penjualan = dbContext.Penjualan
        //       .Include(x => x.Gudang)
        //       .Include(x => x.Items).ThenInclude(x => x.Unit)
        //       .Include(x => x.Items).ThenInclude(x => x.Product);

        //        IQueryable<PengembalianPenjualan> pengembalianPenjualan = dbContext.PengembalianPenjualan
        //        .Include(x => x.Gudang)
        //        .Include(x => x.Items)
        //        .Include(x => x.Items).ThenInclude(x => x.Product);

        //        IQueryable<Pemindahan> pemindahans = dbContext.Pemindahan
        //            .Include(x => x.Dari)
        //            .Include(x => x.Tujuan)
        //            .Include(x => x.Items).ThenInclude(x => x.Unit)
        //            .Include(x => x.Items).ThenInclude(x => x.Product);

        //        IEnumerable<ProductStock> pemindahan = new List<ProductStock>();


        //        if (gudangId > 0)
        //        {
        //            pembelian = pembelian.Where(x => x.Gudang.Id == gudangId);
        //            penjualan = penjualan.Where(x => x.Gudang.Id == gudangId);
        //            pengembalianPenjualan = pengembalianPenjualan.Where(x => x.Gudang.Id == gudangId);

        //            pemindahan = (from x in pemindahans.Where(x => x.Dari.Id == gudangId).SelectMany(x => x.Items)
        //                          select new ProductStock
        //                          {
        //                              Category = x.Product.Category,
        //                              Supplier = x.Product.Supplier,
        //                              CodeArticle = x.Product.CodeArticle,
        //                              CodeName = x.Product.CodeName,
        //                              Description = x.Product.Description,
        //                              Id = x.Product.Id,
        //                              Merk = x.Product.Merk,
        //                              Name = x.Product.Name,
        //                              Discount = x.Product.Discount,
        //                              Color = x.Product.Color,
        //                              Stock = -1 * x.Quantity * x.Unit.Quantity,
        //                              Size = x.Product.Size,
        //                              Units = x.Product.Units,
        //                              ProductImage = x.Product.ProductImage,
        //                              SelectedUnit = x.Unit

        //                          }).ToList();

        //            IEnumerable<ProductStock> pemindahan1 = (from x in pemindahans.Where(x => x.Tujuan.Id == gudangId).SelectMany(x => x.Items)
        //                                                     select new ProductStock
        //                                                     {
        //                                                         Category = x.Product.Category,
        //                                                         Supplier = x.Product.Supplier,
        //                                                         CodeArticle = x.Product.CodeArticle,
        //                                                         CodeName = x.Product.CodeName,
        //                                                         Description = x.Product.Description,
        //                                                         Id = x.Product.Id,
        //                                                         Merk = x.Product.Merk,
        //                                                         Name = x.Product.Name,
        //                                                         Discount = x.Product.Discount,
        //                                                         Color = x.Product.Color,
        //                                                         Stock = x.Quantity * x.Unit.Quantity,
        //                                                         Size = x.Product.Size,
        //                                                         Units = x.Product.Units,
        //                                                         ProductImage = x.Product.ProductImage,
        //                                                         SelectedUnit = x.Unit

        //                                                     }).ToList();
        //            pemindahan = pemindahan.Concat(pemindahan1);
        //        }

        //        IEnumerable<ProductStock> results = from x in pembelian.SelectMany(x => x.Items)
        //                                            select new ProductStock
        //                                            {
        //                                                Category = x.Product.Category,
        //                                                Supplier = x.Product.Supplier,
        //                                                CodeArticle = x.Product.CodeArticle,
        //                                                CodeName = x.Product.CodeName,
        //                                                Description = x.Product.Description,
        //                                                Id = x.Product.Id,
        //                                                Merk = x.Product.Merk,
        //                                                Name = x.Product.Name,
        //                                                Discount = x.Product.Discount,
        //                                                Color = x.Product.Color,
        //                                                Stock = x.Amount * x.Unit.Quantity,
        //                                                Size = x.Product.Size,
        //                                                Units = x.Product.Units,
        //                                                ProductImage = x.Product.ProductImage,
        //                                                SelectedUnit = x.Unit

        //                                            };


        //        IEnumerable<ProductStock> penjualans = from x in penjualan.SelectMany(x => x.Items)
        //                                               select new ProductStock
        //                                               {
        //                                                   Id = x.Product.Id,
        //                                                   Stock = -1 * x.Quantity * x.Unit.Quantity,
        //                                                   SelectedUnit = x.Unit,
        //                                               };
        //        results = results.Concat(penjualans);


        //        results = results.Concat(from x in pengembalianPenjualan.SelectMany(x => x.Items)
        //                                 select new ProductStock
        //                                 {
        //                                     Id = x.Product.Id,
        //                                     Stock = x.Quantity * x.Unit.Quantity,
        //                                     SelectedUnit = x.Unit,
        //                                 });

        //        if (pemindahan != null)
        //            results = results.Concat(pemindahan);

        //        var resultGroup = from x in results.GroupBy(x => x.Id)
        //                          select new ProductStock
        //                          {
        //                              Category = x.First().Category,
        //                              Supplier = x.First().Supplier,
        //                              CodeArticle = x.First().CodeArticle,
        //                              CodeName = x.First().CodeName,
        //                              Description = x.First().Description,
        //                              Id = x.First().Id,
        //                              Merk = x.First().Merk,
        //                              Name = x.First().Name,
        //                              Discount = x.First().Discount,
        //                              Color = x.First().Color,
        //                              Stock = x.Sum(m => m.Stock),
        //                              Size = x.First().Size,
        //                              Units = x.First().Units,
        //                              SelectedUnit = x.First().SelectedUnit
        //                          };

        //        var finalResult = merkId <= 0 ? resultGroup.AsEnumerable() : resultGroup.Where(x => x.Merk.Id == merkId);

        //        return Task.FromResult(finalResult.AsEnumerable());


        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.Message);
        //        throw new SystemException(ex.Message);
        //    }
        //}

        //public Task<IEnumerable<ProductStock>> GetProductStocks(int gudangId)
        //{
        //    var query = from stock in dbContext.Stocks.Include(x => x.Gudang) select stock;

        //    if (gudangId > 0)
        //    {
        //        query = query.Where(x => x.Gudang.Id == gudangId);
        //    }

        //    var result = from stock in query
        //                .Include(x => x.Product).ThenInclude(x => x.Category)
        //                .Include(x => x.Product).ThenInclude(x => x.Supplier)
        //                 join c in dbContext.Category on stock.Product.Category.Id equals c.Id
        //                 join s in dbContext.Supplier on stock.Product.Supplier.Id equals s.Id
        //                 select new ProductStock
        //                 {
        //                     Category = c,
        //                     Supplier = s,
        //                     CodeArticle = stock.Product.CodeArticle,
        //                     CodeName = stock.Product.CodeName,
        //                     Description = stock.Product.Description,
        //                     Id = stock.Product.Id,
        //                     Merk = stock.Product.Merk,
        //                     Name = stock.Product.Name,
        //                     Discount = stock.Product.Discount,
        //                     Color = stock.Product.Color,
        //                     Stock = stock.Quantity,
        //                     Size = stock.Product.Size,
        //                     Units = stock.Product.Units,
        //                     SelectedUnit = stock.Product.UnitSelected
        //                 };


        //    return Task.FromResult(result.AsEnumerable());
        //}




        #endregion
    }
}
