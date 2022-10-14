using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class AddPricing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "NetworkInPricings",
                newName: "NetworkInPricings",
                newSchema: "Base");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "NetworkInPricings",
                schema: "Base",
                newName: "NetworkInPricings");
        }
    }
}
