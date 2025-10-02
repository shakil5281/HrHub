using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNewRoles_IT_HR_HRManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2ba2c5d7-6965-4d57-9aca-9e8e91c08ec8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "89d416fc-1607-4413-903d-2f6d9b9dac28");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b4f9f22e-8047-41f5-9fb0-1ded3ac2ee3d");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "2ba2c5d7-6965-4d57-9aca-9e8e91c08ec8", "d3f2cd8d-fdfb-4083-a725-d4a61c048d69", new DateTime(2025, 10, 1, 17, 20, 4, 449, DateTimeKind.Utc).AddTicks(9927), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "89d416fc-1607-4413-903d-2f6d9b9dac28", "a240692c-85f1-41aa-9aee-ea1d5e8d093e", new DateTime(2025, 10, 1, 17, 20, 4, 449, DateTimeKind.Utc).AddTicks(9911), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "b4f9f22e-8047-41f5-9fb0-1ded3ac2ee3d", "e3001dc8-7906-44e7-9426-8f9a30209a92", new DateTime(2025, 10, 1, 17, 20, 4, 449, DateTimeKind.Utc).AddTicks(9902), "System Administrator with full access", true, "Admin", "ADMIN", null }
                });
        }
    }
}
