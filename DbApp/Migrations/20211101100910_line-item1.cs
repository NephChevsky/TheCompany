using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class lineitem1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceLineItem",
                table: "InvoiceLineItem");

            migrationBuilder.RenameTable(
                name: "InvoiceLineItem",
                newName: "InvoiceLineItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceLineItems",
                table: "InvoiceLineItems",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceLineItems",
                table: "InvoiceLineItems");

            migrationBuilder.RenameTable(
                name: "InvoiceLineItems",
                newName: "InvoiceLineItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceLineItem",
                table: "InvoiceLineItem",
                column: "Id");
        }
    }
}
