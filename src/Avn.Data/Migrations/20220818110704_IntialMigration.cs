using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class IntialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Base");

            migrationBuilder.CreateTable(
                name: "Network",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NetworkType = table.Column<byte>(type: "tinyint", nullable: false),
                    GasFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Network", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "Base",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    EmailIsApproved = table.Column<bool>(type: "bit", nullable: false),
                    Password = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    PasswordSalt = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    UserType = table.Column<byte>(type: "tinyint", nullable: false),
                    OrganizationName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    WalletAdress = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SourceIp = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Website = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ApiKey = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Base",
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserJwtToken",
                schema: "Base",
                columns: table => new
                {
                    UserJwtTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccessTokenHash = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    AccessTokenExpiresTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RefreshTokenHash = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    RefreshTokenExpiresTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJwtToken", x => x.UserJwtTokenId);
                    table.ForeignKey(
                        name: "FK_UserJwtToken_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Base",
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TemplateType = table.Column<byte>(type: "tinyint", nullable: false),
                    DeliveryType = table.Column<byte>(type: "tinyint", nullable: false),
                    NetworkId = table.Column<int>(type: "int", nullable: false),
                    EventUri = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVirtual = table.Column<bool>(type: "bit", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalSchema: "Base",
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Base",
                        principalTable: "Project",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Event_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Base",
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Token",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    TokenId = table.Column<int>(type: "int", nullable: false),
                    Mint = table.Column<bool>(type: "bit", nullable: false),
                    Burn = table.Column<bool>(type: "bit", nullable: false),
                    OwerWalletAddress = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    UniqueCode = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Token_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Base",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_NetworkId",
                schema: "Base",
                table: "Event",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_ProjectId",
                schema: "Base",
                table: "Event",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_UserId",
                schema: "Base",
                table: "Event",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_UserId",
                schema: "Base",
                table: "Project",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Token_EventId",
                schema: "Base",
                table: "Token",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJwtToken_UserId",
                schema: "Base",
                table: "UserJwtToken",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Token",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "UserJwtToken",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "Event",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "Network",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "Project",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "User",
                schema: "Base");
        }
    }
}
