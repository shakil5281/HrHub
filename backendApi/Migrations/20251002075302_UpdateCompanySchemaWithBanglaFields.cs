using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompanySchemaWithBanglaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3e4c438b-caaa-4c4c-a696-e9fb50dbbc8d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "634b8283-8e81-49a9-a0c2-5b4f4ddebdb5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6c2708f9-cde8-4c45-a6a5-f4d2d614162e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "82f7a2a0-103b-43d1-a776-bb06dd8dbf9e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "da891190-d3f0-4539-8c30-336a71349f40");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f468da76-742a-41d9-9720-b0af9b8b5d2c");

            migrationBuilder.AddColumn<string>(
                name: "AddressBangla",
                table: "Companies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorizedSignature",
                table: "Companies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyNameBangla",
                table: "Companies",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "16840147-8ec8-4ab7-892f-653eb6776869", "fbf14cac-a64d-4c4d-b772-ea8ed5b27bad", new DateTime(2025, 10, 2, 7, 53, 0, 320, DateTimeKind.Utc).AddTicks(6993), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "395aa6b9-6290-4eb1-8570-44ee2c550206", "1ce76013-cc21-428e-b5a8-992a97eb7b7c", new DateTime(2025, 10, 2, 7, 53, 0, 320, DateTimeKind.Utc).AddTicks(6983), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "55376fbe-62b4-47f3-ad41-18a6a783e95e", "e623ef80-27ec-4788-88e7-57e8f28207d9", new DateTime(2025, 10, 2, 7, 53, 0, 320, DateTimeKind.Utc).AddTicks(6977), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "69ca3e4e-5084-466f-abc5-8bca66f9450a", "c841446b-cfcc-4cb3-9a2d-56574c9f4b20", new DateTime(2025, 10, 2, 7, 53, 0, 320, DateTimeKind.Utc).AddTicks(6971), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "70629a7a-d36e-4763-8ca1-20df3611a94c", "543df085-21b6-4bf0-9fc1-c0ca39eba33e", new DateTime(2025, 10, 2, 7, 53, 0, 320, DateTimeKind.Utc).AddTicks(6964), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "9dd6c94a-b4bf-4bb4-a638-d8d6ba5a157f", "c8b8aa4d-cc85-4eda-ac5d-011ebe5f3f0e", new DateTime(2025, 10, 2, 7, 53, 0, 320, DateTimeKind.Utc).AddTicks(6999), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "16840147-8ec8-4ab7-892f-653eb6776869");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "395aa6b9-6290-4eb1-8570-44ee2c550206");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55376fbe-62b4-47f3-ad41-18a6a783e95e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "69ca3e4e-5084-466f-abc5-8bca66f9450a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "70629a7a-d36e-4763-8ca1-20df3611a94c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9dd6c94a-b4bf-4bb4-a638-d8d6ba5a157f");

            migrationBuilder.DropColumn(
                name: "AddressBangla",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "AuthorizedSignature",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyNameBangla",
                table: "Companies");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "3e4c438b-caaa-4c4c-a696-e9fb50dbbc8d", "4778059d-4997-4227-9ff0-116012c9e519", new DateTime(2025, 10, 2, 2, 27, 8, 804, DateTimeKind.Utc).AddTicks(5401), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "634b8283-8e81-49a9-a0c2-5b4f4ddebdb5", "5bddd107-9ebb-47e9-a87b-c0cd41efb7cf", new DateTime(2025, 10, 2, 2, 27, 8, 804, DateTimeKind.Utc).AddTicks(5433), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "6c2708f9-cde8-4c45-a6a5-f4d2d614162e", "73b218bc-b679-4cb7-8dbb-5c3bf0ace849", new DateTime(2025, 10, 2, 2, 27, 8, 804, DateTimeKind.Utc).AddTicks(5428), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "82f7a2a0-103b-43d1-a776-bb06dd8dbf9e", "06ea2aa7-5a65-49ab-a387-dae88f7b7aa1", new DateTime(2025, 10, 2, 2, 27, 8, 804, DateTimeKind.Utc).AddTicks(5414), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "da891190-d3f0-4539-8c30-336a71349f40", "9bfdb8af-3637-4a30-a324-b9eb4d3a3c83", new DateTime(2025, 10, 2, 2, 27, 8, 804, DateTimeKind.Utc).AddTicks(5420), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "f468da76-742a-41d9-9720-b0af9b8b5d2c", "c7ab5e77-27b3-4157-9ab9-34c9772826c5", new DateTime(2025, 10, 2, 2, 27, 8, 804, DateTimeKind.Utc).AddTicks(5409), "Manager with limited administrative access", true, "Manager", "MANAGER", null }
                });
        }
    }
}
