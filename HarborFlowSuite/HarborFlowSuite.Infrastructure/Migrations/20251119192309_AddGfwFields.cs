using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HarborFlowSuite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGfwFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Callsign",
                table: "GfwMetadataCache",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Geartype",
                table: "GfwMetadataCache",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipName",
                table: "GfwMetadataCache",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Callsign",
                table: "GfwMetadataCache");

            migrationBuilder.DropColumn(
                name: "Geartype",
                table: "GfwMetadataCache");

            migrationBuilder.DropColumn(
                name: "ShipName",
                table: "GfwMetadataCache");
        }
    }
}
