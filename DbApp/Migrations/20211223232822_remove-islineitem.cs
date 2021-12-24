using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class removeislineitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLineItem",
                table: "ExtractionSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLineItem",
                table: "ExtractionSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
