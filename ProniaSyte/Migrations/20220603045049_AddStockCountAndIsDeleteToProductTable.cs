using Microsoft.EntityFrameworkCore.Migrations;

namespace ProniaSyte.Migrations
{
    public partial class AddStockCountAndIsDeleteToProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StockCount",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StockCount",
                table: "Products");
        }
    }
}
