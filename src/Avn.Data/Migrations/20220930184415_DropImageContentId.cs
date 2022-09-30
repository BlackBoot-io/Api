using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class DropImageContentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentId",
                schema: "User",
                table: "Drops",
                newName: "ImageContentId");

            migrationBuilder.RenameColumn(
                name: "DropUri",
                schema: "User",
                table: "Drops",
                newName: "DropContentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageContentId",
                schema: "User",
                table: "Drops",
                newName: "ContentId");

            migrationBuilder.RenameColumn(
                name: "DropContentId",
                schema: "User",
                table: "Drops",
                newName: "DropUri");
        }
    }
}
