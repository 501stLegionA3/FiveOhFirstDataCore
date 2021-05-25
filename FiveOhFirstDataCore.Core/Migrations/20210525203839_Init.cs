using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class Init : Migration
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
                    NickName = table.Column<string>(type: "text", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    RTORank = table.Column<int>(type: "integer", nullable: true),
                    MedicRank = table.Column<int>(type: "integer", nullable: true),
                    PilotRank = table.Column<int>(type: "integer", nullable: true),
                    WarrantRank = table.Column<int>(type: "integer", nullable: true),
                    WardenRank = table.Column<int>(type: "integer", nullable: true),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Team = table.Column<int>(type: "integer", nullable: true),
                    Flight = table.Column<int>(type: "integer", nullable: true),
                    CShops = table.Column<long>(type: "bigint", nullable: false),
                    Qualifications = table.Column<long>(type: "bigint", nullable: false),
                    LastPromotion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StartOfService = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastBilletChange = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GraduatedBCTOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GraduatedUTCOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InitalTraining = table.Column<string>(type: "text", nullable: true),
                    UTC = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    DiscordId = table.Column<string>(type: "text", nullable: true),
                    SteamLink = table.Column<string>(type: "text", nullable: true),
                    AccessCode = table.Column<string>(type: "text", nullable: true),
                    NotificationItems = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
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
                name: "CShopChanges",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Added = table.Column<long>(type: "bigint", nullable: false),
                    Removed = table.Column<long>(type: "bigint", nullable: false),
                    OldCShops = table.Column<long>(type: "bigint", nullable: false),
                    ChangedById = table.Column<int>(type: "integer", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CShopChanges", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_CShopChanges_AspNetUsers_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CShopChanges_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisciplinaryActions",
                columns: table => new
                {
                    DAID = table.Column<Guid>(type: "uuid", nullable: false),
                    FiledById = table.Column<int>(type: "integer", nullable: false),
                    FiledToId = table.Column<int>(type: "integer", nullable: false),
                    FiledAgainstId = table.Column<int>(type: "integer", nullable: false),
                    FiledOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OccouredOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Situation = table.Column<string>(type: "text", nullable: false),
                    IncidentReport = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    Recommendation = table.Column<string>(type: "text", nullable: false),
                    ActionTaken = table.Column<bool>(type: "boolean", nullable: false),
                    WillTakeAction = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisciplinaryActions", x => x.DAID);
                    table.ForeignKey(
                        name: "FK_DisciplinaryActions_AspNetUsers_FiledAgainstId",
                        column: x => x.FiledAgainstId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisciplinaryActions_AspNetUsers_FiledById",
                        column: x => x.FiledById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisciplinaryActions_AspNetUsers_FiledToId",
                        column: x => x.FiledToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NickNameChange",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<int>(type: "integer", nullable: false),
                    OldNickname = table.Column<string>(type: "text", nullable: false),
                    NewNickname = table.Column<string>(type: "text", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NickNameChange", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_NickNameChange_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NickNameChange_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualificationChanges",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Added = table.Column<long>(type: "bigint", nullable: false),
                    Removed = table.Column<long>(type: "bigint", nullable: false),
                    Revoked = table.Column<bool>(type: "boolean", nullable: false),
                    OldQualifications = table.Column<long>(type: "bigint", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualificationChanges", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_QualificationChanges_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RankChanges",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangedFrom = table.Column<int>(type: "integer", nullable: false),
                    ChangedTo = table.Column<int>(type: "integer", nullable: false),
                    ChangedById = table.Column<int>(type: "integer", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankChanges", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_RankChanges_AspNetUsers_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RankChanges_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentChanges",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecruitedById = table.Column<int>(type: "integer", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentChanges", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_RecruitmentChanges_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecruitmentChanges_AspNetUsers_RecruitedById",
                        column: x => x.RecruitedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecruitStatuses",
                columns: table => new
                {
                    RecruitStatusKey = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OverSixteen = table.Column<bool>(type: "boolean", nullable: false),
                    ModsInstalled = table.Column<bool>(type: "boolean", nullable: false),
                    TrooperId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitStatuses", x => x.RecruitStatusKey);
                    table.ForeignKey(
                        name: "FK_RecruitStatuses_AspNetUsers_TrooperId",
                        column: x => x.TrooperId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlotChanges",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewSlot = table.Column<int>(type: "integer", nullable: false),
                    NewTeam = table.Column<int>(type: "integer", nullable: true),
                    NewRole = table.Column<int>(type: "integer", nullable: true),
                    NewFlight = table.Column<int>(type: "integer", nullable: true),
                    OldSlot = table.Column<int>(type: "integer", nullable: false),
                    OldTeam = table.Column<int>(type: "integer", nullable: true),
                    OldRole = table.Column<int>(type: "integer", nullable: true),
                    OldFlight = table.Column<int>(type: "integer", nullable: true),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotChanges", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_SlotChanges_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrooperFlags",
                columns: table => new
                {
                    FlagId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: false),
                    FlagForId = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Contents = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrooperFlags", x => x.FlagId);
                    table.ForeignKey(
                        name: "FK_TrooperFlags_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrooperFlags_AspNetUsers_FlagForId",
                        column: x => x.FlagForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisciplinaryActionTrooper",
                columns: table => new
                {
                    WitnessedDisciplinaryActionsDAID = table.Column<Guid>(type: "uuid", nullable: false),
                    WitnessesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisciplinaryActionTrooper", x => new { x.WitnessedDisciplinaryActionsDAID, x.WitnessesId });
                    table.ForeignKey(
                        name: "FK_DisciplinaryActionTrooper_AspNetUsers_WitnessesId",
                        column: x => x.WitnessesId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisciplinaryActionTrooper_DisciplinaryActions_WitnessedDisc~",
                        column: x => x.WitnessedDisciplinaryActionsDAID,
                        principalTable: "DisciplinaryActions",
                        principalColumn: "DAID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualificationChangeTrooper",
                columns: table => new
                {
                    InstructorsId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedQualificationChangesChangeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualificationChangeTrooper", x => new { x.InstructorsId, x.SubmittedQualificationChangesChangeId });
                    table.ForeignKey(
                        name: "FK_QualificationChangeTrooper_AspNetUsers_InstructorsId",
                        column: x => x.InstructorsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualificationChangeTrooper_QualificationChanges_SubmittedQu~",
                        column: x => x.SubmittedQualificationChangesChangeId,
                        principalTable: "QualificationChanges",
                        principalColumn: "ChangeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlotChangeTrooper",
                columns: table => new
                {
                    ApprovedById = table.Column<int>(type: "integer", nullable: false),
                    ApprovedSlotChangesChangeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotChangeTrooper", x => new { x.ApprovedById, x.ApprovedSlotChangesChangeId });
                    table.ForeignKey(
                        name: "FK_SlotChangeTrooper_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SlotChangeTrooper_SlotChanges_ApprovedSlotChangesChangeId",
                        column: x => x.ApprovedSlotChangesChangeId,
                        principalTable: "SlotChanges",
                        principalColumn: "ChangeId",
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
                name: "IX_CShopChanges_ChangedById",
                table: "CShopChanges",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_CShopChanges_ChangedForId",
                table: "CShopChanges",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplinaryActions_FiledAgainstId",
                table: "DisciplinaryActions",
                column: "FiledAgainstId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplinaryActions_FiledById",
                table: "DisciplinaryActions",
                column: "FiledById");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplinaryActions_FiledToId",
                table: "DisciplinaryActions",
                column: "FiledToId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplinaryActionTrooper_WitnessesId",
                table: "DisciplinaryActionTrooper",
                column: "WitnessesId");

            migrationBuilder.CreateIndex(
                name: "IX_NickNameChange_ApprovedById",
                table: "NickNameChange",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_NickNameChange_ChangedForId",
                table: "NickNameChange",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_QualificationChanges_ChangedForId",
                table: "QualificationChanges",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_QualificationChangeTrooper_SubmittedQualificationChangesCha~",
                table: "QualificationChangeTrooper",
                column: "SubmittedQualificationChangesChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_RankChanges_ChangedById",
                table: "RankChanges",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_RankChanges_ChangedForId",
                table: "RankChanges",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentChanges_ChangedForId",
                table: "RecruitmentChanges",
                column: "ChangedForId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentChanges_RecruitedById",
                table: "RecruitmentChanges",
                column: "RecruitedById");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitStatuses_TrooperId",
                table: "RecruitStatuses",
                column: "TrooperId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SlotChanges_ChangedForId",
                table: "SlotChanges",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotChangeTrooper_ApprovedSlotChangesChangeId",
                table: "SlotChangeTrooper",
                column: "ApprovedSlotChangesChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrooperFlags_AuthorId",
                table: "TrooperFlags",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TrooperFlags_FlagForId",
                table: "TrooperFlags",
                column: "FlagForId");
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
                name: "CShopChanges");

            migrationBuilder.DropTable(
                name: "DisciplinaryActionTrooper");

            migrationBuilder.DropTable(
                name: "NickNameChange");

            migrationBuilder.DropTable(
                name: "QualificationChangeTrooper");

            migrationBuilder.DropTable(
                name: "RankChanges");

            migrationBuilder.DropTable(
                name: "RecruitmentChanges");

            migrationBuilder.DropTable(
                name: "RecruitStatuses");

            migrationBuilder.DropTable(
                name: "SlotChangeTrooper");

            migrationBuilder.DropTable(
                name: "TrooperFlags");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DisciplinaryActions");

            migrationBuilder.DropTable(
                name: "QualificationChanges");

            migrationBuilder.DropTable(
                name: "SlotChanges");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
