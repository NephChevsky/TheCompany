using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class removeuselessfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "LineItemsDefinitions");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "LineItemsDefinitions");

            migrationBuilder.DropColumn(
                name: "UnitaryPrice",
                table: "LineItemsDefinitions");

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "LineItemsDefinitions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "LineItemsDefinitions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "LineItemsDefinitions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "LineItemsDefinitions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceId",
                table: "LineItemsDefinitions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "LineItemsDefinitions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "UnitaryPrice",
                table: "LineItemsDefinitions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
