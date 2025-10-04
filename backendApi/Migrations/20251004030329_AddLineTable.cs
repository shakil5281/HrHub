using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddLineTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "03b9fbdd-2c19-493c-8478-8bc4b5a9c194");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "124156f3-41a5-417f-81f9-0b26fff0dc54");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "180e4bf0-5bc4-4807-9b89-26bd12977f3a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3e3cce22-cbd4-4b4d-9ee0-eccdd623aa6e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8322bb13-f535-4b9c-b204-53a65cf9a3c7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "96a38a37-8ebf-427d-9bd7-d1aa0844e400");

            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameBangla = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lines_Companies_CompanyId",
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
                    { "416f5d81-1cdf-4f14-bb39-2541cdc7ff44", "0d51302e-a363-4b66-97b5-6cb82a6e7707", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9705), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "44311961-1b8a-4260-8d7e-f52ca9d408b7", "08a338b3-dcf9-4b8e-9882-34ab9e4dee0f", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9727), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "59aa3a43-9a6d-459b-9f89-870617bba693", "0dc411f9-b029-4d75-bc44-c67890a969a7", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9733), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "85709f65-a95a-4208-9638-f17a71c5c373", "230c1489-dfa3-43eb-a17d-7d36f8112d28", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9715), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "8d17de3f-c0b9-4553-902e-e413a4524293", "4338b148-f6ee-4891-8177-acc87f6fee7b", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9741), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "9580912e-a57f-4661-8d1e-2782ea83e6e5", "0d0452b1-6be3-4b00-80e5-5b39777b83ac", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9721), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Line_CompanyId_Name",
                table: "Lines",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lines_CompanyId",
                table: "Lines",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Lines_IsActive",
                table: "Lines",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Lines_Name",
                table: "Lines",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "416f5d81-1cdf-4f14-bb39-2541cdc7ff44");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "44311961-1b8a-4260-8d7e-f52ca9d408b7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "59aa3a43-9a6d-459b-9f89-870617bba693");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "85709f65-a95a-4208-9638-f17a71c5c373");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8d17de3f-c0b9-4553-902e-e413a4524293");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9580912e-a57f-4661-8d1e-2782ea83e6e5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "03b9fbdd-2c19-493c-8478-8bc4b5a9c194", "45d50ee2-17d0-4447-bf31-d56f9e177b47", new DateTime(2025, 10, 4, 2, 39, 22, 204, DateTimeKind.Utc).AddTicks(3353), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "124156f3-41a5-417f-81f9-0b26fff0dc54", "d6f860cf-3700-4c0e-907a-575c70e66e6f", new DateTime(2025, 10, 4, 2, 39, 22, 204, DateTimeKind.Utc).AddTicks(3359), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "180e4bf0-5bc4-4807-9b89-26bd12977f3a", "f77adfae-ad63-4750-a3f9-d098ccf154cc", new DateTime(2025, 10, 4, 2, 39, 22, 204, DateTimeKind.Utc).AddTicks(3292), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "3e3cce22-cbd4-4b4d-9ee0-eccdd623aa6e", "e4a27bac-5e10-4c28-8c91-d4d113f6206f", new DateTime(2025, 10, 4, 2, 39, 22, 204, DateTimeKind.Utc).AddTicks(3364), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "8322bb13-f535-4b9c-b204-53a65cf9a3c7", "e7f0b545-0372-415e-bd0d-ab4d9b69ba16", new DateTime(2025, 10, 4, 2, 39, 22, 204, DateTimeKind.Utc).AddTicks(3283), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "96a38a37-8ebf-427d-9bd7-d1aa0844e400", "a7c831f2-4ccd-400c-8454-19bf502c4bd8", new DateTime(2025, 10, 4, 2, 39, 22, 204, DateTimeKind.Utc).AddTicks(3275), "System Administrator with full access", true, "Admin", "ADMIN", null }
                });
        }
    }
}
