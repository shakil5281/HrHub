using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCompanyManyToManyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1132d091-6aca-4fdd-803b-10ed27dbff41");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "29ade77d-847b-4608-a355-cf7db8661052");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "650d912c-64ba-4045-a66d-99331026490b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b239be87-69de-476f-8634-62e89b0b282c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b383423b-42e8-4ef4-a1a5-989eeeea6167");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c4c57ce1-4e3b-4fb6-a3f3-d66e21652a0d");

            migrationBuilder.CreateTable(
                name: "UserCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    AssignedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompanies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "21846bf9-bf00-4ede-80ae-c1e5f2c4ca46", "9c2ee918-ff2a-4fc3-8d10-5429539c9f5e", new DateTime(2025, 10, 2, 8, 44, 27, 330, DateTimeKind.Utc).AddTicks(9099), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "28e1ef77-21c7-470a-8826-8bc98228c595", "68805199-7111-4f85-8786-38329557b0e5", new DateTime(2025, 10, 2, 8, 44, 27, 330, DateTimeKind.Utc).AddTicks(9092), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "386b3871-d112-4fd6-91d0-fd529262cab8", "58d7e07d-e0a8-4aee-866b-4a2aba6f27e8", new DateTime(2025, 10, 2, 8, 44, 27, 330, DateTimeKind.Utc).AddTicks(9107), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "6a318562-40e3-4d26-b562-af2d17b4d5f8", "7d6ce000-9dec-4f13-81fa-f1cd7231bca4", new DateTime(2025, 10, 2, 8, 44, 27, 330, DateTimeKind.Utc).AddTicks(9188), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "b455936e-3524-40d4-b2e7-1925290106d1", "37175c9f-b394-47ac-b9dd-e57640d7d8b3", new DateTime(2025, 10, 2, 8, 44, 27, 330, DateTimeKind.Utc).AddTicks(9194), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "d572ac9d-8ca0-41f3-b66b-eaa0db7a91de", "afb2af6d-0a5a-4012-a617-522ab3361c1d", new DateTime(2025, 10, 2, 8, 44, 27, 330, DateTimeKind.Utc).AddTicks(9200), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_CompanyId",
                table: "UserCompanies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_IsActive",
                table: "UserCompanies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_UserId",
                table: "UserCompanies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompany_UserId_CompanyId",
                table: "UserCompanies",
                columns: new[] { "UserId", "CompanyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCompanies");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21846bf9-bf00-4ede-80ae-c1e5f2c4ca46");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "28e1ef77-21c7-470a-8826-8bc98228c595");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "386b3871-d112-4fd6-91d0-fd529262cab8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6a318562-40e3-4d26-b562-af2d17b4d5f8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b455936e-3524-40d4-b2e7-1925290106d1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d572ac9d-8ca0-41f3-b66b-eaa0db7a91de");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "1132d091-6aca-4fdd-803b-10ed27dbff41", "64f1f7e5-2243-47d8-8af9-0cc2c03ddce7", new DateTime(2025, 10, 2, 8, 15, 12, 388, DateTimeKind.Utc).AddTicks(9585), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "29ade77d-847b-4608-a355-cf7db8661052", "f2b7ae48-01dc-4340-bcc2-bcd4155629dc", new DateTime(2025, 10, 2, 8, 15, 12, 388, DateTimeKind.Utc).AddTicks(9591), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "650d912c-64ba-4045-a66d-99331026490b", "c5794890-4a44-4041-99ad-f96b6ded7d80", new DateTime(2025, 10, 2, 8, 15, 12, 388, DateTimeKind.Utc).AddTicks(9570), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "b239be87-69de-476f-8634-62e89b0b282c", "d267b577-2f26-4ce4-88df-1d6beac0d759", new DateTime(2025, 10, 2, 8, 15, 12, 388, DateTimeKind.Utc).AddTicks(9580), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "b383423b-42e8-4ef4-a1a5-989eeeea6167", "8be6da5b-3c17-40ec-8fea-a5e007b06769", new DateTime(2025, 10, 2, 8, 15, 12, 388, DateTimeKind.Utc).AddTicks(9605), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "c4c57ce1-4e3b-4fb6-a3f3-d66e21652a0d", "aea85b8e-dcf0-4395-825b-8fd61ba1bd20", new DateTime(2025, 10, 2, 8, 15, 12, 388, DateTimeKind.Utc).AddTicks(9597), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null }
                });
        }
    }
}
