using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syc_pm_server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlToEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "PwEntries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "PwEntries");
        }
    }
}
