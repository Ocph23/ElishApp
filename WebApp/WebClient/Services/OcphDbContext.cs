using Microsoft.Extensions.Configuration;
using Ocph.DAL.DbContext;
using Ocph.DAL.Provider.MySql;
using Ocph.DAL.Repository;
using ShareModels;
using System;
using System.Data;
using System.Linq;

namespace WebClient
{
    public class OcphDbContext :  IDisposable
    {
        private IDbConnection connection;
        private IDataTables _dt;

        public OcphDbContext(IDbConnection con, IDataTables dt)
        {
            this.connection = con;
            _dt = dt;
        }

        public IRepository<User> Users { get { return new Repository<User>(connection); } }
        public IRepository<Userrole> UserRoles { get { return new Repository<Userrole>(connection); } }
        public IRepository<Role> Roles { get { return new Repository<Role>(connection); } }
        public IRepository<Unit> Units { get { return new Repository<Unit>(connection); } }
        public IRepository<Supplier> Suppliers { get { return new Repository<Supplier>(connection); } }
        public IRepository<Supplierproduct> SupplierProducts { get { return new Repository<Supplierproduct>(connection); } }
        public IRepository<Product> Products { get { return new Repository<Product>(connection); } }
        public IRepository<Category> Categories { get { return new Repository<Category>(connection); } }


        public IRepository<Orderpembelian> OrderPembelians { get { return new Repository<Orderpembelian>(connection); } }
        public IRepository<OrderpembelianItem> OrderPembelianItems { get { return new Repository<OrderpembelianItem>(connection); } }
        public IRepository<Pembelian> Pembelians { get { return new Repository<Pembelian>(connection); } }
        public IRepository<PembelianItem> PembelianItems { get { return new Repository<PembelianItem>(connection); } }


        public IRepository<Orderpenjualan> OrderPenjualans { get { return new Repository<Orderpenjualan>(connection); } }
        public IRepository<OrderPenjualanItem> OrderPenjualanItems { get { return new Repository<OrderPenjualanItem>(connection); } }
        public IRepository<Penjualan> Penjualans { get { return new Repository<Penjualan>(connection); } }
        public IRepository<Penjualanitem> PenjualanItems { get { return new Repository<Penjualanitem>(connection); } }


        public IRepository<Pembayaranpembelian> PembayaranPembelians { get { return new Repository<Pembayaranpembelian>(connection); } }
        public IRepository<Pembayaranpenjualan> PembayaranPenjualans { get { return new Repository<Pembayaranpenjualan>(connection); } }


        public IRepository<Customer> Customers { get { return new Repository<Customer>(connection); } }
        public IRepository<Karyawan> Karyawans { get { return new Repository<Karyawan>(connection); } }
        public IRepository<IncomingItem> IncomingItems { get { return new Repository<IncomingItem>(connection); } }

        public void Dispose()
        {
            try
            {
                connection.Close();
            }
            catch (Exception)
            {
            }
        }

        internal IDbTransaction BeginTransaction()
        {
            return connection.BeginTransaction();
        }

        internal IDbCommand CreateCommand()
        {
            return connection.CreateCommand();
        }
    }


}