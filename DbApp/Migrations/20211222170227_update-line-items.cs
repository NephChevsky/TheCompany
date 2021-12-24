using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class updatelineitems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "UnitaryPrice",
                table: "LineItems");

            migrationBuilder.AddColumn<double>(
                name: "PriceNoVAT",
                table: "LineItems",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PriceVAT",
                table: "LineItems",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "LineItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "LineItems",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VAT",
                table: "LineItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceNoVAT",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "PriceVAT",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "VAT",
                table: "LineItems");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "LineItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "UnitaryPrice",
                table: "LineItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
