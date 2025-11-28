using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HarborFlowSuite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MMSI",
                table: "Vessels",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_MMSI",
                table: "Vessels",
                column: "MMSI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirebaseUid",
                table: "Users",
                column: "FirebaseUid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vessels_MMSI",
                table: "Vessels");

            migrationBuilder.DropIndex(
                name: "IX_Users_FirebaseUid",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MMSI",
                table: "Vessels");
        }
    }
}
