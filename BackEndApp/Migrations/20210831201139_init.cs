using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEndApp.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    LastLoginDateTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "Users");
        }
    }
}
