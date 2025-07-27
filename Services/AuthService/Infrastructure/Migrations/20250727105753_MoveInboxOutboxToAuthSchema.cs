using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveInboxOutboxToAuthSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                newName: "OutboxMessages",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "InboxMessages",
                newName: "InboxMessages",
                newSchema: "auth");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "auth",
                newName: "OutboxMessages");

            migrationBuilder.RenameTable(
                name: "InboxMessages",
                schema: "auth",
                newName: "InboxMessages");
        }
    }
}
