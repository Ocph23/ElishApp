using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using ShareModels;

namespace ApsWebApp.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Gudang> Gudang { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Merk> Merk { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<IncomingItem> IncomingItem { get; set; }
        public virtual DbSet<Karyawan> Karyawan { get; set; }
        public virtual DbSet<OrderPembelian> OrderPembelian { get; set; }
        public virtual DbSet<OrderPembelianItem> OrderPembelianItem { get; set; }
        public virtual DbSet<OrderPenjualan> OrderPenjualan { get; set; }
        public virtual DbSet<OrderPenjualanItem> OrderPenjualanItem { get; set; }
        //public virtual DbSet<PembayaranPembelian> PembayaranPembelian { get; set; }
        //public virtual DbSet<PembayaranPenjualan> PembayaranPenjualan { get; set; }
        public virtual DbSet<Pembelian> Pembelian { get; set; }
        public virtual DbSet<PembelianItem> PembelianItem { get; set; }
        public virtual DbSet<Penjualan> Penjualan { get; set; }
        public virtual DbSet<Penjualanitem> Penjualanitem { get; set; }
        public virtual DbSet<Pemindahan> Pemindahan { get; set; }
        public virtual DbSet<PemindahanItem> PemindahanItem { get; set; }
        public virtual DbSet<PengembalianPenjualan> PengembalianPenjualan { get; set; }
        public virtual DbSet<PengembalianPenjualanItem> PengembalianPenjualanItem { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductImage> ProductImage { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRole> Userrole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var constring = "server=151.106.112.219;port=3306;database=ApsDb;user=ocph23;password=Alpharian@77";
                optionsBuilder.UseMySql(constring, ServerVersion.AutoDetect(constring));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<Category>(entity =>
            //{
            //   // //entity.ToTable("category");

            //    entity.Property(e => e.Description).HasColumnType("longtext");

            //    entity.Property(e => e.Name).HasMaxLength(100);
            //});

            //modelBuilder.Entity<Customer>(entity =>
            //{
            //    //entity.ToTable("customer");

            //    entity.HasIndex(e => e.UserId)
            //        .HasName("fk_Customer_User1_idx");

            //    entity.Property(e => e.ContactName).HasMaxLength(45);

            //    entity.Property(e => e.Email).HasMaxLength(45);

            //    entity.Property(e => e.Name).HasMaxLength(200);

            //    entity.Property(e => e.NPWP)
            //        .HasColumnName("NPWP")
            //        .HasMaxLength(45);

            //    entity.Property(e => e.Telepon).HasMaxLength(45);

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.Customers)
            //        .HasForeignKey(d => d.UserId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_Customer_User1");
            //});

            //modelBuilder.Entity<IncomingItem>(entity =>
            //{
            //    //entity.ToTable("incomingitem");

            //    entity.HasIndex(e => e.PembelianId)
            //        .HasName("fk_IncomingItem_Pembelian1_idx");

            //    entity.HasIndex(e => new { e.ProductId, e.PembelianId })
            //        .HasName("pembelian_Product")
            //        .IsUnique();

            //    entity.HasOne(d => d.Pembelian)
            //        .WithMany(p => p.Incomingitem)
            //        .HasForeignKey(d => d.PembelianId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_IncomingItem_Pembelian1");

            //    entity.HasOne(d => d.Product)
            //        .WithMany(p => p.Incomingitem)
            //        .HasForeignKey(d => d.ProductId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_IncomingItem_Product1");
            //});

            //modelBuilder.Entity<Karyawan>(entity =>
            //{
            //    //entity.ToTable("karyawan");

            //    entity.HasIndex(e => e.UserId)
            //        .HasName("fk_Karyawan_User1_idx");

            //    entity.Property(e => e.Address).HasMaxLength(45);

            //    entity.Property(e => e.Email).HasMaxLength(45);

            //    entity.Property(e => e.Name).HasMaxLength(45);

            //    entity.Property(e => e.Telepon).HasMaxLength(45);

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.Sales)
            //        .HasForeignKey(d => d.UserId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_Karyawan_User1");
            //});

            //modelBuilder.Entity<OrderPembelian>(entity =>
            //{
            //    //entity.ToTable("orderpembelian");

            //    entity.HasIndex(e => e.SupplierId)
            //        .HasName("fk_OrderPembelian_Supplier1_idx");

            //    entity.HasOne(d => d.Supplier)
            //        .WithMany(p => p.Orderpembelian)
            //        .HasForeignKey(d => d.SupplierId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_OrderPembelian_Supplier1");
            //});

            //modelBuilder.Entity<OrderPembelianItem>(entity =>
            //{
            //    //entity.ToTable("orderpembelianitem");

            //    entity.HasIndex(e => e.OrderPembelianId)
            //        .HasName("fk_OrderPembelianItem_OrderPembelian1_idx");

            //    entity.HasIndex(e => e.ProductId)
            //        .HasName("fk_OrderPembelianItem_Product1_idx");

            //    entity.HasIndex(e => e.UnitId)
            //        .HasName("fk_PembelianItem_Unit1_idx");

            //    entity.HasOne(d => d.OrderPembelian)
            //        .WithMany(p => p.Items)
            //        .HasForeignKey(d => d.OrderPembelianId)
            //        .HasConstraintName("fk_OrderPembelianItem_OrderPembelian1");

            //    entity.HasOne(d => d.Product)
            //        .WithMany(p => p.Orderpembelianitem)
            //        .HasForeignKey(d => d.ProductId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_OrderPembelianItem_Product1");

            //    //entity.HasOne(d => d.Unit)
            //    //    .WithMany(p => p.OrderpembelianItem)
            //    //    .HasForeignKey(d => d.UnitId)
            //    //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    //    .HasConstraintName("fk_PembelianItem_Unit1");
            //});

            //modelBuilder.Entity<OrderPenjualan>(entity =>
            //{
            //    //entity.ToTable("orderpenjualan");

            //    entity.HasIndex(e => e.CustomerId)
            //        .HasName("fk_OrderPenjualan_Customer1_idx");

            //    entity.HasIndex(e => e.SalesId)
            //        .HasName("fk_OrderPenjualan_Karyawan1_idx");

            //    entity.HasOne(d => d.Customer)
            //        .WithMany(p => p.Orderpenjualan)
            //        .HasForeignKey(d => d.CustomerId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_OrderPenjualan_Customer1");

            //    entity.HasOne(d => d.Sales)
            //        .WithMany(p => p.Orderpenjualan)
            //        .HasForeignKey(d => d.SalesId)
            //        .HasConstraintName("fk_OrderPenjualan_Karyawan1");
            //});

            //modelBuilder.Entity<OrderPenjualanItem>(entity =>
            //{
            //    //entity.ToTable("orderpenjualanitem");

            //    entity.HasIndex(e => e.OrderPenjualanId)
            //        .HasName("fk_OrderPenjualanItem_OrderPenjualan1_idx");

            //    entity.HasIndex(e => e.ProductId)
            //        .HasName("fk_OrderPenjualanItem_Product1_idx");

            //    entity.HasIndex(e => e.UnitId)
            //        .HasName("fk_PenjualanItem_Unit1_idx");

            //    entity.HasOne(d => d.OrderPenjualan)
            //        .WithMany(p => p.Items)
            //        .HasForeignKey(d => d.OrderPenjualanId)
            //        .OnDelete(DeleteBehavior.Cascade)
            //        .HasConstraintName("fk_OrderPenjualanItem_OrderPenjualan1");

            //    entity.HasOne(d => d.Product)
            //        .WithMany(p => p.Orderpenjualanitem)
            //        .HasForeignKey(d => d.ProductId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_OrderPenjualanItem_Product1");

            //    //entity.HasOne(d => d.Unit)
            //    //    .WithMany(p => p.OrderPenjualanItem)
            //    //    .HasForeignKey(d => d.UnitId)
            //    //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    //    .HasConstraintName("fk_PenjualanItem_Unit1");
            //});

            //modelBuilder.Entity<Pembayaranpembelian>(entity =>
            //{
            //    //entity.ToTable("pembayaranpembelian");

            //    entity.HasIndex(e => e.PembelianId)
            //        .HasName("fk_PembayaranPembelian_Pembelian1_idx");

            //    entity.Property(e => e.Description).HasColumnType("longtext");

            //    entity.HasOne(d => d.Pembelian)
            //        .WithMany(p => p.Pembayaranpembelian)
            //        .HasForeignKey(d => d.PembelianId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_PembayaranPembelian_Pembelian1");
            //});

            //modelBuilder.Entity<Pembayaranpenjualan>(entity =>
            //{
            //    //entity.ToTable("pembayaranpenjualan");

            //    entity.HasIndex(e => e.PenjualanId)
            //        .HasName("fk_PembayaranPenjualan_Penjualan1_idx");

            //    entity.Property(e => e.Description).HasColumnType("longtext");

            //    entity.HasOne(d => d.Penjualan)
            //        .WithMany(p => p.Pembayaranpenjualan)
            //        .HasForeignKey(d => d.PenjualanId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_PembayaranPenjualan_Penjualan1");
            //});

            //modelBuilder.Entity<Pembelian>(entity =>
            //{
            //    //entity.HasIndex(e => e.OrderPembelianId)
            //    //    .HasName("index3")
            //    //    .IsUnique();

            //    entity.HasOne(d => d.OrderPembelian)
            //        .WithOne(p => p.Pembelian)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_Pembelian_OrderPembelian1");
            //});

            //modelBuilder.Entity<PembelianItem>(entity =>
            //{
            //    //entity.ToTable("pembelianitem");

            //    entity.HasIndex(e => e.PembelianId)
            //        .HasName("fk_PembelianItem_Pembelian1_idx");

            //    entity.HasIndex(e => e.ProductId)
            //        .HasName("fk_OrderPembelianItem_Product1_idx");

            //    entity.HasIndex(e => e.UnitId)
            //        .HasName("fk_PembelianItem_Unit1_idx");

            //    entity.HasOne(d => d.Pembelian)
            //        .WithMany(p => p.Items)
            //        .HasForeignKey(d => d.PembelianId)
            //        .HasConstraintName("fk_PembelianItem_Pembelian1");

            //    entity.HasOne(d => d.Product)
            //        .WithMany(p => p.PembelianItem)
            //        .HasForeignKey(d => d.ProductId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_OrderPembelianItem_Product10");

            //    //entity.HasOne(d => d.Unit)
            //    //    .WithMany(p => p.PembelianItem)
            //    //    .HasForeignKey(d => d.UnitId)
            //    //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    //    .HasConstraintName("fk_PembelianItem_Unit10");
            //});

            //modelBuilder.Entity<Penjualan>(entity =>
            //{
            //    //entity.ToTable("penjualan");

            //    entity.HasIndex(e => e.OrderPenjualanId)
            //        .HasName("index3")
            //        .IsUnique();

            //    entity.HasOne(d => d.OrderPenjualan)
            //        .WithOne(p => p.Penjualan)
            //        .HasForeignKey<Penjualan>(d => d.OrderPenjualanId)
            //        .OnDelete(DeleteBehavior.Cascade)
            //        .HasConstraintName("fk_Penjualan_OrderPenjualan1");
            //});

            //modelBuilder.Entity<Penjualanitem>(entity =>
            //{
            //    //entity.ToTable("penjualanitem");

            //    entity.HasIndex(e => e.PenjualanId)
            //        .HasName("fk_PenjualanItem_Penjualan1_idx");

            //    entity.HasIndex(e => e.ProductId)
            //        .HasName("fk_OrderPembelianItem_Product1_idx");

            //    entity.HasIndex(e => e.UnitId)
            //        .HasName("fk_PembelianItem_Unit1_idx");

            //    entity.HasOne(d => d.Penjualan)
            //        .WithMany(p => p.Items)
            //        .HasForeignKey(d => d.PenjualanId)
            //        .OnDelete(DeleteBehavior.Cascade)
            //        .HasConstraintName("fk_PenjualanItem_Penjualan1");

            //    entity.HasOne(d => d.Product)
            //        .WithMany(p => p.PenjualanItem)
            //        .HasForeignKey(d => d.ProductId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_OrderPembelianItem_Product100");

            //    //entity.HasOne(d => d.Unit)
            //    //    .WithMany(p => p.Penjualanitem)
            //    //    .HasForeignKey(d => d.UnitId)
            //    //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    //    .HasConstraintName("fk_PembelianItem_Unit100");
            //});


            //modelBuilder.Entity<Product>(entity =>
            //{
            //    //entity.ToTable("product");

            //    entity.HasIndex(e => e.CategoryId)
            //        .HasName("fk_Product_Category_idx");

            //    entity.HasIndex(e => e.SupplierId)
            //        .HasName("fk_Product_Supplier1");

            //    entity.Property(e => e.CodeArticle).HasMaxLength(50);

            //    entity.Property(e => e.CodeName).HasMaxLength(200);

            //    entity.Property(e => e.Description).HasColumnType("longtext");

            //    entity.Property(e => e.Merk).HasMaxLength(100);

            //    entity.Property(e => e.Name).HasMaxLength(200);

            //    entity.Property(e => e.Size).HasMaxLength(45);

            //    entity.HasOne(d => d.Category)
            //        .WithMany(p => p.Product)
            //        .HasForeignKey(d => d.CategoryId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_Product_Category");
            //});

            //modelBuilder.Entity<ProductImage>(entity=> {
            //    entity.HasIndex(e => e.ProductId)
            //        .HasName("fk_ProductImage_Product1");
            //    entity.Property(e => e.FileName).HasMaxLength(200);
            //    entity.Property(e => e.Thumb).HasMaxLength(200);
            //    entity.Property(e => e.FileType).HasMaxLength(200);

            //});

            //modelBuilder.Entity<Role>(entity =>
            //{
            //    //entity.ToTable("role");

            //    entity.Property(e => e.Name).HasMaxLength(45);
            //});

            //modelBuilder.Entity<Supplier>(entity =>
            //{
            //    //entity.ToTable("supplier");

            //    entity.Property(e => e.Address).HasColumnType("longtext");

            //    entity.Property(e => e.ContactPerson).HasMaxLength(45);

            //    entity.Property(e => e.ContactPersonName).HasMaxLength(45);

            //    entity.Property(e => e.Email).HasMaxLength(100);

            //    entity.Property(e => e.Nama).HasMaxLength(45);

            //    entity.Property(e => e.NPWP)
            //        .HasColumnName("NPWP")
            //        .HasMaxLength(45);

            //    entity.Property(e => e.Telepon).HasMaxLength(45);
            //});



            //modelBuilder.Entity<Unit>(entity =>
            //{
            //    //entity.ToTable("unit");

            //    entity.HasIndex(e => e.ProductId)
            //        .HasName("fk_Unit_Product1_idx");

            //    entity.HasIndex(e => new { e.ProductId, e.Level })
            //        .HasName("ProductUnitLevelIndex")
            //        .IsUnique();

            //    entity.HasIndex(e => new { e.ProductId, e.Name })
            //        .HasName("ProductUnitName")
            //        .IsUnique();

            //    entity.Property(e => e.Name).HasMaxLength(45);

            //    //entity.HasOne(d => d.Product)
            //    //    .WithMany(p => p.Units)
            //    //    .HasForeignKey(d => d.ProductId)
            //    //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    //    .HasConstraintName("fk_Unit_Product1");
            //});

            //modelBuilder.Entity<User>(entity =>
            //{
            //    //entity.ToTable("user");

            //    entity.Property(e => e.Email).HasMaxLength(45);

            //    entity.Property(e => e.PasswordHash).HasColumnType("longtext");

            //    entity.Property(e => e.UserName).HasMaxLength(45);
            //});

            //modelBuilder.Entity<Userrole>(entity =>
            //{

            //    entity.HasIndex(e => e.RoleId)
            //        .HasName("fk_UserRole_Role1_idx");

            //    entity.HasIndex(e => e.UserId)
            //        .HasName("fk_UserRole_User1_idx");

            //    entity.HasOne(d => d.Role)
            //        .WithMany()
            //        .HasForeignKey(d => d.RoleId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_UserRole_Role1");

            //    entity.HasOne(d => d.User)
            //        .WithMany()
            //        .HasForeignKey(d => d.UserId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_UserRole_User1");
            //});

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);




    }



    public static class AppDbContextExt
    {
        public static void DisplayTrackedEntities(this ChangeTracker changeTracker)
        {
            var entries = changeTracker.Entries();
            foreach (var entry in entries)
            {
                Debug.WriteLine($"Entity Name: {entry.Entity.GetType().FullName}");
                Debug.WriteLine($"Status: {entry.State}");
            }
        }
    }

}
