using Microsoft.EntityFrameworkCore.Migrations;

namespace ApsWebApp.Migrations
{
    public partial class _changePembayaran : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PembayaranPenjualan_Penjualan_PenjualanId",
                table: "PembayaranPenjualan");

            migrationBuilder.AlterColumn<int>(
                name: "PenjualanId",
                table: "PembayaranPenjualan",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PembayaranPenjualan_Penjualan_PenjualanId",
                table: "PembayaranPenjualan",
                column: "PenjualanId",
                principalTable: "Penjualan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PembayaranPenjualan_Penjualan_PenjualanId",
                table: "PembayaranPenjualan");

            migrationBuilder.AlterColumn<int>(
                name: "PenjualanId",
                table: "PembayaranPenjualan",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PembayaranPenjualan_Penjualan_PenjualanId",
                table: "PembayaranPenjualan",
                column: "PenjualanId",
                principalTable: "Penjualan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
