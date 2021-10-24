using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class IExtractable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExtractDateTime",
                table: "Invoices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ExtractId",
                table: "Invoices",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtractDateTime",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ExtractId",
                table: "Invoices");
        }
    }
}
