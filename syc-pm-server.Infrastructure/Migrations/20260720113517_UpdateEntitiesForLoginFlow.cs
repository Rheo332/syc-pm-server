using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syc_pm_server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntitiesForLoginFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "PwEntryAccesses");

            migrationBuilder.AddColumn<string>(
                name: "Pbkdf2Salt",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PwEntries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pbkdf2Salt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PwEntries");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "PwEntryAccesses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
