using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class addgeneratable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GenerationDateTime",
                table: "Invoices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenerationDateTime",
                table: "Invoices");
        }
    }
}
