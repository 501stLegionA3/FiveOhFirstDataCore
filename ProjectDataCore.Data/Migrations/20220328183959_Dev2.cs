using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDataCore.Data.Migrations
{
    public partial class Dev2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthKey",
                table: "NavModules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "NavModules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PageId",
                table: "NavModules",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthKey",
                table: "NavModules");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "NavModules");

            migrationBuilder.DropColumn(
                name: "PageId",
                table: "NavModules");
        }
    }
}
