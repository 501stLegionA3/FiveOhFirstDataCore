using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class DevSoyvolonPolicies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicPolicies",
                columns: table => new
                {
                    PolicyName = table.Column<string>(type: "text", nullable: false),
                    RequiredRoles = table.Column<List<string>>(type: "text[]", nullable: false),
                    EditableByPolicyName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicPolicies", x => x.PolicyName);
                    table.ForeignKey(
                        name: "FK_DynamicPolicies_DynamicPolicies_EditableByPolicyName",
                        column: x => x.EditableByPolicyName,
                        principalTable: "DynamicPolicies",
                        principalColumn: "PolicyName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyClaimData",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    Claim = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    DynamicPolicyPolicyName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyClaimData", x => x.Key);
                    table.ForeignKey(
                        name: "FK_PolicyClaimData_DynamicPolicies_DynamicPolicyPolicyName",
                        column: x => x.DynamicPolicyPolicyName,
                        principalTable: "DynamicPolicies",
                        principalColumn: "PolicyName",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PolicySections",
                columns: table => new
                {
                    SectionName = table.Column<string>(type: "text", nullable: false),
                    PolicyName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicySections", x => x.SectionName);
                    table.ForeignKey(
                        name: "FK_PolicySections_DynamicPolicies_PolicyName",
                        column: x => x.PolicyName,
                        principalTable: "DynamicPolicies",
                        principalColumn: "PolicyName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicPolicies_EditableByPolicyName",
                table: "DynamicPolicies",
                column: "EditableByPolicyName");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyClaimData_DynamicPolicyPolicyName",
                table: "PolicyClaimData",
                column: "DynamicPolicyPolicyName");

            migrationBuilder.CreateIndex(
                name: "IX_PolicySections_PolicyName",
                table: "PolicySections",
                column: "PolicyName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyClaimData");

            migrationBuilder.DropTable(
                name: "PolicySections");

            migrationBuilder.DropTable(
                name: "DynamicPolicies");
        }
    }
}
