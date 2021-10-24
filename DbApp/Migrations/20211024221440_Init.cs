using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers_Individual",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    CustomerId = table.Column<string>(nullable: false),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    MobilePhoneNumber = table.Column<string>(nullable: true),
                    Address_Number = table.Column<string>(nullable: true),
                    Address_Street = table.Column<string>(nullable: true),
                    Address_ZipCode = table.Column<string>(nullable: true),
                    Address_City = table.Column<string>(nullable: true),
                    Address_Country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers_Individual", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtractionSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DataSource = table.Column<string>(nullable: false),
                    Field = table.Column<string>(nullable: false),
                    X = table.Column<int>(nullable: false),
                    Y = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractionSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    LockedBy = table.Column<string>(nullable: true),
                    FileId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    FileSize = table.Column<long>(nullable: false),
                    ShouldBeExtracted = table.Column<bool>(nullable: false, defaultValue: true),
                    IsExtracted = table.Column<bool>(nullable: false, defaultValue: false),
                    ExtractId = table.Column<Guid>(nullable: false),
                    ExtractDateTime = table.Column<DateTime>(nullable: false),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Login = table.Column<string>(maxLength: 512, nullable: false),
                    Email = table.Column<string>(maxLength: 512, nullable: false),
                    Password = table.Column<string>(maxLength: 512, nullable: false),
                    LastLoginDateTime = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers_Individual");

            migrationBuilder.DropTable(
                name: "ExtractionSettings");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
