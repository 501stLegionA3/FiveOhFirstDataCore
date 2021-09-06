using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class PhaseOneInit : Migration
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
                    InitialTraining = table.Column<string>(type: "text", nullable: true),
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
                name: "NoticeBoards",
                columns: table => new
                {
                    Location = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeBoards", x => x.Location);
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
                name: "ClaimUpdates",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AutomaticChange = table.Column<bool>(type: "boolean", nullable: false),
                    ChangedById = table.Column<int>(type: "integer", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false),
                    RevertChange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimUpdates", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_ClaimUpdates_AspNetUsers_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimUpdates_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CShopUpdates",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Added = table.Column<long>(type: "bigint", nullable: false),
                    Removed = table.Column<long>(type: "bigint", nullable: false),
                    OldCShops = table.Column<long>(type: "bigint", nullable: false),
                    ChangedById = table.Column<int>(type: "integer", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false),
                    RevertChange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CShopUpdates", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_CShopUpdates_AspNetUsers_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CShopUpdates_AspNetUsers_ChangedForId",
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
                    OccurredOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                name: "NickNameUpdate",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<int>(type: "integer", nullable: false),
                    OldNickname = table.Column<string>(type: "text", nullable: false),
                    NewNickname = table.Column<string>(type: "text", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false),
                    RevertChange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NickNameUpdate", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_NickNameUpdate_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NickNameUpdate_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualificationUpdates",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Added = table.Column<long>(type: "bigint", nullable: false),
                    Removed = table.Column<long>(type: "bigint", nullable: false),
                    Revoked = table.Column<bool>(type: "boolean", nullable: false),
                    OldQualifications = table.Column<long>(type: "bigint", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false),
                    RevertChange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualificationUpdates", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_QualificationUpdates_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RankUpdates",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangedFrom = table.Column<int>(type: "integer", nullable: false),
                    ChangedTo = table.Column<int>(type: "integer", nullable: false),
                    ChangedById = table.Column<int>(type: "integer", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false),
                    RevertChange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankUpdates", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_RankUpdates_AspNetUsers_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RankUpdates_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentUpdates",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecruitedById = table.Column<int>(type: "integer", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false),
                    RevertChange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentUpdates", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_RecruitmentUpdates_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecruitmentUpdates_AspNetUsers_RecruitedById",
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
                name: "SlotUpdates",
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
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false),
                    RevertChange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotUpdates", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_SlotUpdates_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeUpdates",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewGraduatedBCT = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OldGraduatedBCT = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NewGraduatedUTC = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OldGraduatedUTC = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NewBilletChange = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OldBilletChange = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NewPromotion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OldPromotion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NewStartOfService = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OldStartOfService = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ChangedById = table.Column<int>(type: "integer", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false),
                    RevertChange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeUpdates", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_TimeUpdates_AspNetUsers_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeUpdates_AspNetUsers_ChangedForId",
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
                name: "Notices",
                columns: table => new
                {
                    NoticeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: false),
                    NoticeBoardName = table.Column<string>(type: "text", nullable: false),
                    PostedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Sticky = table.Column<bool>(type: "boolean", nullable: false),
                    Contents = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notices", x => x.NoticeId);
                    table.ForeignKey(
                        name: "FK_Notices_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notices_NoticeBoards_NoticeBoardName",
                        column: x => x.NoticeBoardName,
                        principalTable: "NoticeBoards",
                        principalColumn: "Location",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClaimUpdateData",
                columns: table => new
                {
                    UpdateKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ClaimUpdateChangeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClaimUpdateChangeId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimUpdateData", x => x.UpdateKey);
                    table.ForeignKey(
                        name: "FK_ClaimUpdateData_ClaimUpdates_ClaimUpdateChangeId",
                        column: x => x.ClaimUpdateChangeId,
                        principalTable: "ClaimUpdates",
                        principalColumn: "ChangeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimUpdateData_ClaimUpdates_ClaimUpdateChangeId1",
                        column: x => x.ClaimUpdateChangeId1,
                        principalTable: "ClaimUpdates",
                        principalColumn: "ChangeId",
                        onDelete: ReferentialAction.Restrict);
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
                name: "QualificationUpdateTrooper",
                columns: table => new
                {
                    InstructorsId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedQualificationUpdatesChangeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualificationUpdateTrooper", x => new { x.InstructorsId, x.SubmittedQualificationUpdatesChangeId });
                    table.ForeignKey(
                        name: "FK_QualificationUpdateTrooper_AspNetUsers_InstructorsId",
                        column: x => x.InstructorsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualificationUpdateTrooper_QualificationUpdates_SubmittedQu~",
                        column: x => x.SubmittedQualificationUpdatesChangeId,
                        principalTable: "QualificationUpdates",
                        principalColumn: "ChangeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlotUpdateTrooper",
                columns: table => new
                {
                    ApprovedById = table.Column<int>(type: "integer", nullable: false),
                    ApprovedSlotUpdatesChangeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotUpdateTrooper", x => new { x.ApprovedById, x.ApprovedSlotUpdatesChangeId });
                    table.ForeignKey(
                        name: "FK_SlotUpdateTrooper_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SlotUpdateTrooper_SlotUpdates_ApprovedSlotUpdatesChangeId",
                        column: x => x.ApprovedSlotUpdatesChangeId,
                        principalTable: "SlotUpdates",
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
                name: "IX_ClaimUpdateData_ClaimUpdateChangeId",
                table: "ClaimUpdateData",
                column: "ClaimUpdateChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimUpdateData_ClaimUpdateChangeId1",
                table: "ClaimUpdateData",
                column: "ClaimUpdateChangeId1");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimUpdates_ChangedById",
                table: "ClaimUpdates",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimUpdates_ChangedForId",
                table: "ClaimUpdates",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_CShopUpdates_ChangedById",
                table: "CShopUpdates",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_CShopUpdates_ChangedForId",
                table: "CShopUpdates",
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
                name: "IX_NickNameUpdate_ApprovedById",
                table: "NickNameUpdate",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_NickNameUpdate_ChangedForId",
                table: "NickNameUpdate",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_Notices_AuthorId",
                table: "Notices",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Notices_NoticeBoardName",
                table: "Notices",
                column: "NoticeBoardName");

            migrationBuilder.CreateIndex(
                name: "IX_QualificationUpdates_ChangedForId",
                table: "QualificationUpdates",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_QualificationUpdateTrooper_SubmittedQualificationUpdatesCha~",
                table: "QualificationUpdateTrooper",
                column: "SubmittedQualificationUpdatesChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_RankUpdates_ChangedById",
                table: "RankUpdates",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_RankUpdates_ChangedForId",
                table: "RankUpdates",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentUpdates_ChangedForId",
                table: "RecruitmentUpdates",
                column: "ChangedForId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentUpdates_RecruitedById",
                table: "RecruitmentUpdates",
                column: "RecruitedById");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitStatuses_TrooperId",
                table: "RecruitStatuses",
                column: "TrooperId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SlotUpdates_ChangedForId",
                table: "SlotUpdates",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotUpdateTrooper_ApprovedSlotUpdatesChangeId",
                table: "SlotUpdateTrooper",
                column: "ApprovedSlotUpdatesChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeUpdates_ChangedById",
                table: "TimeUpdates",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_TimeUpdates_ChangedForId",
                table: "TimeUpdates",
                column: "ChangedForId");

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
                name: "ClaimUpdateData");

            migrationBuilder.DropTable(
                name: "CShopUpdates");

            migrationBuilder.DropTable(
                name: "DisciplinaryActionTrooper");

            migrationBuilder.DropTable(
                name: "NickNameUpdate");

            migrationBuilder.DropTable(
                name: "Notices");

            migrationBuilder.DropTable(
                name: "QualificationUpdateTrooper");

            migrationBuilder.DropTable(
                name: "RankUpdates");

            migrationBuilder.DropTable(
                name: "RecruitmentUpdates");

            migrationBuilder.DropTable(
                name: "RecruitStatuses");

            migrationBuilder.DropTable(
                name: "SlotUpdateTrooper");

            migrationBuilder.DropTable(
                name: "TimeUpdates");

            migrationBuilder.DropTable(
                name: "TrooperFlags");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ClaimUpdates");

            migrationBuilder.DropTable(
                name: "DisciplinaryActions");

            migrationBuilder.DropTable(
                name: "NoticeBoards");

            migrationBuilder.DropTable(
                name: "QualificationUpdates");

            migrationBuilder.DropTable(
                name: "SlotUpdates");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
