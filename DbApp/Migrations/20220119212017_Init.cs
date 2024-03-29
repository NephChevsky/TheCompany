﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdditionalFieldDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DataSource = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalFieldDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdditionalFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SourceId = table.Column<Guid>(nullable: false),
                    FieldId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    MobilePhoneNumber = table.Column<string>(nullable: true),
                    Siret = table.Column<string>(nullable: true),
                    Logo = table.Column<Guid>(nullable: false),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtractionSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DataSource = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
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
                name: "FilePreviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FileId = table.Column<Guid>(nullable: false),
                    Page = table.Column<int>(nullable: false),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilePreviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FilePath = table.Column<string>(nullable: false),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Individuals",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    CustomerNumber = table.Column<string>(nullable: false),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    MobilePhoneNumber = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Individuals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    RecipientNumber = table.Column<string>(nullable: true),
                    RecipientId = table.Column<Guid>(nullable: false),
                    RecipientFirstName = table.Column<string>(nullable: true),
                    RecipientLastName = table.Column<string>(nullable: true),
                    RecipientAddress = table.Column<string>(nullable: true),
                    LockedBy = table.Column<string>(nullable: true),
                    FileId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    FileSize = table.Column<long>(nullable: false),
                    ShouldBeExtracted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsExtracted = table.Column<bool>(nullable: false, defaultValue: false),
                    ShouldBeGenerated = table.Column<bool>(nullable: false, defaultValue: false),
                    IsGenerated = table.Column<bool>(nullable: false, defaultValue: false),
                    GenerationDateTime = table.Column<DateTime>(nullable: false),
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
                name: "LineItemDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Reference = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Unit = table.Column<string>(nullable: true),
                    VAT = table.Column<double>(nullable: true),
                    PriceNoVAT = table.Column<double>(nullable: true),
                    PriceVAT = table.Column<double>(nullable: true),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItemDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LineItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    InvoiceId = table.Column<Guid>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    Unit = table.Column<string>(nullable: true),
                    VAT = table.Column<double>(nullable: true),
                    PriceNoVAT = table.Column<double>(nullable: true),
                    PriceVAT = table.Column<double>(nullable: true),
                    TotalPrice = table.Column<double>(nullable: false),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    RecipientNumber = table.Column<string>(nullable: true),
                    RecipientId = table.Column<Guid>(nullable: false),
                    RecipientFirstName = table.Column<string>(nullable: true),
                    RecipientLastName = table.Column<string>(nullable: true),
                    RecipientAddress = table.Column<string>(nullable: true),
                    LockedBy = table.Column<string>(nullable: true),
                    FileId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    FileSize = table.Column<long>(nullable: false),
                    ShouldBeExtracted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsExtracted = table.Column<bool>(nullable: false, defaultValue: false),
                    ShouldBeGenerated = table.Column<bool>(nullable: false, defaultValue: false),
                    IsGenerated = table.Column<bool>(nullable: false, defaultValue: false),
                    GenerationDateTime = table.Column<DateTime>(nullable: false),
                    ExtractId = table.Column<Guid>(nullable: false),
                    ExtractDateTime = table.Column<DateTime>(nullable: false),
                    Owner = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
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
                name: "AdditionalFieldDefinitions");

            migrationBuilder.DropTable(
                name: "AdditionalFields");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "ExtractionSettings");

            migrationBuilder.DropTable(
                name: "FilePreviews");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Individuals");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "LineItemDefinitions");

            migrationBuilder.DropTable(
                name: "LineItems");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
