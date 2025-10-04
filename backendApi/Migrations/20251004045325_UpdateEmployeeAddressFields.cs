using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a7e1078-a6dd-414c-b7e0-cd447b700620");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c68dd7a-931a-4bc7-affe-f541990c7951");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "63988fac-ff9a-49e3-9569-196fa318af00");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84187ccf-760a-4870-9a3b-4a1c830deecb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9df04efe-2016-4c29-bae8-4cf7c025fa0b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eb79a102-4375-4c73-8bd2-3f2cb2266ebd");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Employees",
                newName: "PresentAddress");

            migrationBuilder.AddColumn<string>(
                name: "PermanentAddress",
                table: "Employees",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PermanentDistrict",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PermanentDivision",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PermanentPostalCode",
                table: "Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PermanentUpazila",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PresentDistrict",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PresentDivision",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PresentPostalCode",
                table: "Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PresentUpazila",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "02133fb0-ae46-486a-9191-ba9f88ea0cd5", "7520895a-64a3-4dd6-a96b-e210185a9d95", new DateTime(2025, 10, 4, 4, 53, 24, 153, DateTimeKind.Utc).AddTicks(1337), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "3529b9cf-9370-427c-975b-9dee4862cef6", "68185036-171e-4514-82a8-6842f987c84f", new DateTime(2025, 10, 4, 4, 53, 24, 153, DateTimeKind.Utc).AddTicks(1358), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "88f15984-7717-4bc8-a2ee-609968d5c5ab", "8e521652-63ba-446a-b42d-46dee8a96de9", new DateTime(2025, 10, 4, 4, 53, 24, 153, DateTimeKind.Utc).AddTicks(1343), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "b94332c6-d77d-416d-98e9-3f7b318ac637", "5f419c1e-b3b3-435b-af12-1a7c6b98c49f", new DateTime(2025, 10, 4, 4, 53, 24, 153, DateTimeKind.Utc).AddTicks(1364), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "c929d4c3-338a-44af-b517-afed5e1e2aa9", "eafd17c6-d2b6-4ef5-ac4d-674561b15cc0", new DateTime(2025, 10, 4, 4, 53, 24, 153, DateTimeKind.Utc).AddTicks(1352), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "ef8d6e12-b77a-4831-a9dc-a4bfb669038c", "8d27aeec-e63f-4163-bd0e-4aea55d2d150", new DateTime(2025, 10, 4, 4, 53, 24, 153, DateTimeKind.Utc).AddTicks(1369), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PermanentDistrict",
                table: "Employees",
                column: "PermanentDistrict");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PermanentDivision",
                table: "Employees",
                column: "PermanentDivision");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PermanentUpazila",
                table: "Employees",
                column: "PermanentUpazila");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PresentDistrict",
                table: "Employees",
                column: "PresentDistrict");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PresentDivision",
                table: "Employees",
                column: "PresentDivision");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PresentUpazila",
                table: "Employees",
                column: "PresentUpazila");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_PermanentDistrict",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PermanentDivision",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PermanentUpazila",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PresentDistrict",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PresentDivision",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PresentUpazila",
                table: "Employees");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02133fb0-ae46-486a-9191-ba9f88ea0cd5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3529b9cf-9370-427c-975b-9dee4862cef6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88f15984-7717-4bc8-a2ee-609968d5c5ab");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b94332c6-d77d-416d-98e9-3f7b318ac637");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c929d4c3-338a-44af-b517-afed5e1e2aa9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef8d6e12-b77a-4831-a9dc-a4bfb669038c");

            migrationBuilder.DropColumn(
                name: "PermanentAddress",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PermanentDistrict",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PermanentDivision",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PermanentPostalCode",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PermanentUpazila",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PresentDistrict",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PresentDivision",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PresentPostalCode",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PresentUpazila",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "PresentAddress",
                table: "Employees",
                newName: "Address");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "1a7e1078-a6dd-414c-b7e0-cd447b700620", "419caa93-fd01-4e00-bc0e-2042b39826c1", new DateTime(2025, 10, 4, 4, 40, 24, 305, DateTimeKind.Utc).AddTicks(3539), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "2c68dd7a-931a-4bc7-affe-f541990c7951", "213f2563-7ad3-4512-9f15-9061cf946e31", new DateTime(2025, 10, 4, 4, 40, 24, 305, DateTimeKind.Utc).AddTicks(3525), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "63988fac-ff9a-49e3-9569-196fa318af00", "a02e5935-601c-4c16-b080-9a79d125fdac", new DateTime(2025, 10, 4, 4, 40, 24, 305, DateTimeKind.Utc).AddTicks(3512), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "84187ccf-760a-4870-9a3b-4a1c830deecb", "68eb2434-0cec-4608-aa6a-8ba568dd2a22", new DateTime(2025, 10, 4, 4, 40, 24, 305, DateTimeKind.Utc).AddTicks(3518), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "9df04efe-2016-4c29-bae8-4cf7c025fa0b", "bf2b4213-c258-4ef1-8dfe-886987008c01", new DateTime(2025, 10, 4, 4, 40, 24, 305, DateTimeKind.Utc).AddTicks(3504), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "eb79a102-4375-4c73-8bd2-3f2cb2266ebd", "d41c3cef-d640-4b96-a262-1bea53ec42a5", new DateTime(2025, 10, 4, 4, 40, 24, 305, DateTimeKind.Utc).AddTicks(3533), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null }
                });
        }
    }
}
