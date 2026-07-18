using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syc_pm_server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedVaultSystemToAccessTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PwEntries_Vaults_VaultId",
                table: "PwEntries");

            migrationBuilder.DropTable(
                name: "VaultMembers");

            migrationBuilder.DropTable(
                name: "Vaults");

            migrationBuilder.DropIndex(
                name: "IX_PwEntries_VaultId",
                table: "PwEntries");

            migrationBuilder.DropColumn(
                name: "VaultId",
                table: "PwEntries");

            migrationBuilder.CreateTable(
                name: "PwEntryAccesses",
                columns: table => new
                {
                    PwEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncryptedEntryKey = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PwEntryAccesses", x => new { x.PwEntryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PwEntryAccesses_PwEntries_PwEntryId",
                        column: x => x.PwEntryId,
                        principalTable: "PwEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PwEntryAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PwEntryAccesses_UserId",
                table: "PwEntryAccesses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PwEntryAccesses");

            migrationBuilder.AddColumn<Guid>(
                name: "VaultId",
                table: "PwEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Vaults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaultMembers",
                columns: table => new
                {
                    VaultId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncryptedVaultKey = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultMembers", x => new { x.VaultId, x.UserId });
                    table.ForeignKey(
                        name: "FK_VaultMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VaultMembers_Vaults_VaultId",
                        column: x => x.VaultId,
                        principalTable: "Vaults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PwEntries_VaultId",
                table: "PwEntries",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultMembers_UserId",
                table: "VaultMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PwEntries_Vaults_VaultId",
                table: "PwEntries",
                column: "VaultId",
                principalTable: "Vaults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
