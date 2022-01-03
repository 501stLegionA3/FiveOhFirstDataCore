using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectDataCore.Data.Migrations
{
    public partial class Dev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayId = table.Column<int>(type: "integer", nullable: false),
                    NickName = table.Column<string>(type: "text", nullable: false),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    SteamLink = table.Column<string>(type: "text", nullable: true),
                    AccessCode = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssignableConfigurations",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    AllowMultiple = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedInput = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    AvalibleValues = table.Column<List<DateOnly>>(type: "date[]", nullable: true),
                    DateTimeValueAssignableConfiguration_AvalibleValues = table.Column<List<DateTime>>(type: "timestamp with time zone[]", nullable: true),
                    DoubleValueAssignableConfiguration_AvalibleValues = table.Column<List<double>>(type: "double precision[]", nullable: true),
                    IntegerValueAssignableConfiguration_AvalibleValues = table.Column<List<int>>(type: "integer[]", nullable: true),
                    StringValueAssignableConfiguration_AvalibleValues = table.Column<List<string>>(type: "text[]", nullable: true),
                    TimeOnlyValueAssignableConfiguration_AvalibleValues = table.Column<List<TimeOnly>>(type: "time without time zone[]", nullable: true),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignableConfigurations", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "CustomPageSettings",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    Route = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LayoutId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomPageSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RosterObject",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    OccupiedById = table.Column<int>(type: "integer", nullable: true),
                    ParentRosterId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterObject", x => x.Key);
                    table.ForeignKey(
                        name: "FK_RosterObject_AspNetUsers_OccupiedById",
                        column: x => x.OccupiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RosterObject_RosterObject_ParentRosterId",
                        column: x => x.ParentRosterId,
                        principalTable: "RosterObject",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignableValues",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    ForUserId = table.Column<int>(type: "integer", nullable: false),
                    AssignableConfigurationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    SetValue = table.Column<DateOnly>(type: "date", nullable: true),
                    DateTimeAssignableValue_SetValue = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DoubleAssignableValue_SetValue = table.Column<double>(type: "double precision", nullable: true),
                    IntegerAssignableValue_SetValue = table.Column<int>(type: "integer", nullable: true),
                    StringAssignableValue_SetValue = table.Column<string>(type: "text", nullable: true),
                    TimeOnlyAssignableValue_SetValue = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignableValues", x => x.Key);
                    table.ForeignKey(
                        name: "FK_AssignableValues_AspNetUsers_ForUserId",
                        column: x => x.ForUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignableValues_AssignableConfigurations_AssignableConfigu~",
                        column: x => x.AssignableConfigurationId,
                        principalTable: "AssignableConfigurations",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageComponentSettingsBase",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    QualifiedTypeName = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ParentLayoutId = table.Column<Guid>(type: "uuid", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    MaxChildComponents = table.Column<int>(type: "integer", nullable: true),
                    ParentPageId = table.Column<Guid>(type: "uuid", nullable: true),
                    PropertyToEdit = table.Column<string>(type: "text", nullable: true),
                    StaticProperty = table.Column<bool>(type: "boolean", nullable: true),
                    Label = table.Column<string>(type: "text", nullable: true),
                    UserScopeId = table.Column<Guid>(type: "uuid", nullable: true),
                    FormatString = table.Column<string>(type: "text", nullable: true),
                    Placeholder = table.Column<string>(type: "text", nullable: true),
                    Scoped = table.Column<bool>(type: "boolean", nullable: true),
                    AllowUserLisiting = table.Column<bool>(type: "boolean", nullable: true),
                    LevelFromTop = table.Column<int>(type: "integer", nullable: true),
                    Depth = table.Column<int>(type: "integer", nullable: true),
                    DefaultRoster = table.Column<Guid>(type: "uuid", nullable: true),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageComponentSettingsBase", x => x.Key);
                    table.ForeignKey(
                        name: "FK_PageComponentSettingsBase_CustomPageSettings_ParentPageId",
                        column: x => x.ParentPageId,
                        principalTable: "CustomPageSettings",
                        principalColumn: "Key");
                    table.ForeignKey(
                        name: "FK_PageComponentSettingsBase_PageComponentSettingsBase_ParentL~",
                        column: x => x.ParentLayoutId,
                        principalTable: "PageComponentSettingsBase",
                        principalColumn: "Key");
                    table.ForeignKey(
                        name: "FK_PageComponentSettingsBase_PageComponentSettingsBase_UserSco~",
                        column: x => x.UserScopeId,
                        principalTable: "PageComponentSettingsBase",
                        principalColumn: "Key");
                });

            migrationBuilder.CreateTable(
                name: "RosterDisplaySettings",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    HostRosterId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterDisplaySettings", x => x.Key);
                    table.ForeignKey(
                        name: "FK_RosterDisplaySettings_RosterObject_HostRosterId",
                        column: x => x.HostRosterId,
                        principalTable: "RosterObject",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RosterOrders",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    TreeToOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SlotToOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterOrders", x => x.Key);
                    table.ForeignKey(
                        name: "FK_RosterOrders_RosterObject_ParentObjectId",
                        column: x => x.ParentObjectId,
                        principalTable: "RosterObject",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RosterOrders_RosterObject_SlotToOrderId",
                        column: x => x.SlotToOrderId,
                        principalTable: "RosterObject",
                        principalColumn: "Key");
                    table.ForeignKey(
                        name: "FK_RosterOrders_RosterObject_TreeToOrderId",
                        column: x => x.TreeToOrderId,
                        principalTable: "RosterObject",
                        principalColumn: "Key");
                });

            migrationBuilder.CreateTable(
                name: "RosterTreeRosterTree",
                columns: table => new
                {
                    ChildRostersKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentRostersKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterTreeRosterTree", x => new { x.ChildRostersKey, x.ParentRostersKey });
                    table.ForeignKey(
                        name: "FK_RosterTreeRosterTree_RosterObject_ChildRostersKey",
                        column: x => x.ChildRostersKey,
                        principalTable: "RosterObject",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RosterTreeRosterTree_RosterObject_ParentRostersKey",
                        column: x => x.ParentRostersKey,
                        principalTable: "RosterObject",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataCoreUserProperty",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    FormatString = table.Column<string>(type: "text", nullable: false),
                    Alias = table.Column<int>(type: "integer", nullable: false),
                    RosterComponentUserListingDisplayId = table.Column<Guid>(type: "uuid", nullable: true),
                    RosterComponentDefaultDisplayId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataCoreUserProperty", x => x.Key);
                    table.ForeignKey(
                        name: "FK_DataCoreUserProperty_PageComponentSettingsBase_RosterCompo~1",
                        column: x => x.RosterComponentUserListingDisplayId,
                        principalTable: "PageComponentSettingsBase",
                        principalColumn: "Key");
                    table.ForeignKey(
                        name: "FK_DataCoreUserProperty_PageComponentSettingsBase_RosterCompon~",
                        column: x => x.RosterComponentDefaultDisplayId,
                        principalTable: "PageComponentSettingsBase",
                        principalColumn: "Key");
                });

            migrationBuilder.CreateTable(
                name: "RosterComponentSettingsRosterDisplaySettings",
                columns: table => new
                {
                    AvalibleRostersKey = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayComponentsKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterComponentSettingsRosterDisplaySettings", x => new { x.AvalibleRostersKey, x.DisplayComponentsKey });
                    table.ForeignKey(
                        name: "FK_RosterComponentSettingsRosterDisplaySettings_PageComponentS~",
                        column: x => x.DisplayComponentsKey,
                        principalTable: "PageComponentSettingsBase",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RosterComponentSettingsRosterDisplaySettings_RosterDisplayS~",
                        column: x => x.AvalibleRostersKey,
                        principalTable: "RosterDisplaySettings",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignableValues_AssignableConfigurationId",
                table: "AssignableValues",
                column: "AssignableConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignableValues_ForUserId",
                table: "AssignableValues",
                column: "ForUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPageSettings_Route",
                table: "CustomPageSettings",
                column: "Route",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataCoreUserProperty_RosterComponentDefaultDisplayId",
                table: "DataCoreUserProperty",
                column: "RosterComponentDefaultDisplayId");

            migrationBuilder.CreateIndex(
                name: "IX_DataCoreUserProperty_RosterComponentUserListingDisplayId",
                table: "DataCoreUserProperty",
                column: "RosterComponentUserListingDisplayId");

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

            migrationBuilder.CreateIndex(
                name: "IX_RosterComponentSettingsRosterDisplaySettings_DisplayCompone~",
                table: "RosterComponentSettingsRosterDisplaySettings",
                column: "DisplayComponentsKey");

            migrationBuilder.CreateIndex(
                name: "IX_RosterDisplaySettings_HostRosterId",
                table: "RosterDisplaySettings",
                column: "HostRosterId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterObject_OccupiedById",
                table: "RosterObject",
                column: "OccupiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RosterObject_ParentRosterId",
                table: "RosterObject",
                column: "ParentRosterId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterOrders_ParentObjectId",
                table: "RosterOrders",
                column: "ParentObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterOrders_SlotToOrderId",
                table: "RosterOrders",
                column: "SlotToOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RosterOrders_TreeToOrderId",
                table: "RosterOrders",
                column: "TreeToOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterTreeRosterTree_ParentRostersKey",
                table: "RosterTreeRosterTree",
                column: "ParentRostersKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AssignableValues");

            migrationBuilder.DropTable(
                name: "DataCoreUserProperty");

            migrationBuilder.DropTable(
                name: "RosterComponentSettingsRosterDisplaySettings");

            migrationBuilder.DropTable(
                name: "RosterOrders");

            migrationBuilder.DropTable(
                name: "RosterTreeRosterTree");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AssignableConfigurations");

            migrationBuilder.DropTable(
                name: "PageComponentSettingsBase");

            migrationBuilder.DropTable(
                name: "RosterDisplaySettings");

            migrationBuilder.DropTable(
                name: "CustomPageSettings");

            migrationBuilder.DropTable(
                name: "RosterObject");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
