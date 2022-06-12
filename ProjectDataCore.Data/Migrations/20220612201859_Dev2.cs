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

            migrationBuilder.DropForeignKey(
                name: "FK_PageComponentSettingsBase_PageComponentSettingsBase_UserSco~",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSelectComponentSettings_PageComponentSettingsBase_Layou~",
                table: "UserSelectComponentSettings");

            migrationBuilder.DropIndex(
                name: "IX_UserSelectComponentSettings_LayoutComponentId",
                table: "UserSelectComponentSettings");

            migrationBuilder.DropIndex(
                name: "IX_PageComponentSettingsBase_ParentLayoutId",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropIndex(
                name: "IX_PageComponentSettingsBase_ParentPageId",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropIndex(
                name: "IX_PageComponentSettingsBase_UserScopeId",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropColumn(
                name: "LayoutComponentId",
                table: "UserSelectComponentSettings");

            migrationBuilder.DropColumn(
                name: "MaxChildComponents",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropColumn(
                name: "ParentLayoutId",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropColumn(
                name: "ParentPageId",
                table: "PageComponentSettingsBase");

            migrationBuilder.RenameColumn(
                name: "UserScopeId",
                table: "PageComponentSettingsBase",
                newName: "ParentNodeId");

            migrationBuilder.RenameColumn(
                name: "PropertyToEdit",
                table: "PageComponentSettingsBase",
                newName: "UnAuthorizedRaw");

            migrationBuilder.RenameColumn(
                name: "Label",
                table: "PageComponentSettingsBase",
                newName: "PropertyName");

            migrationBuilder.RenameColumn(
                name: "FormatString",
                table: "PageComponentSettingsBase",
                newName: "AuthorizedRaw");

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
                name: "AssignableValueRenderers",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    Static = table.Column<bool>(type: "boolean", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignableValueRenderers", x => x.Key);
                });

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
                    KeyPressed = table.Column<string>(type: "text", nullable: false),
                    Keybinding = table.Column<int>(type: "integer", nullable: false),
                    ShiftKey = table.Column<bool>(type: "boolean", nullable: false),
                    CtrlKey = table.Column<bool>(type: "boolean", nullable: false),
                    AltKey = table.Column<bool>(type: "boolean", nullable: false),
                    MetaKey = table.Column<bool>(type: "boolean", nullable: false),
                    DataCoreUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKeybinding", x => x.KeyPressed);
                    table.ForeignKey(
                        name: "FK_UserKeybinding_AspNetUsers_DataCoreUserId",
                        column: x => x.DataCoreUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserScopes",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    PageId = table.Column<Guid>(type: "uuid", nullable: false),
                    IncludeLocalUser = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserScopes", x => x.Key);
                    table.ForeignKey(
                        name: "FK_UserScopes_CustomPageSettings_PageId",
                        column: x => x.PageId,
                        principalTable: "CustomPageSettings",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignableValueConversions",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    RendererId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValueName = table.Column<string>(type: "text", nullable: false),
                    String_FormatString = table.Column<string>(type: "text", nullable: true),
                    Numeric_FormatString = table.Column<string>(type: "text", nullable: true),
                    DateTime_ToStringPattern = table.Column<string>(type: "text", nullable: true),
                    DateTime_ConvertToTimeSpan = table.Column<bool>(type: "boolean", nullable: false),
                    DateTime_TimeSpanConversionCompareTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Bool_FormatOnTrue = table.Column<string>(type: "text", nullable: false),
                    Bool_FormatOnFalse = table.Column<string>(type: "text", nullable: false),
                    Multi_MaxValues = table.Column<int>(type: "integer", nullable: false),
                    Multi_Separator = table.Column<string>(type: "text", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignableValueConversions", x => x.Key);
                    table.ForeignKey(
                        name: "FK_AssignableValueConversions_AssignableValueRenderers_Rendere~",
                        column: x => x.RendererId,
                        principalTable: "AssignableValueRenderers",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageComponentSettingsBaseUserScope",
                columns: table => new
                {
                    ScopeListenersKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopeProvidersKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageComponentSettingsBaseUserScope", x => new { x.ScopeListenersKey, x.ScopeProvidersKey });
                    table.ForeignKey(
                        name: "FK_PageComponentSettingsBaseUserScope_PageComponentSettingsBas~",
                        column: x => x.ScopeListenersKey,
                        principalTable: "PageComponentSettingsBase",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageComponentSettingsBaseUserScope_UserScopes_ScopeProvider~",
                        column: x => x.ScopeProvidersKey,
                        principalTable: "UserScopes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageComponentSettingsBaseUserScope1",
                columns: table => new
                {
                    ScopeListenersKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopeProvidersKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageComponentSettingsBaseUserScope1", x => new { x.ScopeListenersKey, x.ScopeProvidersKey });
                    table.ForeignKey(
                        name: "FK_PageComponentSettingsBaseUserScope1_PageComponentSettingsBa~",
                        column: x => x.ScopeProvidersKey,
                        principalTable: "PageComponentSettingsBase",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageComponentSettingsBaseUserScope1_UserScopes_ScopeListene~",
                        column: x => x.ScopeListenersKey,
                        principalTable: "UserScopes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageComponentSettingsBase_ParentNodeId",
                table: "PageComponentSettingsBase",
                column: "ParentNodeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignableValueConversions_RendererId",
                table: "AssignableValueConversions",
                column: "RendererId");

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
                name: "IX_PageComponentSettingsBaseUserScope_ScopeProvidersKey",
                table: "PageComponentSettingsBaseUserScope",
                column: "ScopeProvidersKey");

            migrationBuilder.CreateIndex(
                name: "IX_PageComponentSettingsBaseUserScope1_ScopeProvidersKey",
                table: "PageComponentSettingsBaseUserScope1",
                column: "ScopeProvidersKey");

            migrationBuilder.CreateIndex(
                name: "IX_UserKeybinding_DataCoreUserId",
                table: "UserKeybinding",
                column: "DataCoreUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_PageId",
                table: "UserScopes",
                column: "PageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageComponentSettingsBase_LayoutNodes_ParentNodeId",
                table: "PageComponentSettingsBase",
                column: "ParentNodeId",
                principalTable: "LayoutNodes",
                principalColumn: "Key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageComponentSettingsBase_LayoutNodes_ParentNodeId",
                table: "PageComponentSettingsBase");

            migrationBuilder.DropTable(
                name: "AssignableValueConversions");

            migrationBuilder.DropTable(
                name: "LayoutNodes");

            migrationBuilder.DropTable(
                name: "PageComponentSettingsBaseUserScope");

            migrationBuilder.DropTable(
                name: "PageComponentSettingsBaseUserScope1");

            migrationBuilder.DropTable(
                name: "UserKeybinding");

            migrationBuilder.DropTable(
                name: "AssignableValueRenderers");

            migrationBuilder.DropTable(
                name: "UserScopes");

            migrationBuilder.DropIndex(
                name: "IX_PageComponentSettingsBase_ParentNodeId",
                table: "PageComponentSettingsBase");

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
                name: "UnAuthorizedRaw",
                table: "PageComponentSettingsBase",
                newName: "PropertyToEdit");

            migrationBuilder.RenameColumn(
                name: "PropertyName",
                table: "PageComponentSettingsBase",
                newName: "Label");

            migrationBuilder.RenameColumn(
                name: "ParentNodeId",
                table: "PageComponentSettingsBase",
                newName: "UserScopeId");

            migrationBuilder.RenameColumn(
                name: "AuthorizedRaw",
                table: "PageComponentSettingsBase",
                newName: "FormatString");

            migrationBuilder.AddColumn<Guid>(
                name: "LayoutComponentId",
                table: "UserSelectComponentSettings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "MaxChildComponents",
                table: "PageComponentSettingsBase",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "PageComponentSettingsBase",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentLayoutId",
                table: "PageComponentSettingsBase",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentPageId",
                table: "PageComponentSettingsBase",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSelectComponentSettings_LayoutComponentId",
                table: "UserSelectComponentSettings",
                column: "LayoutComponentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageComponentSettingsBase_ParentLayoutId",
                table: "PageComponentSettingsBase",
                column: "ParentLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_PageComponentSettingsBase_ParentPageId",
                table: "PageComponentSettingsBase",
                column: "ParentPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageComponentSettingsBase_UserScopeId",
                table: "PageComponentSettingsBase",
                column: "UserScopeId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_PageComponentSettingsBase_PageComponentSettingsBase_UserSco~",
                table: "PageComponentSettingsBase",
                column: "UserScopeId",
                principalTable: "PageComponentSettingsBase",
                principalColumn: "Key");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSelectComponentSettings_PageComponentSettingsBase_Layou~",
                table: "UserSelectComponentSettings",
                column: "LayoutComponentId",
                principalTable: "PageComponentSettingsBase",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
