using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HarborFlowSuite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGfwMetadataCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GfwMetadataCache",
                columns: table => new
                {
                    Mmsi = table.Column<string>(type: "text", nullable: false),
                    Flag = table.Column<string>(type: "text", nullable: true),
                    Length = table.Column<double>(type: "double precision", nullable: false),
                    ImoNumber = table.Column<string>(type: "text", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GfwMetadataCache", x => x.Mmsi);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GfwMetadataCache");
        }
    }
}
