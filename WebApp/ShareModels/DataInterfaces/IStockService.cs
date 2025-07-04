using ShareModels.ModelViews;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IStockService
    {
        /// <summary>
        /// Ambil Stock semua produk berdasarkan Gudang
        /// </summary>
        /// <param name="GudangId"></param>
        /// <returns></returns>
        public Task<IEnumerable<Stock>> GetStockByGudangId(int gudangId);
        public Task<Stock> GetStockByProductId(int productId);
        public Task<Stock> GetStockByProductIdAndGudangId(int productId, int gudangId);
        public Task<Stock> GetStockByProductIdAndGudangIdIncludeOrder(int productId, int gudangId);



        public Task<bool> SyncStockFromMovmentStock();
        Task<IEnumerable<ProductStock>> GetProductStocks(int gudangId);
        public Task<bool> AddMovementStock(int productId, int gudangId, StockMovementType stockMovementType,
            ReferenceType referenceType, int referenceId, double quantity);
        public Task<StockMovement> GetMovementStock(StockMovementType type, ReferenceType referanceType, int referenceId);

        public Task<bool> UpdateStockMovement(StockMovement movementStockId, double newStock);
        public Task<bool> RemoveStockMovement(StockMovement stockMovement, double newStock);
    }





}
