using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class ChangeOwnerAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "File",
                table: "Attachments");

            migrationBuilder.RenameColumn(
                name: "VerificationType",
                schema: "Auth",
                table: "VerificationCodes",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "OwerWalletAddress",
                schema: "User",
                table: "Tokens",
                newName: "OwnerWalletAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "Auth",
                table: "VerificationCodes",
                newName: "VerificationType");

            migrationBuilder.RenameColumn(
                name: "OwnerWalletAddress",
                schema: "User",
                table: "Tokens",
                newName: "OwerWalletAddress");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "File",
                table: "Attachments",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
