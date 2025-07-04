using ApsWebApp.Data;
using Microsoft.EntityFrameworkCore;
using ShareModels;
using ShareModels.ModelViews;

namespace ApsWebApp.Services
{
    public class StockService : IStockService
    {
        private ApplicationDbContext dbContext;
        private ILogger<PenjualanService> _logger;

        public StockService(ApplicationDbContext db, ILogger<PenjualanService> log)
        {
            dbContext = db;
            _logger = log;
        }


        public Task<IEnumerable<ProductStock>> GetProductStocks(int gudangId)
        {
            var query = from stock in dbContext.Stocks.Include(x => x.Gudang) select stock;

            if (gudangId > 0)
            {
                query = query.Where(x => x.Gudang.Id == gudangId);
            }

            var result = from stock in query
                        .Include(x => x.Product).ThenInclude(x => x.Category)
                        .Include(x => x.Product).ThenInclude(x => x.Supplier)
                         join c in dbContext.Category on stock.Product.Category.Id equals c.Id
                         join s in dbContext.Supplier on stock.Product.Supplier.Id equals s.Id
                         select new ProductStock
                         {
                             Category = c,
                             Supplier = s,
                             CodeArticle = stock.Product.CodeArticle,
                             CodeName = stock.Product.CodeName,
                             Description = stock.Product.Description,
                             Id = stock.Product.Id,
                             Merk = stock.Product.Merk,
                             Name = stock.Product.Name,
                             Discount = stock.Product.Discount,
                             Color = stock.Product.Color,
                             Stock = stock.Quantity,
                             Size = stock.Product.Size,
                             Units = stock.Product.Units,
                             SelectedUnit = stock.Product.UnitSelected
                         };


            return Task.FromResult(result.AsEnumerable());
        }

        /// <summary>
        /// Ambil Stock Semua Produk berdasarkan Gudang
        /// </summary>
        /// <param name="gudangId"></param>
        /// <returns></returns>
        public Task<IEnumerable<Stock>> GetStockByGudangId(int gudangId)
        {
            var data = dbContext.Stocks.Where(x => x.GudangId == gudangId).ToList();
            return Task.FromResult(data.AsEnumerable());
        }


