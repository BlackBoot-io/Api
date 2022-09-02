using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class CompleteDataModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings",
                schema: "Base");

            migrationBuilder.DropColumn(
                name: "Network",
                schema: "Payment",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AvailableNetworks",
                schema: "Base",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "Price",
                schema: "Base",
                table: "Pricings");

            migrationBuilder.RenameColumn(
                name: "GasFee",
                schema: "User",
                table: "Drops",
                newName: "Wages");

            migrationBuilder.AddColumn<byte>(
                name: "VerificationType",
                schema: "Auth",
                table: "VerificationCodes",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountUsdtAmount",
                schema: "Payment",
                table: "Transactions",
                type: "decimal(21,9)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                schema: "Payment",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalEndFee",
                schema: "Payment",
                table: "Transactions",
                type: "decimal(21,9)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DiscountForYearlySubscription",
                schema: "Base",
                table: "Pricings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestsPerDay",
                schema: "Base",
                table: "Pricings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TokenPerDay",
                schema: "Base",
                table: "Pricings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsdtAmount",
                schema: "Base",
                table: "Pricings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SmartContractAddress",
                schema: "Base",
                table: "Networks",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Coin = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Discount = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentMethodId",
                schema: "Payment",
                table: "Transactions",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_PaymentMethods_PaymentMethodId",
                schema: "Payment",
                table: "Transactions",
                column: "PaymentMethodId",
                principalSchema: "Base",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_PaymentMethods_PaymentMethodId",
                schema: "Payment",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "PaymentMethods",
                schema: "Base");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PaymentMethodId",
                schema: "Payment",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "VerificationType",
                schema: "Auth",
                table: "VerificationCodes");

            migrationBuilder.DropColumn(
                name: "DiscountUsdtAmount",
                schema: "Payment",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                schema: "Payment",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TotalEndFee",
                schema: "Payment",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DiscountForYearlySubscription",
                schema: "Base",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "RequestsPerDay",
                schema: "Base",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "TokenPerDay",
                schema: "Base",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "UsdtAmount",
                schema: "Base",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "SmartContractAddress",
                schema: "Base",
                table: "Networks");

            migrationBuilder.RenameColumn(
                name: "Wages",
                schema: "User",
                table: "Drops",
                newName: "GasFee");

            migrationBuilder.AddColumn<byte>(
                name: "Network",
                schema: "Payment",
                table: "Transactions",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "AvailableNetworks",
                schema: "Base",
                table: "Pricings",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "Base",
                table: "Pricings",
                type: "decimal(21,9)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscountForYearlySubscription = table.Column<int>(type: "int", nullable: false),
                    NetworkForPayment = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }
    }
}
