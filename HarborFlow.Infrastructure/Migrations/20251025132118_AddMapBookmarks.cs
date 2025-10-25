using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HarborFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMapBookmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "map_bookmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    North = table.Column<double>(type: "double precision", nullable: false),
                    South = table.Column<double>(type: "double precision", nullable: false),
                    East = table.Column<double>(type: "double precision", nullable: false),
                    West = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map_bookmarks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_map_bookmarks_UserId_Name",
                table: "map_bookmarks",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "map_bookmarks");
        }
    }
}
