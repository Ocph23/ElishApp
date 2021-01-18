using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IService<T>
    {
        Task<IEnumerable<T>> Get();
        Task<T> Get(int id);
        Task<T> Post(T value);
        Task<bool> Update(int id, T value);
        Task<bool> Delete(int id);
    }

    public interface IUserStateService
    {
        AuthenticateResponse User { get; set; }
        Task<bool> Login(UserLogin model);
        Task Logout();
        Task Initialize();
    }

    public interface IUserAuthentification
    {
        Task<AuthenticateResponse> Authenticate(UserLogin model);
        Task<object> Profile();
    }

    public interface IUserService   : IUserAuthentification
    {
        Task<User> FindUserById(int id);
        Task<string> AuthenticateUSerProvider(User user);
        Task<string> GenerateToken(User user);
        Task<User> Register(RegisterModel user);
        Task AddUserRole(string v, User admin);
        Task<Customer> RegisterCustomer(Customer value);
        Task<Karyawan> RegisterKaryawan(Karyawan value);
        Task<User> FindUserByUserName(string userName);
        Task<User> FindUserByEmail(string email);
        Task<IEnumerable<User>> GetUsers();
    }

    public interface IPembelianService
    {
        #region 
        Task<Orderpembelian> CreateOrder(Orderpembelian order);
        Task<IEnumerable<Orderpembelian>> GetOrdersBySupplierId(int supplierId);
        Task<IEnumerable<Orderpembelian>> GetOrders();
        Task<Orderpembelian> GetOrder(int id);
        Task<Orderpembelian> UpdateOrder(int orderId, Orderpembelian order);
        Task<bool> DeleteOrder(int id);
        #endregion


        #region Pembelian
        Task<Pembelian> CreatePembelian(int orderid);
        Task<Pembelian> UpdatePembelian(int pembelianId, Pembelian order);
        Task<Pembelian> GetPembelian(int id);
        Task<IEnumerable<Pembelian>> GetPembelians();
        Task<IEnumerable<Pembelian>> GetPembeliansBySupplierId(int id);
        Task<bool> DeletePembelian(int id);
        #endregion

        #region Pembayaran
        Task<IEnumerable<Pembayaranpembelian>> GetPembayaran(int pembayaranId);
        Task<Pembayaranpembelian> CreatePembayaran(int pembelianId, Pembayaranpembelian model, bool forced);
        #endregion
    }
    public interface IPenjualanService
    {
        Task<Orderpenjualan> CreateOrder(Orderpenjualan order);
        Task<IEnumerable<Orderpenjualan>> GetOrdersBySalesId(int supplierId);
        Task<IEnumerable<Orderpenjualan>> GetOrdersByCustomerId(int supplierId);
        Task<IEnumerable<Orderpenjualan>> GetOrders();
        Task<Orderpenjualan> GetOrder(int id);
        Task<Orderpenjualan> UpdateOrder(int orderId, Orderpenjualan order);
        Task<bool> DeleteOrder(int id);

        Task<Penjualan> CreatePenjualan(int orderid);
        Task<Penjualan> UpdatePenjualan(int penjualanId, Penjualan order);
        Task<Penjualan> GetPenjualan(int id);
        Task<IEnumerable<Penjualan>> GetPenjualans();
        Task<IEnumerable<PenjualanViewModel>> GetPenjualans(DateTime start, DateTime end);
        Task<IEnumerable<Penjualan>> GetPenjualansByCustomerId(int id);
        Task<IEnumerable<Penjualan>> GetPenjualansBySalesId(int id);
        Task<bool> DeletePenjualan(int id);

        #region Pembayaran
        Task<IEnumerable<Pembayaranpenjualan>> GetPembayaran(int pembelianId);
        Task<Pembayaranpenjualan> CreatePembayaran(int pembelianId, Pembayaranpenjualan model, bool forced);
        #endregion
    }

    public interface IProductService : IService<Product>
    {
        Task<IEnumerable<Product>> GetProductsBySupplier(int id);
        Task<Product> AddProduct(int supplierId, Product product);
        Task<Unit> AddUnit(int productId, Unit unit);
        Task<Unit> UpdateUnit(int unitId, Unit unit);
        Task<IEnumerable<ProductStock>> GetProductStock();
        Task<ProductImage> AddPhoto(ProductImage image);
        Task<bool> RemovePhoto(int id);
    }

    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetSuppliers();
        Task<Supplier> GetSupplier(int supplierId);
        Task<IEnumerable<Product>> GetProducts(int supplierId);
        Task<Supplier> Post(Supplier value);
        Task<bool> Update(int id, Supplier value);
        Task<bool> Delete(int id);
    }

    public interface ICategoryService : IService<Category>{}
    public interface ICustomerService : IService<Customer>{
     ObservableCollection<Customer> CustomerCollection { get; set; }
    }
    public interface IKaryawanService : IService<Karyawan>{}













}
