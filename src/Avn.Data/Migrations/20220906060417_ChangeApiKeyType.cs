using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class ChangeApiKeyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WalletAdress",
                schema: "Auth",
                table: "Users",
                newName: "WalletAddress");

            migrationBuilder.AlterColumn<string>(
                name: "PinCode",
                schema: "Auth",
                table: "VerificationCodes",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApiKey",
                schema: "User",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WalletAddress",
                schema: "Auth",
                table: "Users",
                newName: "WalletAdress");

            migrationBuilder.AlterColumn<int>(
                name: "PinCode",
                schema: "Auth",
                table: "VerificationCodes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ApiKey",
                schema: "User",
                table: "Projects",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
