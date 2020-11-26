using Microsoft.Extensions.Configuration;
using Ocph.DAL.Provider.MySql;
using Ocph.DAL.Repository;
using ShareModels;

namespace WebClient
{
    public class OcphDbContext : MySqlDbConnection
    {
        public OcphDbContext(IConfiguration configuration) 
        {
            this.ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IRepository<User> Users{ get { return new Repository<User>(this); } }
        public IRepository<Userrole> UserRoles{ get { return new Repository<Userrole>(this); } }
        public IRepository<Role> Roles{ get { return new Repository<Role>(this); } }
        public IRepository<Unit> Units { get { return new Repository<Unit>(this); } }
        public IRepository<Supplier> Suppliers { get { return new Repository<Supplier>(this); } }
        public IRepository<Supplierproduct> SupplierProducts { get { return new Repository<Supplierproduct>(this); } }
        public IRepository<Product> Products { get { return new Repository<Product>(this); } }
        public IRepository<Category> Categories { get { return new Repository<Category>(this); } }

        public IRepository<Orderpembelian> OrderPembelians { get { return new Repository<Orderpembelian>(this); } }
        public IRepository<OrderpembelianItem> OrderPembelianItems { get { return new Repository<OrderpembelianItem>(this); } }
        public IRepository<Pembelian> Pembelians { get { return new Repository<Pembelian>(this); } }
        public IRepository<PembelianItem> PembelianItems { get { return new Repository<PembelianItem>(this); } }


        public IRepository<Orderpenjualan> OrderPenjualans { get { return new Repository<Orderpenjualan>(this); } }
        public IRepository<OrderPenjualanItem> OrderPenjualanItems { get { return new Repository<OrderPenjualanItem>(this); } }
        public IRepository<Penjualan> Penjualans { get { return new Repository<Penjualan>(this); } }
        public IRepository<Penjualanitem> PenjualanItems { get { return new Repository<Penjualanitem>(this); } }
        public IRepository<Customer> Customers { get { return new Repository<Customer>(this); } }
        public IRepository<Karyawan> Karyawans { get { return new Repository<Karyawan>(this); } }

    }
}
