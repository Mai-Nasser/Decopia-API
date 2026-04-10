using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rules_Engine_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEvaluationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AssignedTo",
                table: "EvaluationSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pattern",
                table: "AttackEvaluationRecords",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationSessions_AssignedTo",
                table: "EvaluationSessions",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_AttackEvaluationRecords_EvaluationSessionId_AttackName_Pattern",
                table: "AttackEvaluationRecords",
                columns: new[] { "EvaluationSessionId", "AttackName", "Pattern" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EvaluationSessions_AssignedTo",
                table: "EvaluationSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttackEvaluationRecords_EvaluationSessionId_AttackName_Pattern",
                table: "AttackEvaluationRecords");

            migrationBuilder.DropColumn(
                name: "Pattern",
                table: "AttackEvaluationRecords");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedTo",
                table: "EvaluationSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
