using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookAuditTrail.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditLogIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookAuditLogs_BookId",
                table: "BookAuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_BookAuditLogs_ChangedAt",
                table: "BookAuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_BookAuditLogs_ChangeType",
                table: "BookAuditLogs");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CreatedAt",
                table: "Books",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuditLogs_BookId_ChangedAt",
                table: "BookAuditLogs",
                columns: new[] { "BookId", "ChangedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_CreatedAt",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_BookAuditLogs_BookId_ChangedAt",
                table: "BookAuditLogs");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuditLogs_BookId",
                table: "BookAuditLogs",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuditLogs_ChangedAt",
                table: "BookAuditLogs",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuditLogs_ChangeType",
                table: "BookAuditLogs",
                column: "ChangeType");
        }
    }
}
