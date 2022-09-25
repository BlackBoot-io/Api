using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class AddUserIdInAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Transactions_TransactionId",
                schema: "User",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionId",
                schema: "User",
                table: "Subscriptions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "File",
                table: "Attachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_UserId",
                schema: "File",
                table: "Attachments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Users_UserId",
                schema: "File",
                table: "Attachments",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Transactions_TransactionId",
                schema: "User",
                table: "Subscriptions",
                column: "TransactionId",
                principalSchema: "Payment",
                principalTable: "Transactions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Users_UserId",
                schema: "File",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Transactions_TransactionId",
                schema: "User",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_UserId",
                schema: "File",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "File",
                table: "Attachments");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionId",
                schema: "User",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Transactions_TransactionId",
                schema: "User",
                table: "Subscriptions",
                column: "TransactionId",
                principalSchema: "Payment",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
