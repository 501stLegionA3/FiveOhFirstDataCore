using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class DevSoyvolonAdditionalRecruitInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "RecruitStatuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PossibleTroll",
                table: "RecruitStatuses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PreferredRole",
                table: "RecruitStatuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "RecruitStatuses");

            migrationBuilder.DropColumn(
                name: "PossibleTroll",
                table: "RecruitStatuses");

            migrationBuilder.DropColumn(
                name: "PreferredRole",
                table: "RecruitStatuses");
        }
    }
}
