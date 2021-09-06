using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class ReleaseSoyvolonVersionZeroTwoZero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecruitStatuses",
                table: "RecruitStatuses");

            migrationBuilder.RenameColumn(
                name: "RecruitStatusKey",
                table: "RecruitStatuses",
                newName: "PreferredRole");

            migrationBuilder.AlterColumn<int>(
                name: "PreferredRole",
                table: "RecruitStatuses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "RecruitStatusId",
                table: "RecruitStatuses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddColumn<bool>(
                name: "MilitaryPolice",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecruitStatuses",
                table: "RecruitStatuses",
                column: "RecruitStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecruitStatuses",
                table: "RecruitStatuses");

            migrationBuilder.DropColumn(
                name: "RecruitStatusId",
                table: "RecruitStatuses");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "RecruitStatuses");

            migrationBuilder.DropColumn(
                name: "PossibleTroll",
                table: "RecruitStatuses");

            migrationBuilder.DropColumn(
                name: "MilitaryPolice",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PreferredRole",
                table: "RecruitStatuses",
                newName: "RecruitStatusKey");

            migrationBuilder.AlterColumn<int>(
                name: "RecruitStatusKey",
                table: "RecruitStatuses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecruitStatuses",
                table: "RecruitStatuses",
                column: "RecruitStatusKey");
        }
    }
}
