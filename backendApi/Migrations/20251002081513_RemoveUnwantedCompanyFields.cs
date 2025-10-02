using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnwantedCompanyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "EmployeeCount",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "EstablishedDate",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Companies");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "EmployeeCount",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstablishedDate",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Companies",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
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
    }
}
