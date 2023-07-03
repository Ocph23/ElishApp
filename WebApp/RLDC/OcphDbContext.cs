using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ocph.DAL.DbContext;
using Ocph.DAL.Provider.MySql;
using Ocph.DAL.Repository;
using ShareModels;
using System;
using System.Data;
using System.Linq;

namespace RLDC
{
    public class OcphDbContext : DbContext
    {
        //private IDbConnection connection;
        //private IDataTables _dt;
        //public OcphDbContext(IDbConnection con, IDataTables dt)
        //{
        //    this.connection = con;
        //    _dt = dt;
        //}
        //public DbSet<User> Users { get { return new Repository<User>(connection); } }
        //public DbSet<Userrole> UserRoles { get { return new Repository<Userrole>(connection); } }
        //public DbSet<Role> Roles { get { return new Repository<Role>(connection); } }
        //public DbSet<Unit> Units { get { return new Repository<Unit>(connection); } }

        public OcphDbContext(DbContextOptions<OcphDbContext> options) : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplierproduct>()
                .HasNoKey();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Supplierproduct> SupplierProduct { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }


        //public DbSet<Orderpembelian> OrderPembelians { get { return new Repository<Orderpembelian>(connection); } }
        //public DbSet<OrderpembelianItem> OrderPembelianItems { get { return new Repository<OrderpembelianItem>(connection); } }
        //public DbSet<Pembelian> Pembelians { get { return new Repository<Pembelian>(connection); } }
        //public DbSet<PembelianItem> PembelianItems { get { return new Repository<PembelianItem>(connection); } }


        //public DbSet<Orderpenjualan> OrderPenjualans { get { return new Repository<Orderpenjualan>(connection); } }
        //public DbSet<OrderPenjualanItem> OrderPenjualanItems { get { return new Repository<OrderPenjualanItem>(connection); } }
        //public DbSet<Penjualan> Penjualans { get { return new Repository<Penjualan>(connection); } }
        //public DbSet<Penjualanitem> PenjualanItems { get { return new Repository<Penjualanitem>(connection); } }


        //public DbSet<Pembayaranpembelian> PembayaranPembelians { get { return new Repository<Pembayaranpembelian>(connection); } }
        //public DbSet<Pembayaranpenjualan> PembayaranPenjualans { get { return new Repository<Pembayaranpenjualan>(connection); } }


        //public DbSet<Customer> Customers { get { return new Repository<Customer>(connection); } }
        //public DbSet<Karyawan> Karyawans { get { return new Repository<Karyawan>(connection); } }


    }


}