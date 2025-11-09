using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HarborFlowSuite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalHistory_ServiceRequests_ServiceRequestId",
                table: "ApprovalHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalHistory_Users_ApproverId",
                table: "ApprovalHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApprovalHistory",
                table: "ApprovalHistory");

            migrationBuilder.RenameTable(
                name: "ApprovalHistory",
                newName: "ApprovalHistories");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalHistory_ServiceRequestId",
                table: "ApprovalHistories",
                newName: "IX_ApprovalHistories_ServiceRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalHistory_ApproverId",
                table: "ApprovalHistories",
                newName: "IX_ApprovalHistories_ApproverId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ApprovalHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApprovalHistories",
                table: "ApprovalHistories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistories_UserId",
                table: "ApprovalHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalHistories_ServiceRequests_ServiceRequestId",
                table: "ApprovalHistories",
                column: "ServiceRequestId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalHistories_Users_ApproverId",
                table: "ApprovalHistories",
                column: "ApproverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalHistories_Users_UserId",
                table: "ApprovalHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalHistories_ServiceRequests_ServiceRequestId",
                table: "ApprovalHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalHistories_Users_ApproverId",
                table: "ApprovalHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalHistories_Users_UserId",
                table: "ApprovalHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApprovalHistories",
                table: "ApprovalHistories");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalHistories_UserId",
                table: "ApprovalHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ApprovalHistories");

            migrationBuilder.RenameTable(
                name: "ApprovalHistories",
                newName: "ApprovalHistory");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalHistories_ServiceRequestId",
                table: "ApprovalHistory",
                newName: "IX_ApprovalHistory_ServiceRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalHistories_ApproverId",
                table: "ApprovalHistory",
                newName: "IX_ApprovalHistory_ApproverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApprovalHistory",
                table: "ApprovalHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalHistory_ServiceRequests_ServiceRequestId",
                table: "ApprovalHistory",
                column: "ServiceRequestId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalHistory_Users_ApproverId",
                table: "ApprovalHistory",
                column: "ApproverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