        /// <summary>
        /// Ambil Stock berdasarkan ProductId di semua gudang
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <exception cref="SystemException"></exception>
        public Task<Stock> GetStockByProductId(int productId)
        {
            try
            {
                var stocks = from s in dbContext.Stocks
                             where s.ProductId == productId
                             select s;

                if (stocks.Any())
                {
                    var stock = stocks.FirstOrDefault();
                    stock.Quantity = stocks.Sum(x => x.Quantity);
                    return Task.FromResult(stock);
                }
                else
                {
                    return Task.FromResult(new Stock()
                    {
                        ProductId = productId,
                        Quantity = 0
                    });
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Stock> GetStockByProductIdAndGudangId(int productId, int gudangId)
        {
            try
            {
                var stocks = from s in dbContext.Stocks
                             where s.ProductId == productId && s.GudangId == gudangId
                             select s;

                if (stocks.Any())
                {
                    var stock = stocks.FirstOrDefault();
                    stock.Quantity = stocks.Sum(x => x.Quantity);
                    return Task.FromResult(stock);
                }
                else
                {
                    return Task.FromResult(new Stock()
                    {
                        ProductId = productId,
                        Quantity = 0
                    });
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> SyncStockFromMovmentStock()
        {
            try
            {

                var stockByProduct = dbContext.StockMovements
               .GroupBy(m => new { m.ProductId, m.GudangId })
               .Select(g => new
               {
                   ProductId = g.Key.ProductId,
                   GudangId = g.Key.GudangId,
                   Total = g.Where(x => x.StockMovementType == StockMovementType.IN).Sum(m => m.Quantity) - g.Where(x => x.StockMovementType == StockMovementType.OUT).Sum(m => m.Quantity)
               })
               .ToList();

                foreach (var item in stockByProduct)
                {
                    var existingStock = dbContext.Stocks
                        .FirstOrDefault(s => s.ProductId == item.ProductId && s.GudangId == item.GudangId);

                    if (existingStock == null)
                    {
                        dbContext.Stocks.Add(new Stock
                        {
                            GudangId = item.GudangId,
                            ProductId = item.ProductId,
                            Quantity = item.Total
                        });
                    }
                    else
                    {
                        existingStock.Quantity = item.Total;
                        dbContext.Stocks.Update(existingStock);
                    }
                }

                dbContext.SaveChanges();

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }

        #region Stockmovement
        public async Task<bool> AddMovementStock(int productId, int gudangId,
          StockMovementType stockMovementType, ReferenceType referenceType, int referenceId, double quantity)
        {
            try
            {
                dbContext.StockMovements.Add(new StockMovement()
                {
                    GudangId = gudangId,
                    ProductId = productId,
                    StockMovementType = stockMovementType,
                    ReferenceType = referenceType,
                    ReferenceId = referenceId,
                    Quantity = quantity,
                    MovementDate = DateTime.Now.ToUniversalTime()
                });
                var stock = dbContext.Stocks.Where(x => x.ProductId == productId && x.GudangId == gudangId)
                   .FirstOrDefault();
                if (stock == null)
                {
                    dbContext.Stocks.Add(new Stock()
                    {
                        GudangId = gudangId,
                        ProductId = productId,
                        Quantity = quantity
                    });
                }
                else
                {
                    if (stockMovementType == StockMovementType.OUT)
                    {
                        if (stock.Quantity < quantity)
                        {
                            throw new SystemException($"Stock tidak cukup, {stock.Product.Name} sisa {stock.Quantity} {stock.Product.UnitSelected.Name} ");
                        }
                        stock.Quantity -= quantity;
                    }
                    else
                    {
                        stock.Quantity += quantity;
                    }
                }
                //dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error updating stock");
                throw new SystemException("Error updating stock");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating stock");
                throw new SystemException("Error updating stock");
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }



        public async Task<StockMovement> GetMovementStock(StockMovementType type, ReferenceType refType, int referenceId)
        {
            try
            {
                var result = await dbContext.StockMovements.FirstOrDefaultAsync(x => x.StockMovementType == type
                && x.ReferenceType == refType
                && x.ReferenceId == referenceId);
                return result!;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<bool> UpdateStockMovement(StockMovement stockMovemnet, double newStock)
        {
            try
            {
                var stock = dbContext.Stocks.FirstOrDefault(x => x.ProductId == stockMovemnet.ProductId && x.GudangId == stockMovemnet.GudangId);
                stock.Quantity += newStock - stockMovemnet.Quantity;
                stockMovemnet.Quantity = newStock;
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Stock> GetStockByProductIdAndGudangIdIncludeOrder(int productId, int gudangId)
        {
            try
            {
                var stocks = from s in dbContext.Stocks
                             where s.ProductId == productId && s.GudangId == gudangId
                             select s;

                if (stocks.Any())
                {
                    var stock = stocks.FirstOrDefault();
                    stock.Quantity = stocks.Sum(x => x.Quantity);


                    var orderPenjualan = dbContext.OrderPenjualan
                        .Include(x => x.Items)
                        .Include(x => x.Gudang)
                        .Where(x => x.Status == OrderStatus.Baru && x.Gudang.Id == gudangId)
                        .SelectMany(x => x.Items.Where(x => x.Product.Id == productId));


                    if (orderPenjualan.Count() > 0)
                    {
                        stock.Quantity -= orderPenjualan.Sum(x => x.Quantity);
                    }

                    return Task.FromResult(stock);
                }
                else
                {
                    return Task.FromResult(new Stock()
                    {
                        ProductId = productId,
                        Quantity = 0
                    });
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> RemoveStockMovement(StockMovement stockMovement, double newStock)
        {
            try
            {
                dbContext.StockMovements.Remove(stockMovement);
                await UpdateStockMovement(stockMovement, newStock);
                return true;
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }

        #endregion
    }


}
