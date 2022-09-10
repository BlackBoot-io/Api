using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class AddDropAttachmentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "File");

            migrationBuilder.AddColumn<int>(
                name: "AttachmentId",
                schema: "User",
                table: "Drops",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Attachments",
                schema: "File",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drops_AttachmentId",
                schema: "User",
                table: "Drops",
                column: "AttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drops_Attachments_AttachmentId",
                schema: "User",
                table: "Drops",
                column: "AttachmentId",
                principalSchema: "File",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drops_Attachments_AttachmentId",
                schema: "User",
                table: "Drops");

            migrationBuilder.DropTable(
                name: "Attachments",
                schema: "File");

            migrationBuilder.DropIndex(
                name: "IX_Drops_AttachmentId",
                schema: "User",
                table: "Drops");

            migrationBuilder.DropColumn(
                name: "AttachmentId",
                schema: "User",
                table: "Drops");
        }
    }
}
