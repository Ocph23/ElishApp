using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApsWebApp.Migrations
{
    /// <inheritdoc />
    public partial class _Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gudang",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gudang", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Merk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merk", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nama = table.Column<string>(type: "text", nullable: true),
                    ContactPerson = table.Column<string>(type: "text", nullable: true),
                    ContactPersonName = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Telepon = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NPWP = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    Activated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pemindahan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DariId = table.Column<int>(type: "integer", nullable: true),
                    TujuanId = table.Column<int>(type: "integer", nullable: true),
                    WaktuPemindahan = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pemindahan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pemindahan_Gudang_DariId",
                        column: x => x.DariId,
                        principalTable: "Gudang",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pemindahan_Gudang_TujuanId",
                        column: x => x.TujuanId,
                        principalTable: "Gudang",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderPembelian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Discription = table.Column<string>(type: "text", nullable: true),
                    SupplierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPembelian", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPembelian_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CodeName = table.Column<string>(type: "text", nullable: true),
                    CodeArticle = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Discount = table.Column<double>(type: "double precision", nullable: false),
                    MerkId = table.Column<int>(type: "integer", nullable: true),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_Merk_MerkId",
                        column: x => x.MerkId,
                        principalTable: "Merk",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Karyawan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Telepon = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Karyawan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Karyawan_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Userrole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Userrole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Userrole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Userrole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pembelian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderPembelianId = table.Column<int>(type: "integer", nullable: false),
                    DeadLine = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GudangId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pembelian", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pembelian_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pembelian_OrderPembelian_OrderPembelianId",
                        column: x => x.OrderPembelianId,
                        principalTable: "OrderPembelian",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    Thumb = table.Column<string>(type: "text", nullable: true),
                    FileType = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GudangId = table.Column<int>(type: "integer", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    Quntity = table.Column<double>(type: "double precision", nullable: false),
                    StockMovementType = table.Column<int>(type: "integer", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: false),
                    ReferenceType = table.Column<int>(type: "integer", nullable: false),
                    MovementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GudangId = table.Column<int>(type: "integer", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    Quntity = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Stocks_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Buy = table.Column<double>(type: "double precision", nullable: false),
                    Sell = table.Column<double>(type: "double precision", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Unit_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    ContactName = table.Column<string>(type: "text", nullable: true),
                    Telepon = table.Column<string>(type: "text", nullable: true),
                    NPWP = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    KaryawanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customer_Karyawan_KaryawanId",
                        column: x => x.KaryawanId,
                        principalTable: "Karyawan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customer_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PembayaranPembelian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PayDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PayTo = table.Column<string>(type: "text", nullable: true),
                    PayType = table.Column<int>(type: "integer", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    RekNumber = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PayValue = table.Column<double>(type: "double precision", nullable: false),
                    PembelianId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PembayaranPembelian", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PembayaranPembelian_Pembelian_PembelianId",
                        column: x => x.PembelianId,
                        principalTable: "Pembelian",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IncomingItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActualValue = table.Column<double>(type: "double precision", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    PembelianId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomingItem_Pembelian_PembelianId",
                        column: x => x.PembelianId,
                        principalTable: "Pembelian",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncomingItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncomingItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderPembelianItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quntity = table.Column<double>(type: "double precision", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Keterangan = table.Column<string>(type: "text", nullable: true),
                    Discount = table.Column<double>(type: "double precision", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    OrderPembelianId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPembelianItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPembelianItem_OrderPembelian_OrderPembelianId",
                        column: x => x.OrderPembelianId,
                        principalTable: "OrderPembelian",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderPembelianItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderPembelianItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PembelianItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    Discount = table.Column<double>(type: "double precision", nullable: false),
                    PembelianId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PembelianItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PembelianItem_Pembelian_PembelianId",
                        column: x => x.PembelianId,
                        principalTable: "Pembelian",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PembelianItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PembelianItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PemindahanItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    PemindahanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PemindahanItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PemindahanItem_Pemindahan_PemindahanId",
                        column: x => x.PemindahanId,
                        principalTable: "Pemindahan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PemindahanItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PemindahanItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderPenjualan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeadLine = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Discription = table.Column<string>(type: "text", nullable: true),
                    GudangId = table.Column<int>(type: "integer", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    SalesId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPenjualan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPenjualan_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderPenjualan_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderPenjualan_Karyawan_SalesId",
                        column: x => x.SalesId,
                        principalTable: "Karyawan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PengembalianPenjualan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    GudangId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PengembalianPenjualan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualan_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualan_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderPenjualanItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Discount = table.Column<double>(type: "double precision", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    OrderPenjualanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPenjualanItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPenjualanItem_OrderPenjualan_OrderPenjualanId",
                        column: x => x.OrderPenjualanId,
                        principalTable: "OrderPenjualan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderPenjualanItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderPenjualanItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Penjualan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    SalesmanId = table.Column<int>(type: "integer", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeadLine = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Activity = table.Column<int>(type: "integer", nullable: false),
                    FeeSalesman = table.Column<double>(type: "double precision", nullable: false),
                    Expedisi = table.Column<double>(type: "double precision", nullable: false),
                    GudangId = table.Column<int>(type: "integer", nullable: true),
                    OrderPenjualanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penjualan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penjualan_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Penjualan_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Penjualan_Karyawan_SalesmanId",
                        column: x => x.SalesmanId,
                        principalTable: "Karyawan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Penjualan_OrderPenjualan_OrderPenjualanId",
                        column: x => x.OrderPenjualanId,
                        principalTable: "OrderPenjualan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PembayaranPenjualan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PayDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PayTo = table.Column<string>(type: "text", nullable: true),
                    PayType = table.Column<int>(type: "integer", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    RekNumber = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PayValue = table.Column<double>(type: "double precision", nullable: false),
                    PenjualanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PembayaranPenjualan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PembayaranPenjualan_Penjualan_PenjualanId",
                        column: x => x.PenjualanId,
                        principalTable: "Penjualan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PengembalianPenjualanItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Discount = table.Column<double>(type: "double precision", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    PenjualanId = table.Column<int>(type: "integer", nullable: true),
                    PengembalianPenjualanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PengembalianPenjualanItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualanItem_PengembalianPenjualan_Pengembalia~",
                        column: x => x.PengembalianPenjualanId,
                        principalTable: "PengembalianPenjualan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualanItem_Penjualan_PenjualanId",
                        column: x => x.PenjualanId,
                        principalTable: "Penjualan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualanItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualanItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Penjualanitem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Discount = table.Column<double>(type: "double precision", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    PenjualanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penjualanitem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penjualanitem_Penjualan_PenjualanId",
                        column: x => x.PenjualanId,
                        principalTable: "Penjualan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Penjualanitem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Penjualanitem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_KaryawanId",
                table: "Customer",
                column: "KaryawanId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_UserId",
                table: "Customer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingItem_PembelianId",
                table: "IncomingItem",
                column: "PembelianId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingItem_ProductId",
                table: "IncomingItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingItem_UnitId",
                table: "IncomingItem",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Karyawan_UserId",
                table: "Karyawan",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPembelian_SupplierId",
                table: "OrderPembelian",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPembelianItem_OrderPembelianId",
                table: "OrderPembelianItem",
                column: "OrderPembelianId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPembelianItem_ProductId",
                table: "OrderPembelianItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPembelianItem_UnitId",
                table: "OrderPembelianItem",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPenjualan_CustomerId",
                table: "OrderPenjualan",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPenjualan_GudangId",
                table: "OrderPenjualan",
                column: "GudangId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPenjualan_SalesId",
                table: "OrderPenjualan",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPenjualanItem_OrderPenjualanId",
                table: "OrderPenjualanItem",
                column: "OrderPenjualanId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPenjualanItem_ProductId",
                table: "OrderPenjualanItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPenjualanItem_UnitId",
                table: "OrderPenjualanItem",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PembayaranPembelian_PembelianId",
                table: "PembayaranPembelian",
                column: "PembelianId");

            migrationBuilder.CreateIndex(
                name: "IX_PembayaranPenjualan_PenjualanId",
                table: "PembayaranPenjualan",
                column: "PenjualanId");

            migrationBuilder.CreateIndex(
                name: "IX_Pembelian_GudangId",
                table: "Pembelian",
                column: "GudangId");

            migrationBuilder.CreateIndex(
                name: "IX_Pembelian_OrderPembelianId",
                table: "Pembelian",
                column: "OrderPembelianId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PembelianItem_PembelianId",
                table: "PembelianItem",
                column: "PembelianId");

            migrationBuilder.CreateIndex(
                name: "IX_PembelianItem_ProductId",
                table: "PembelianItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PembelianItem_UnitId",
                table: "PembelianItem",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Pemindahan_DariId",
                table: "Pemindahan",
                column: "DariId");

            migrationBuilder.CreateIndex(
                name: "IX_Pemindahan_TujuanId",
                table: "Pemindahan",
                column: "TujuanId");

            migrationBuilder.CreateIndex(
                name: "IX_PemindahanItem_PemindahanId",
                table: "PemindahanItem",
                column: "PemindahanId");

            migrationBuilder.CreateIndex(
                name: "IX_PemindahanItem_ProductId",
                table: "PemindahanItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PemindahanItem_UnitId",
                table: "PemindahanItem",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PengembalianPenjualan_CustomerId",
                table: "PengembalianPenjualan",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PengembalianPenjualan_GudangId",
                table: "PengembalianPenjualan",
                column: "GudangId");

            migrationBuilder.CreateIndex(
                name: "IX_PengembalianPenjualanItem_PengembalianPenjualanId",
                table: "PengembalianPenjualanItem",
                column: "PengembalianPenjualanId");

            migrationBuilder.CreateIndex(
                name: "IX_PengembalianPenjualanItem_PenjualanId",
                table: "PengembalianPenjualanItem",
                column: "PenjualanId");

            migrationBuilder.CreateIndex(
                name: "IX_PengembalianPenjualanItem_ProductId",
                table: "PengembalianPenjualanItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PengembalianPenjualanItem_UnitId",
                table: "PengembalianPenjualanItem",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Penjualan_CustomerId",
                table: "Penjualan",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Penjualan_GudangId",
                table: "Penjualan",
                column: "GudangId");

            migrationBuilder.CreateIndex(
                name: "IX_Penjualan_OrderPenjualanId",
                table: "Penjualan",
                column: "OrderPenjualanId");

            migrationBuilder.CreateIndex(
                name: "IX_Penjualan_SalesmanId",
                table: "Penjualan",
                column: "SalesmanId");

            migrationBuilder.CreateIndex(
                name: "IX_Penjualanitem_PenjualanId",
                table: "Penjualanitem",
                column: "PenjualanId");

            migrationBuilder.CreateIndex(
                name: "IX_Penjualanitem_ProductId",
                table: "Penjualanitem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Penjualanitem_UnitId",
                table: "Penjualanitem",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_MerkId",
                table: "Product",
                column: "MerkId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SupplierId",
                table: "Product",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_GudangId",
                table: "StockMovements",
                column: "GudangId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductId",
                table: "StockMovements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_GudangId",
                table: "Stocks",
                column: "GudangId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProductId",
                table: "Stocks",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Unit_ProductId",
                table: "Unit",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Userrole_RoleId",
                table: "Userrole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Userrole_UserId",
                table: "Userrole",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingItem");

            migrationBuilder.DropTable(
                name: "OrderPembelianItem");

            migrationBuilder.DropTable(
                name: "OrderPenjualanItem");

            migrationBuilder.DropTable(
                name: "PembayaranPembelian");

            migrationBuilder.DropTable(
                name: "PembayaranPenjualan");

            migrationBuilder.DropTable(
                name: "PembelianItem");

            migrationBuilder.DropTable(
                name: "PemindahanItem");

            migrationBuilder.DropTable(
                name: "PengembalianPenjualanItem");

            migrationBuilder.DropTable(
                name: "Penjualanitem");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Userrole");

            migrationBuilder.DropTable(
                name: "Pembelian");

            migrationBuilder.DropTable(
                name: "Pemindahan");

            migrationBuilder.DropTable(
                name: "PengembalianPenjualan");

            migrationBuilder.DropTable(
                name: "Penjualan");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "OrderPembelian");

            migrationBuilder.DropTable(
                name: "OrderPenjualan");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Gudang");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Merk");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "Karyawan");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
