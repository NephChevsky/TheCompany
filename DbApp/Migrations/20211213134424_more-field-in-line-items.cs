using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class morefieldinlineitems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "LineItemsDefinitions");

            migrationBuilder.AddColumn<double>(
                name: "PriceNoVAT",
                table: "LineItemsDefinitions",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceVAT",
                table: "LineItemsDefinitions",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "LineItemsDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VAT",
                table: "LineItemsDefinitions",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceNoVAT",
                table: "LineItemsDefinitions");

            migrationBuilder.DropColumn(
                name: "PriceVAT",
                table: "LineItemsDefinitions");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "LineItemsDefinitions");

            migrationBuilder.DropColumn(
                name: "VAT",
                table: "LineItemsDefinitions");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "LineItemsDefinitions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
