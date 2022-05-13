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
            migrationBuilder.DropForeignKey(
                name: "FK_PageComponentSettingsBase_CustomPageSettings_ParentPageId",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropForeignKey(
                name: "FK_PageComponentSettingsBase_PageComponentSettingsBase_ParentL~",
                table: "PageComponentSettingsBase");

            migrationBuilder.RenameColumn(
                name: "ParentPageId",
                table: "PageComponentSettingsBase",
                newName: "ParentNodeId");

            migrationBuilder.RenameColumn(
                name: "ParentLayoutId",
                table: "PageComponentSettingsBase",
                newName: "LayoutComponentSettingsKey");

            migrationBuilder.RenameIndex(
                name: "IX_PageComponentSettingsBase_ParentPageId",
                table: "PageComponentSettingsBase",
                newName: "IX_PageComponentSettingsBase_ParentNodeId");

            migrationBuilder.RenameIndex(
                name: "IX_PageComponentSettingsBase_ParentLayoutId",
                table: "PageComponentSettingsBase",
                newName: "IX_PageComponentSettingsBase_LayoutComponentSettingsKey");

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

            migrationBuilder.AddColumn<List<bool>>(
                name: "BooleanAssignableValue_SetValue",
                table: "AssignableValues",
                type: "boolean[]",
                nullable: true);

            migrationBuilder.AddColumn<List<bool>>(
                name: "BooleanValueAssignableConfiguration_AllowedValues",
                table: "AssignableConfigurations",
                type: "boolean[]",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LayoutNodes",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentNodeId = table.Column<Guid>(type: "uuid", nullable: true),
                    PageSettingsId = table.Column<Guid>(type: "uuid", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Rows = table.Column<bool>(type: "boolean", nullable: false),
                    RawNodeWidths = table.Column<string>(type: "text", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayoutNodes", x => x.Key);
                    table.ForeignKey(
                        name: "FK_LayoutNodes_CustomPageSettings_PageSettingsId",
                        column: x => x.PageSettingsId,
                        principalTable: "CustomPageSettings",
                        principalColumn: "Key");
                    table.ForeignKey(
                        name: "FK_LayoutNodes_LayoutNodes_ParentNodeId",
                        column: x => x.ParentNodeId,
                        principalTable: "LayoutNodes",
                        principalColumn: "Key");
                });

            migrationBuilder.CreateTable(
                name: "UserKeybinding",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    Keybinding = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CtrlKey = table.Column<bool>(type: "boolean", nullable: false),
                    AltKey = table.Column<bool>(type: "boolean", nullable: false),
                    MetaKey = table.Column<bool>(type: "boolean", nullable: false),
                    DataCoreUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKeybinding", x => x.Key);
                    table.ForeignKey(
                        name: "FK_UserKeybinding_AspNetUsers_DataCoreUserId",
                        column: x => x.DataCoreUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LayoutNodes_PageSettingsId",
                table: "LayoutNodes",
                column: "PageSettingsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LayoutNodes_ParentNodeId",
                table: "LayoutNodes",
                column: "ParentNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserKeybinding_DataCoreUserId",
                table: "UserKeybinding",
                column: "DataCoreUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageComponentSettingsBase_LayoutNodes_ParentNodeId",
                table: "PageComponentSettingsBase",
                column: "ParentNodeId",
                principalTable: "LayoutNodes",
                principalColumn: "Key");

            migrationBuilder.AddForeignKey(
                name: "FK_PageComponentSettingsBase_PageComponentSettingsBase_LayoutC~",
                table: "PageComponentSettingsBase",
                column: "LayoutComponentSettingsKey",
                principalTable: "PageComponentSettingsBase",
                principalColumn: "Key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageComponentSettingsBase_LayoutNodes_ParentNodeId",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropForeignKey(
                name: "FK_PageComponentSettingsBase_PageComponentSettingsBase_LayoutC~",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropTable(
                name: "LayoutNodes");

            migrationBuilder.DropTable(
                name: "UserKeybinding");

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
                name: "BooleanAssignableValue_SetValue",
                table: "AssignableValues");

            migrationBuilder.DropColumn(
                name: "BooleanValueAssignableConfiguration_AllowedValues",
                table: "AssignableConfigurations");

            migrationBuilder.RenameColumn(
                name: "ParentNodeId",
                table: "PageComponentSettingsBase",
                newName: "ParentPageId");

            migrationBuilder.RenameColumn(
                name: "LayoutComponentSettingsKey",
                table: "PageComponentSettingsBase",
                newName: "ParentLayoutId");

            migrationBuilder.RenameIndex(
                name: "IX_PageComponentSettingsBase_ParentNodeId",
                table: "PageComponentSettingsBase",
                newName: "IX_PageComponentSettingsBase_ParentPageId");

            migrationBuilder.RenameIndex(
                name: "IX_PageComponentSettingsBase_LayoutComponentSettingsKey",
                table: "PageComponentSettingsBase",
                newName: "IX_PageComponentSettingsBase_ParentLayoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageComponentSettingsBase_CustomPageSettings_ParentPageId",
                table: "PageComponentSettingsBase",
                column: "ParentPageId",
                principalTable: "CustomPageSettings",
                principalColumn: "Key");

            migrationBuilder.AddForeignKey(
                name: "FK_PageComponentSettingsBase_PageComponentSettingsBase_ParentL~",
                table: "PageComponentSettingsBase",
                column: "ParentLayoutId",
                principalTable: "PageComponentSettingsBase",
                principalColumn: "Key");
        }
    }
}
