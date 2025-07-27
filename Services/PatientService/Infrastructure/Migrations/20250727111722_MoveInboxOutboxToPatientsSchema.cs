using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveInboxOutboxToPatientsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "patients");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "patient",
                newName: "OutboxMessages",
                newSchema: "patients");

            migrationBuilder.RenameTable(
                name: "InboxMessages",
                schema: "patient",
                newName: "InboxMessages",
                newSchema: "patients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "patient");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "patients",
                newName: "OutboxMessages",
                newSchema: "patient");

            migrationBuilder.RenameTable(
                name: "InboxMessages",
                schema: "patients",
                newName: "InboxMessages",
                newSchema: "patient");
        }
    }
}
