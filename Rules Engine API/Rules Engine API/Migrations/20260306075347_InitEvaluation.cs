using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rules_Engine_API.Migrations
{
    /// <inheritdoc />
    public partial class InitEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvaluationSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetectedAttackType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MatchedAttacksJson = table.Column<string>(type: "TEXT", nullable: false),
                    DetectionCorrect = table.Column<bool>(type: "bit", nullable: true),
                    ClassificationCorrect = table.Column<bool>(type: "bit", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsEvaluated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationSessions_CreatedAt",
                table: "EvaluationSessions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationSessions_DetectedAttackType",
                table: "EvaluationSessions",
                column: "DetectedAttackType");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationSessions_IsEvaluated",
                table: "EvaluationSessions",
                column: "IsEvaluated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluationSessions");
        }
    }
}
