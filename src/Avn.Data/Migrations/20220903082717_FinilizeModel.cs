using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class FinilizeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PriorityTicketsSupport",
                schema: "Base",
                table: "Pricings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PublicDocumentation",
                schema: "Base",
                table: "Pricings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TicketsSupport",
                schema: "Base",
                table: "Pricings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Count",
                schema: "User",
                table: "Drops",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NetworkInPricings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NetworkId = table.Column<int>(type: "int", nullable: false),
                    PricingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkInPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkInPricings_Networks_NetworkId",
                        column: x => x.NetworkId,
                        principalSchema: "Base",
                        principalTable: "Networks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NetworkInPricings_Pricings_PricingId",
                        column: x => x.PricingId,
                        principalSchema: "Base",
                        principalTable: "Pricings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethodInPricings",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    PricingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodInPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethodInPricings_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "Base",
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentMethodInPricings_Pricings_PricingId",
                        column: x => x.PricingId,
                        principalSchema: "Base",
                        principalTable: "Pricings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NetworkInPricings_NetworkId",
                table: "NetworkInPricings",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkInPricings_PricingId",
                table: "NetworkInPricings",
                column: "PricingId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodInPricings_PaymentMethodId",
                schema: "Base",
                table: "PaymentMethodInPricings",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodInPricings_PricingId",
                schema: "Base",
                table: "PaymentMethodInPricings",
                column: "PricingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NetworkInPricings");

            migrationBuilder.DropTable(
                name: "PaymentMethodInPricings",
                schema: "Base");

            migrationBuilder.DropColumn(
                name: "PriorityTicketsSupport",
                schema: "Base",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "PublicDocumentation",
                schema: "Base",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "TicketsSupport",
                schema: "Base",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "Count",
                schema: "User",
                table: "Drops");
        }
    }
}
