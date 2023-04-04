using ShareModels.ModelViews;
using ShareModels.Reports;
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
        Task<OrderPembelian> CreateOrder(OrderPembelian order);
        Task<IEnumerable<OrderPembelian>> GetOrdersBySupplierId(int supplierId);
        Task<IEnumerable<OrderPembelian>> GetOrders();
        Task<OrderPembelian> GetOrder(int id);
        Task<OrderPembelian> UpdateOrder(int orderId, OrderPembelian order);
        Task<bool> DeleteOrder(int id);
        #endregion


        #region Pembelian
        Task<Pembelian> CreatePembelian(int orderid, int gudangId);
        Task<Pembelian> UpdatePembelian(int pembelianId, Pembelian order);
        Task<Pembelian> GetPembelian(int id);
        Task<IEnumerable<PembelianDataModel>> GetPembelians();
        Task<IEnumerable<Pembelian>> GetPembeliansBySupplierId(int id);
        Task<bool> DeletePembelian(int id);
        #endregion

        #region Pembayaran
        Task<IEnumerable<PembayaranPembelian>> GetPembayaran(int pembayaranId);
        Task<PembayaranPembelian> CreatePembayaran(int pembelianId, PembayaranPembelian model);
        Task<bool> UpdatePembayaran(PembayaranPembelian model);

        #endregion
    }
    public interface IPenjualanService
    {
        Task<OrderPenjualan> CreateOrder(OrderPenjualan order);
        Task<IEnumerable<PenjualanAndOrderModel>> GetOrdersBySalesId(int supplierId);
        Task<IEnumerable<PenjualanAndOrderModel>> GetOrdersByCustomerId(int supplierId);
        Task<IEnumerable<PenjualanAndOrderModel>> GetOrders();
        Task<OrderPenjualan> GetOrder(int id);
        Task<OrderPenjualan> UpdateOrder(int orderId, OrderPenjualan order);
        Task<bool> DeleteOrder(int id);

        Task<Penjualan> CreatePenjualan(int orderid, Penjualan model);
        Task<Penjualan> UpdatePenjualan(int penjualanId, Penjualan order);
        Task<Penjualan> GetPenjualan(int id);
        Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualans();
        Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualans(DateTime start, DateTime end);
        Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansByCustomerId(int id);
        Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansBySalesId(int id);
        Task<bool> DeletePenjualan(int id);

        #region Pembayaran
        Task<IEnumerable<PembayaranPenjualan>> GetPembayaran(int penjualanId);
        Task<PembayaranPenjualan> CreatePembayaran(int penjualanId, PembayaranPenjualan model, bool forced);
        Task<bool> UpdatePembayaran(PembayaranPenjualan model);
        #endregion
    }



    public interface IPemindahanService
    {
        Task<IEnumerable<Pemindahan>> Get();
        Task<Pemindahan> Get(int id);
        Task<Pemindahan> Post(Pemindahan model);
        Task<bool> Put(int id, Pemindahan model);
        Task<bool> Delete(int id);
    }


    public interface IProductService : IService<Product>
    {
        Task<IEnumerable<Product>> GetProductsBySupplier(int id);
        Task<Product> AddProduct(int supplierId, Product product);
        Task<Unit> AddUnit(int productId, Unit unit);
        Task<Unit> UpdateUnit(int unitId, Unit unit);
        Task<ProductImage> AddPhoto(ProductImage image);
        Task<bool> RemovePhoto(int id);
        Task<bool> RemoveUnit(int id);
        Task<IEnumerable<ProductStock>> GetProductStock();
        Task<IEnumerable<ProductStock>> GetProductStockByGudangId(int merkId, int gudangid, bool withOrder);
        
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
    public interface IMerkService : IService<Merk>{}
    public interface IGudangService : IService<Gudang>{}
    public interface ICategoryService : IService<Category>{}
    public interface ICustomerService : IService<Customer>{
        //ObservableCollection<Customer> CustomerCollection { get; set; }

        Task<IEnumerable<Customer>> GetBySales(int id);
        Task<bool> UpdateLocation(Customer cust);
    }
    public interface IKaryawanService : IService<Karyawan>
    {
        Task<bool> RemoveRole(int id);
        Task<IEnumerable<Karyawan>> GetSales();
    }

    public interface IReportService
    {
        Task<IEnumerable<PiutangData>> GetPiutang();
        Task<IEnumerable<PiutangData>> GetUtang();
    }

    public interface IPengembalianPenjualanService
    {
        Task<IEnumerable<Penjualanitem>> GetPenjualanByCustomerId(int customerId);
        Task<PengembalianPenjualan> Post(PengembalianPenjualan model);
        Task<PengembalianPenjualan> Put(int id, PengembalianPenjualan model);

    }








}
