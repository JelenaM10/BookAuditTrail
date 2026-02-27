using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookAuditTrail.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToAuditLogsChangedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BookAuditLogs_ChangedAt",
                table: "BookAuditLogs",
                column: "ChangedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookAuditLogs_ChangedAt",
                table: "BookAuditLogs");
        }
    }
}
