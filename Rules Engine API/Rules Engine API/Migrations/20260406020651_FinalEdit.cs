using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rules_Engine_API.Migrations
{
    /// <inheritdoc />
    public partial class FinalEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EvaluationSessions_IsEvaluated",
                table: "EvaluationSessions");

            migrationBuilder.DropColumn(
                name: "IsEvaluated",
                table: "EvaluationSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEvaluated",
                table: "EvaluationSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationSessions_IsEvaluated",
                table: "EvaluationSessions",
                column: "IsEvaluated");
        }
    }
}
