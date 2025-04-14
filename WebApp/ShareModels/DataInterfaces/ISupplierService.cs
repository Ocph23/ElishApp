using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetSuppliers();
        Task<Supplier> GetSupplier(int supplierId);
        Task<IEnumerable<Product>> GetProducts(int supplierId);
        Task<Supplier> Post(Supplier value);
        Task<bool> Update(int id, Supplier value);
        Task<bool> Delete(int id);
    }








}
