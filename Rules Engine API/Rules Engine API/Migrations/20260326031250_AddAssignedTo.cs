using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rules_Engine_API.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "EvaluationSessions");

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "EvaluationSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "EvaluationSessions");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "EvaluationSessions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
