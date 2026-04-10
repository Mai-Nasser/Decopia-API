using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rules_Engine_API.Migrations
{
    /// <inheritdoc />
    public partial class AddAttackEvaluationRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EvaluationSessions_DetectedAttackType",
                table: "EvaluationSessions");

            migrationBuilder.DropColumn(
                name: "ClassificationCorrect",
                table: "EvaluationSessions");

            migrationBuilder.DropColumn(
                name: "DetectionCorrect",
                table: "EvaluationSessions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "EvaluationSessions");

            migrationBuilder.CreateTable(
                name: "AttackEvaluationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EvaluationSessionId = table.Column<int>(type: "int", nullable: false),
                    AttackName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DetectionCorrect = table.Column<bool>(type: "bit", nullable: false),
                    ClassificationCorrect = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttackEvaluationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttackEvaluationRecords_EvaluationSessions_EvaluationSessionId",
                        column: x => x.EvaluationSessionId,
                        principalTable: "EvaluationSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttackEvaluationRecords_AttackName",
                table: "AttackEvaluationRecords",
                column: "AttackName");

            migrationBuilder.CreateIndex(
                name: "IX_AttackEvaluationRecords_EvaluationSessionId",
                table: "AttackEvaluationRecords",
                column: "EvaluationSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttackEvaluationRecords");

            migrationBuilder.AddColumn<bool>(
                name: "ClassificationCorrect",
                table: "EvaluationSessions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DetectionCorrect",
                table: "EvaluationSessions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "EvaluationSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationSessions_DetectedAttackType",
                table: "EvaluationSessions",
                column: "DetectedAttackType");
        }
    }
}
