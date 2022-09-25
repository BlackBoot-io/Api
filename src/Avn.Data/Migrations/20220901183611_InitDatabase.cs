using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avn.Data.Migrations
{
    public partial class InitDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Base");

            migrationBuilder.EnsureSchema(
                name: "Auth");

            migrationBuilder.EnsureSchema(
                name: "Payment");

            migrationBuilder.CreateTable(
                name: "Networks",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NetworkType = table.Column<byte>(type: "tinyint", nullable: false),
                    GasFee = table.Column<decimal>(type: "decimal(21,9)", nullable: false),
                    Wages = table.Column<decimal>(type: "decimal(21,9)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pricings",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    AvailableNetworks = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(21,9)", nullable: false),
                    RequestsPerSecond = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Auth",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    EmailIsApproved = table.Column<bool>(type: "bit", nullable: false),
                    Password = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    PasswordSalt = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    UserType = table.Column<byte>(type: "tinyint", nullable: false),
                    WalletAdress = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SourceIp = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    ApiKey = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Website = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Network = table.Column<byte>(type: "tinyint", nullable: false),
                    UsdtAmount = table.Column<int>(type: "int", nullable: false),
                    CryptoAmount = table.Column<decimal>(type: "decimal(21,9)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    ConfirmDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WalletAddress = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    TxId = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserJwtTokens",
                schema: "Auth",
                columns: table => new
                {
                    UserJwtTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessTokenHash = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    AccessTokenExpiresTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RefreshTokenHash = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    RefreshTokenExpiresTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJwtTokens", x => x.UserJwtTokenId);
                    table.ForeignKey(
                        name: "FK_UserJwtTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationCodes",
                schema: "Auth",
                columns: table => new
                {
                    VerificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PinCode = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    InsertDateMi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCodes", x => x.VerificationId);
                    table.ForeignKey(
                        name: "FK_VerificationCodes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Drops",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NetworkId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    DeliveryType = table.Column<byte>(type: "tinyint", nullable: false),
                    DropStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    DropUri = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVirtual = table.Column<bool>(type: "bit", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GasFee = table.Column<decimal>(type: "decimal(21,9)", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drops_Networks_NetworkId",
                        column: x => x.NetworkId,
                        principalSchema: "Base",
                        principalTable: "Networks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Drops_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Base",
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Drops_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PricingId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    To = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Pricings_PricingId",
                        column: x => x.PricingId,
                        principalSchema: "Base",
                        principalTable: "Pricings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalSchema: "Payment",
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "Base",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DropId = table.Column<int>(type: "int", nullable: false),
                    ContractTokenId = table.Column<int>(type: "int", nullable: false),
                    IsMinted = table.Column<bool>(type: "bit", nullable: false),
                    IsBurned = table.Column<bool>(type: "bit", nullable: false),
                    OwerWalletAddress = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    UniqueCode = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Drops_DropId",
                        column: x => x.DropId,
                        principalSchema: "Base",
                        principalTable: "Drops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drops_NetworkId",
                schema: "Base",
                table: "Drops",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_Drops_ProjectId",
                schema: "Base",
                table: "Drops",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Drops_UserId",
                schema: "Base",
                table: "Drops",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                schema: "Base",
                table: "Projects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PricingId",
                schema: "Auth",
                table: "Subscriptions",
                column: "PricingId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_TransactionId",
                schema: "Auth",
                table: "Subscriptions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                schema: "Auth",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_DropId",
                schema: "Base",
                table: "Tokens",
                column: "DropId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                schema: "Payment",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJwtTokens_UserId",
                schema: "Auth",
                table: "UserJwtTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCodes_UserId",
                schema: "Auth",
                table: "VerificationCodes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "UserJwtTokens",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "VerificationCodes",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "Pricings",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "Payment");

            migrationBuilder.DropTable(
                name: "Drops",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "Networks",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "Base");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Auth");
        }
    }
}
