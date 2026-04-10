using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rules_Engine_API.Migrations
{
    /// <inheritdoc />
    public partial class InitEvaluationV01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity",
                table: "EvaluationSessions");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "EvaluationSessions");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "EvaluationSessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MatchedAttacksJson",
                table: "EvaluationSessions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "EvaluationSessions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MatchedAttacksJson",
                table: "EvaluationSessions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "EvaluationSessions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalScore",
                table: "EvaluationSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
