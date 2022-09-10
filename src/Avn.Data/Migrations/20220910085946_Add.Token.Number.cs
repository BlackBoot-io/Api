using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class AddTokenNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                schema: "User",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "PinCode",
                schema: "Auth",
                table: "VerificationCodes",
                newName: "UniqueCode");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                schema: "User",
                table: "Tokens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "User",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                schema: "User",
                table: "Drops",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<byte>(
                name: "CategoryType",
                schema: "User",
                table: "Drops",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "ReviewMessage",
                schema: "User",
                table: "Drops",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                schema: "User",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "User",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CategoryType",
                schema: "User",
                table: "Drops");

            migrationBuilder.DropColumn(
                name: "ReviewMessage",
                schema: "User",
                table: "Drops");

            migrationBuilder.RenameColumn(
                name: "UniqueCode",
                schema: "Auth",
                table: "VerificationCodes",
                newName: "PinCode");

            migrationBuilder.AddColumn<Guid>(
                name: "ApiKey",
                schema: "User",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                schema: "User",
                table: "Drops",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
