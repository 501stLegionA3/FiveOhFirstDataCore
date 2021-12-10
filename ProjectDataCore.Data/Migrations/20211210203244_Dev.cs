using System;
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
                name: "CustomPageSettings",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    Route = table.Column<string>(type: "text", nullable: false),
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
                name: "PageComponentSettingsBase",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    QualifiedTypeName = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ParentLayoutId = table.Column<Guid>(type: "uuid", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    PropertyToEdit = table.Column<string>(type: "text", nullable: true),
                    StaticProperty = table.Column<bool>(type: "boolean", nullable: true),
                    Label = table.Column<string>(type: "text", nullable: true),
                    Placeholder = table.Column<string>(type: "text", nullable: true),
                    MaxChildComponents = table.Column<int>(type: "integer", nullable: true),
                    ParentPageId = table.Column<Guid>(type: "uuid", nullable: true),
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
                });

            migrationBuilder.CreateTable(
                name: "RosterObject",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    OccupiedById = table.Column<int>(type: "integer", nullable: true),
                    RosterParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    RosterTreeKey = table.Column<Guid>(type: "uuid", nullable: true),
                    RosterTreeKey1 = table.Column<Guid>(type: "uuid", nullable: true),
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
                        name: "FK_RosterObject_RosterObject_RosterTreeKey",
                        column: x => x.RosterTreeKey,
                        principalTable: "RosterObject",
                        principalColumn: "Key");
                    table.ForeignKey(
                        name: "FK_RosterObject_RosterObject_RosterTreeKey1",
                        column: x => x.RosterTreeKey1,
                        principalTable: "RosterObject",
                        principalColumn: "Key");
                });

            migrationBuilder.CreateTable(
                name: "RosterParentLinks",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ParentRosterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildRosterKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildRosertId = table.Column<Guid>(type: "uuid", nullable: false),
                    ForRosterSettingsId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterParentLinks", x => x.Key);
                    table.ForeignKey(
                        name: "FK_RosterParentLinks_RosterDisplaySettings_ForRosterSettingsId",
                        column: x => x.ForRosterSettingsId,
                        principalTable: "RosterDisplaySettings",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RosterParentLinks_RosterObject_ChildRosterKey",
                        column: x => x.ChildRosterKey,
                        principalTable: "RosterObject",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RosterParentLinks_RosterObject_ParentRosterId",
                        column: x => x.ParentRosterId,
                        principalTable: "RosterObject",
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
                name: "IX_CustomPageSettings_Route",
                table: "CustomPageSettings",
                column: "Route",
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
                name: "IX_RosterDisplaySettings_HostRosterId",
                table: "RosterDisplaySettings",
                column: "HostRosterId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterObject_OccupiedById",
                table: "RosterObject",
                column: "OccupiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RosterObject_RosterParentId",
                table: "RosterObject",
                column: "RosterParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterObject_RosterTreeKey",
                table: "RosterObject",
                column: "RosterTreeKey");

            migrationBuilder.CreateIndex(
                name: "IX_RosterObject_RosterTreeKey1",
                table: "RosterObject",
                column: "RosterTreeKey1");

            migrationBuilder.CreateIndex(
                name: "IX_RosterParentLinks_ChildRosterKey",
                table: "RosterParentLinks",
                column: "ChildRosterKey");

            migrationBuilder.CreateIndex(
                name: "IX_RosterParentLinks_ForRosterSettingsId",
                table: "RosterParentLinks",
                column: "ForRosterSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterParentLinks_ParentRosterId",
                table: "RosterParentLinks",
                column: "ParentRosterId");

            migrationBuilder.AddForeignKey(
                name: "FK_RosterDisplaySettings_RosterObject_HostRosterId",
                table: "RosterDisplaySettings",
                column: "HostRosterId",
                principalTable: "RosterObject",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RosterObject_RosterParentLinks_RosterParentId",
                table: "RosterObject",
                column: "RosterParentId",
                principalTable: "RosterParentLinks",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RosterObject_AspNetUsers_OccupiedById",
                table: "RosterObject");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterObject_RosterParentLinks_RosterParentId",
                table: "RosterObject");

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
                name: "PageComponentSettingsBase");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CustomPageSettings");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "RosterParentLinks");

            migrationBuilder.DropTable(
                name: "RosterDisplaySettings");

            migrationBuilder.DropTable(
                name: "RosterObject");
        }
    }
}
