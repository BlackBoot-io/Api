using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class AddUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "Auth",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                schema: "Auth",
                table: "Users");
        }
    }
}
