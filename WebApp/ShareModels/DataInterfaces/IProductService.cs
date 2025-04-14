using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IProductService : IService<Product>
    {
        Task<IEnumerable<Product>> GetProductsBySupplier(int id);
        Task<Product> AddProduct(int supplierId, Product product);
        Task<Unit> AddUnit(int productId, Unit unit);
        Task<Unit> UpdateUnit(int unitId, Unit unit);
        Task<ProductImage> AddPhoto(ProductImage image);
        Task<bool> RemovePhoto(int id);
        Task<bool> RemoveUnit(int id);
      
    }








}
