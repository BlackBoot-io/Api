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

            migrationBuilder.EnsureSchema(
                name: "Admin");

            migrationBuilder.CreateTable(
                name: "Networks",
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
                    table.PrimaryKey("PK_Networks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Admin",
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
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
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
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Admin",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserJwtTokens",
                schema: "Admin",
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
                    table.PrimaryKey("PK_UserJwtTokens", x => x.UserJwtTokenId);
                    table.ForeignKey(
                        name: "FK_UserJwtTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Admin",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Events",
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
                    DropStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    NetworkId = table.Column<int>(type: "int", nullable: false),
                    EventUri = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVirtual = table.Column<bool>(type: "bit", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Networks_NetworkId",
                        column: x => x.NetworkId,
                        principalSchema: "Base",
                        principalTable: "Networks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Base",
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Events_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Admin",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    ContractTokenId = table.Column<int>(type: "int", nullable: false),
                    Mint = table.Column<bool>(type: "bit", nullable: false),
                    Burn = table.Column<bool>(type: "bit", nullable: false),
                    OwerWalletAddress = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    UniqueCode = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "Base",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_NetworkId",
                schema: "Base",
                table: "Events",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ProjectId",
                schema: "Base",
                table: "Events",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId",
                schema: "Base",
                table: "Events",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                schema: "Base",
                table: "Projects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_EventId",
                schema: "Base",
                table: "Tokens",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJwtTokens_UserId",
                schema: "Admin",
                table: "UserJwtTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "UserJwtTokens",
                schema: "Admin");

            migrationBuilder.DropTable(
                name: "Events",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "Networks",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Admin");
        }
    }
}
