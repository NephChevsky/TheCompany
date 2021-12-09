using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class addgeneratable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "Invoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShouldBeGenerated",
                table: "Invoices",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ShouldBeGenerated",
                table: "Invoices");
        }
    }
}
