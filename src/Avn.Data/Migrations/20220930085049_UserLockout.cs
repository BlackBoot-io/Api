using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class UserLockout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLockoutEnabled",
                schema: "Auth",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEndDateUtc",
                schema: "Auth",
                table: "Users",
                type: "datetime2",  
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLockoutEnabled",
                schema: "Auth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LockoutEndDateUtc",
                schema: "Auth",
                table: "Users");
        }
    }
}
