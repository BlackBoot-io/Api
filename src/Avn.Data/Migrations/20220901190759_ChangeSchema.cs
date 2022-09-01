using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class ChangeSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "User");

            migrationBuilder.RenameTable(
                name: "Tokens",
                schema: "Base",
                newName: "Tokens",
                newSchema: "User");

            migrationBuilder.RenameTable(
                name: "Projects",
                schema: "Base",
                newName: "Projects",
                newSchema: "User");

            migrationBuilder.RenameTable(
                name: "Drops",
                schema: "Base",
                newName: "Drops",
                newSchema: "User");

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NetworkForPayment = table.Column<byte>(type: "tinyint", nullable: false),
                    DiscountForYearlySubscription = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings",
                schema: "Base");

            migrationBuilder.RenameTable(
                name: "Tokens",
                schema: "User",
                newName: "Tokens",
                newSchema: "Base");

            migrationBuilder.RenameTable(
                name: "Projects",
                schema: "User",
                newName: "Projects",
                newSchema: "Base");

            migrationBuilder.RenameTable(
                name: "Drops",
                schema: "User",
                newName: "Drops",
                newSchema: "Base");
        }
    }
}
