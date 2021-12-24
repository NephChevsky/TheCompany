using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class renameextractionfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Field",
                table: "ExtractionSettings");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ExtractionSettings",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ExtractionSettings");

            migrationBuilder.AddColumn<string>(
                name: "Field",
                table: "ExtractionSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
