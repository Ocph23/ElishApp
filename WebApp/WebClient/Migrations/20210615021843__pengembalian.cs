using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebClient.Migrations
{
    public partial class _pengembalian : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PengembalianItem");

            migrationBuilder.DropTable(
                name: "Pengembalian");

            migrationBuilder.CreateTable(
                name: "PengembalianPenjualan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    GudangId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PengembalianPenjualan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualan_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualan_Gudang_GudangId",
                        column: x => x.GudangId,
                        principalTable: "Gudang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PengembalianPenjualanItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<double>(type: "double", nullable: false),
                    Price = table.Column<double>(type: "double", nullable: false),
                    Discount = table.Column<double>(type: "double", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    PenjualanId = table.Column<int>(type: "int", nullable: true),
                    PengembalianPenjualanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PengembalianPenjualanItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualanItem_PengembalianPenjualan_Pengembalian~",
                        column: x => x.PengembalianPenjualanId,
                        principalTable: "PengembalianPenjualan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualanItem_Penjualan_PenjualanId",
                        column: x => x.PenjualanId,
                        principalTable: "Penjualan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualanItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PengembalianPenjualanItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PengembalianPenjualanItem");

            migrationBuilder.DropTable(
                name: "PengembalianPenjualan");

            migrationBuilder.CreateTable(
                name: "Pengembalian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    GudangId = table.Column<int>(type: "int", nullable: true),
                    PembelianId = table.Column<int>(type: "int", nullable: true),
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
                name: "PengembalianItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(type: "double", nullable: false),
                    Discount = table.Column<double>(type: "double", nullable: false),
                    PengembalianId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<double>(type: "double", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true)
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
        }
    }
}
