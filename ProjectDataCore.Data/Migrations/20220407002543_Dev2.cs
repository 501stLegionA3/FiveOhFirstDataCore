using System;
using System.Collections.Generic;
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

            migrationBuilder.AlterColumn<List<bool>>(
                name: "SetValue",
                table: "AssignableValues",
                type: "boolean[]",
                nullable: true,
                oldClrType: typeof(List<DateOnly>),
                oldType: "date[]",
                oldNullable: true);

            migrationBuilder.AddColumn<List<DateOnly>>(
                name: "DateOnlyAssignableValue_SetValue",
                table: "AssignableValues",
                type: "date[]",
                nullable: true);

            migrationBuilder.AlterColumn<List<bool>>(
                name: "AllowedValues",
                table: "AssignableConfigurations",
                type: "boolean[]",
                nullable: true,
                oldClrType: typeof(List<DateOnly>),
                oldType: "date[]",
                oldNullable: true);

            migrationBuilder.AddColumn<List<DateOnly>>(
                name: "DateOnlyValueAssignableConfiguration_AllowedValues",
                table: "AssignableConfigurations",
                type: "date[]",
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

            migrationBuilder.DropColumn(
                name: "DateOnlyAssignableValue_SetValue",
                table: "AssignableValues");

            migrationBuilder.DropColumn(
                name: "DateOnlyValueAssignableConfiguration_AllowedValues",
                table: "AssignableConfigurations");

            migrationBuilder.AlterColumn<List<DateOnly>>(
                name: "SetValue",
                table: "AssignableValues",
                type: "date[]",
                nullable: true,
                oldClrType: typeof(List<bool>),
                oldType: "boolean[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<DateOnly>>(
                name: "AllowedValues",
                table: "AssignableConfigurations",
                type: "date[]",
                nullable: true,
                oldClrType: typeof(List<bool>),
                oldType: "boolean[]",
                oldNullable: true);
        }
    }
}
