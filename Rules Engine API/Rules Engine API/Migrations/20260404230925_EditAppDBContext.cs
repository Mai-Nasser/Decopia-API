using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rules_Engine_API.Migrations
{
    /// <inheritdoc />
    public partial class EditAppDBContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttackEvaluationRecords_EvaluationSessionId_AttackName_Pattern",
                table: "AttackEvaluationRecords");

            migrationBuilder.CreateIndex(
                name: "IX_AttackEvaluationRecords_EvaluationSessionId_AttackName",
                table: "AttackEvaluationRecords",
                columns: new[] { "EvaluationSessionId", "AttackName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttackEvaluationRecords_EvaluationSessionId_AttackName",
                table: "AttackEvaluationRecords");

            migrationBuilder.CreateIndex(
                name: "IX_AttackEvaluationRecords_EvaluationSessionId_AttackName_Pattern",
                table: "AttackEvaluationRecords",
                columns: new[] { "EvaluationSessionId", "AttackName", "Pattern" });
        }
    }
}
