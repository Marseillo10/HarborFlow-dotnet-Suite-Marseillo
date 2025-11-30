using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HarborFlowSuite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: true),
                    ChangedBy = table.Column<string>(type: "text", nullable: true),
                    ChangeDetails = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyHistories_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyHistories_CompanyId",
                table: "CompanyHistories",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyHistories");
        }
    }
}
