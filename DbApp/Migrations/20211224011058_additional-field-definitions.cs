using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbApp.Migrations
{
    public partial class additionalfielddefinitions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LineItemsDefinitions",
                table: "LineItemsDefinitions");

            migrationBuilder.DropColumn(
                name: "DataSource",
                table: "AdditionalFields");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AdditionalFields");

            migrationBuilder.RenameTable(
                name: "LineItemsDefinitions",
                newName: "LineItemDefinitions");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldId",
                table: "AdditionalFields",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SourceId",
                table: "AdditionalFields",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "AdditionalFields",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineItemDefinitions",
                table: "LineItemDefinitions",
                column: "Id");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionalFieldDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LineItemDefinitions",
                table: "LineItemDefinitions");

            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "AdditionalFields");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "AdditionalFields");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "AdditionalFields");

            migrationBuilder.RenameTable(
                name: "LineItemDefinitions",
                newName: "LineItemsDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "DataSource",
                table: "AdditionalFields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AdditionalFields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineItemsDefinitions",
                table: "LineItemsDefinitions",
                column: "Id");
        }
    }
}
