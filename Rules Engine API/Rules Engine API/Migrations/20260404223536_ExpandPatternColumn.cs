using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rules_Engine_API.Migrations
{
    /// <inheritdoc />
    public partial class ExpandPatternColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
        name: "Pattern",
        table: "AttackEvaluationRecords",
        type: "nvarchar(800)",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "nvarchar(50)",
        oldMaxLength: 50,
        oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
       name: "Pattern",
       table: "AttackEvaluationRecords",
       type: "nvarchar(50)",
       maxLength: 50,
       nullable: true,
       oldClrType: typeof(string),
       oldType: "nvarchar(max)",
       oldNullable: true);
        }
    }
}
