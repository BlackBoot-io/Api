using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class ChangeUserType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserType",
                schema: "Auth",
                table: "Users",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "NetworkType",
                schema: "Base",
                table: "Networks",
                newName: "Type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "Auth",
                table: "Users",
                newName: "UserType");

            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "Base",
                table: "Networks",
                newName: "NetworkType");
        }
    }
}
