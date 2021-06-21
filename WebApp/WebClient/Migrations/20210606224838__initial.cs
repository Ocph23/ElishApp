using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebClient.Migrations
{
    public partial class _initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gudang",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gudang", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Merk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merk", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nama = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ContactPerson = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ContactPersonName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Address = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Telepon = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Email = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    NPWP = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Email = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PasswordHash = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Activated = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pemindahan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DariId = table.Column<int>(type: "int", nullable: true),
                    TujuanId = table.Column<int>(type: "int", nullable: true),
                    WaktuPemindahan = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pemindahan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pemindahan_Gudang_DariId",
                        column: x => x.DariId,
                        principalTable: "Gudang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pemindahan_Gudang_TujuanId",
                        column: x => x.TujuanId,
                        principalTable: "Gudang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderPembelian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Discription = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPembelian", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPembelian_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    CodeName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    CodeArticle = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Size = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Color = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Discount = table.Column<double>(type: "double", nullable: false),
                    MerkId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Merk_MerkId",
                        column: x => x.MerkId,
                        principalTable: "Merk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Karyawan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Telepon = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Address = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Email = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Karyawan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Karyawan_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Userrole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Userrole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Userrole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Userrole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pembelian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderPembelianId = table.Column<int>(type: "int", nullable: false),
                    DeadLine = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GudangId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pembelian", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pembelian_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Thumb = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "double", nullable: false),
                    Buy = table.Column<double>(type: "double", nullable: false),
                    Sell = table.Column<double>(type: "double", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Unit_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Email = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ContactName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Telepon = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    NPWP = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Address = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Location = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    KaryawanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customer_Karyawan_KaryawanId",
                        column: x => x.KaryawanId,
                        principalTable: "Karyawan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customer_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PembayaranPembelian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PayTo = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PayType = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    RekNumber = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PayValue = table.Column<double>(type: "double", nullable: false),
                    PembelianId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PembayaranPembelian", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PembayaranPembelian_Pembelian_PembelianId",
                        column: x => x.PembelianId,
                        principalTable: "Pembelian",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pengembalian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PembelianId = table.Column<int>(type: "int", nullable: true),
                    GudangId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PengembalianId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pengembalian", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pengembalian_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pengembalian_Pembelian_PembelianId",
                        column: x => x.PembelianId,
                        principalTable: "Pembelian",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pengembalian_Pengembalian_PengembalianId",
                        column: x => x.PengembalianId,
                        principalTable: "Pengembalian",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncomingItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActualValue = table.Column<double>(type: "double", nullable: false),
                    Amount = table.Column<double>(type: "double", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    PembelianId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomingItem_Pembelian_PembelianId",
                        column: x => x.PembelianId,
                        principalTable: "Pembelian",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncomingItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncomingItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderPembelianItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quntity = table.Column<double>(type: "double", nullable: false),
                    Price = table.Column<double>(type: "double", nullable: false),
                    Discount = table.Column<double>(type: "double", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    OrderPembelianId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPembelianItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPembelianItem_OrderPembelian_OrderPembelianId",
                        column: x => x.OrderPembelianId,
                        principalTable: "OrderPembelian",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPembelianItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPembelianItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PembelianItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(type: "double", nullable: false),
                    Price = table.Column<double>(type: "double", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Discount = table.Column<double>(type: "double", nullable: false),
                    PembelianId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PembelianItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PembelianItem_Pembelian_PembelianId",
                        column: x => x.PembelianId,
                        principalTable: "Pembelian",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PembelianItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PembelianItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PemindahanItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<double>(type: "double", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    PemindahanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PemindahanItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PemindahanItem_Pemindahan_PemindahanId",
                        column: x => x.PemindahanId,
                        principalTable: "Pemindahan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PemindahanItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PemindahanItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderPenjualan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeadLine = table.Column<double>(type: "double", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Discription = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    GudangId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    SalesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPenjualan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPenjualan_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPenjualan_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPenjualan_Karyawan_SalesId",
                        column: x => x.SalesId,
                        principalTable: "Karyawan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PengembalianItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(type: "double", nullable: false),
                    Price = table.Column<double>(type: "double", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Discount = table.Column<double>(type: "double", nullable: false),
                    PengembalianId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PengembalianItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PengembalianItem_Pengembalian_PengembalianId",
                        column: x => x.PengembalianId,
                        principalTable: "Pengembalian",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PengembalianItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PengembalianItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderPenjualanItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<double>(type: "double", nullable: false),
                    Discount = table.Column<double>(type: "double", nullable: false),
                    Price = table.Column<double>(type: "double", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    OrderPenjualanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPenjualanItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPenjualanItem_OrderPenjualan_OrderPenjualanId",
                        column: x => x.OrderPenjualanId,
                        principalTable: "OrderPenjualan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPenjualanItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPenjualanItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Penjualan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    SalesmanId = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeadLine = table.Column<double>(type: "double", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Activity = table.Column<int>(type: "int", nullable: false),
                    FeeSalesman = table.Column<double>(type: "double", nullable: false),
                    Expedisi = table.Column<double>(type: "double", nullable: false),
                    GudangId = table.Column<int>(type: "int", nullable: true),
                    OrderPenjualanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penjualan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penjualan_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penjualan_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penjualan_Karyawan_SalesmanId",
                        column: x => x.SalesmanId,
                        principalTable: "Karyawan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penjualan_OrderPenjualan_OrderPenjualanId",
                        column: x => x.OrderPenjualanId,
                        principalTable: "OrderPenjualan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PembayaranPenjualan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PayTo = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PayType = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    RekNumber = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PenjualanId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PayValue = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PembayaranPenjualan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PembayaranPenjualan_Penjualan_PenjualanId",
                        column: x => x.PenjualanId,
                        principalTable: "Penjualan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Penjualanitem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<double>(type: "double", nullable: false),
                    Discount = table.Column<double>(type: "double", nullable: false),
                    Price = table.Column<double>(type: "double", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    PenjualanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penjualanitem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penjualanitem_Penjualan_PenjualanId",
                        column: x => x.PenjualanId,
                        principalTable: "Penjualan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penjualanitem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penjualanitem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_Pengembalian_GudangId",
                table: "Pengembalian",
                column: "GudangId");

            migrationBuilder.CreateIndex(
                name: "IX_Pengembalian_PembelianId",
                table: "Pengembalian",
                column: "PembelianId");

            migrationBuilder.CreateIndex(
                name: "IX_Pengembalian_PengembalianId",
                table: "Pengembalian",
                column: "PengembalianId");

            migrationBuilder.CreateIndex(
                name: "IX_PengembalianItem_PengembalianId",
                table: "PengembalianItem",
                column: "PengembalianId");

            migrationBuilder.CreateIndex(
                name: "IX_PengembalianItem_ProductId",
                table: "PengembalianItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PengembalianItem_UnitId",
                table: "PengembalianItem",
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
                name: "PengembalianItem");

            migrationBuilder.DropTable(
                name: "Penjualanitem");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "Userrole");

            migrationBuilder.DropTable(
                name: "Pemindahan");

            migrationBuilder.DropTable(
                name: "Pengembalian");

            migrationBuilder.DropTable(
                name: "Penjualan");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Pembelian");

            migrationBuilder.DropTable(
                name: "OrderPenjualan");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "OrderPembelian");

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
