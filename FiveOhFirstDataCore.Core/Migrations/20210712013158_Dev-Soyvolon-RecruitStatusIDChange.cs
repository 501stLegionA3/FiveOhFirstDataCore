using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class DevSoyvolonRecruitStatusIDChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecruitStatuses",
                table: "RecruitStatuses");

            migrationBuilder.DropColumn(
                name: "RecruitStatusKey",
                table: "RecruitStatuses");

            migrationBuilder.AddColumn<Guid>(
                name: "RecruitStatusId",
                table: "RecruitStatuses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddColumn<int>(
                name: "RecruitStatusKey",
                table: "RecruitStatuses",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecruitStatuses",
                table: "RecruitStatuses",
                column: "RecruitStatusKey");
        }
    }
}
