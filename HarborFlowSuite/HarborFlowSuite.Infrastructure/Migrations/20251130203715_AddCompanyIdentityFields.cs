using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HarborFlowSuite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIdentityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryContactEmail",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionTier",
                table: "Companies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Companies",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PrimaryContactEmail",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SubscriptionTier",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Companies");
        }
    }
}
