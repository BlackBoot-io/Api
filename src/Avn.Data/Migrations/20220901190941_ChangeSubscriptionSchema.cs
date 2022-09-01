using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class ChangeSubscriptionSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Subscriptions",
                schema: "Auth",
                newName: "Subscriptions",
                newSchema: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Subscriptions",
                schema: "User",
                newName: "Subscriptions",
                newSchema: "Auth");
        }
    }
}
