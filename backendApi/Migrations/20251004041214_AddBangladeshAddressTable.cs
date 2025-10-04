using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBangladeshAddressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13362f20-884b-4932-a7e7-25192482acb4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e5cc77d-aacf-4ef5-ac4c-b70c74bbeed5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a8ce163-a629-429d-a4b3-505c32838e99");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2629ae0-b03c-41ee-897f-a24bea1606d1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dbdfa7dd-ffb6-43d2-a2fb-461f47331df2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dd1b538c-1253-4bb0-ad50-2cebe4784127");

            migrationBuilder.CreateTable(
                name: "BangladeshAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Division = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DivisionBangla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DistrictBangla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Upazila = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpazilaBangla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Union = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UnionBangla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Area = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AreaBangla = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangladeshAddresses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "35449e5d-d913-49e2-a34a-9b642e9aaa23", "4964b214-50b7-45d2-b2ca-e7e14c19ec8c", new DateTime(2025, 10, 4, 4, 12, 13, 996, DateTimeKind.Utc).AddTicks(7387), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "423a1ad8-837f-4b81-b855-c4bead6f4c9d", "e5457540-04a4-41eb-ac5a-22f1fa2f0182", new DateTime(2025, 10, 4, 4, 12, 13, 996, DateTimeKind.Utc).AddTicks(7399), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "5c0d8263-124a-455f-9ccc-34a2a0baaa8d", "f3acce86-182a-423f-81b9-b8d20dac160a", new DateTime(2025, 10, 4, 4, 12, 13, 996, DateTimeKind.Utc).AddTicks(7315), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "c92dc3a2-246a-40de-ab87-28fbac473902", "ec0dd5f5-44ef-4039-a883-c09bf5726082", new DateTime(2025, 10, 4, 4, 12, 13, 996, DateTimeKind.Utc).AddTicks(7393), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "cb63951f-32e7-43f6-a3c8-9c2f3ff9a159", "b1207ce7-3aba-43ba-8c54-345bc183cf1e", new DateTime(2025, 10, 4, 4, 12, 13, 996, DateTimeKind.Utc).AddTicks(7405), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "f6d16203-2ddd-401b-93d3-27ebf879dbbd", "f282d601-30e5-431c-a49c-ad1b8392721c", new DateTime(2025, 10, 4, 4, 12, 13, 996, DateTimeKind.Utc).AddTicks(7377), "Manager with limited administrative access", true, "Manager", "MANAGER", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangladeshAddress_District_PostalCode",
                table: "BangladeshAddresses",
                columns: new[] { "District", "PostalCode" });

            migrationBuilder.CreateIndex(
                name: "IX_BangladeshAddress_Division_District",
                table: "BangladeshAddresses",
                columns: new[] { "Division", "District" });

            migrationBuilder.CreateIndex(
                name: "IX_BangladeshAddress_Division_District_Upazila",
                table: "BangladeshAddresses",
                columns: new[] { "Division", "District", "Upazila" });

            migrationBuilder.CreateIndex(
                name: "IX_BangladeshAddress_PostalCode_Unique",
                table: "BangladeshAddresses",
                column: "PostalCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BangladeshAddresses_District",
                table: "BangladeshAddresses",
                column: "District");

            migrationBuilder.CreateIndex(
                name: "IX_BangladeshAddresses_Division",
                table: "BangladeshAddresses",
                column: "Division");

            migrationBuilder.CreateIndex(
                name: "IX_BangladeshAddresses_IsActive",
                table: "BangladeshAddresses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_BangladeshAddresses_Union",
                table: "BangladeshAddresses",
                column: "Union");

            migrationBuilder.CreateIndex(
                name: "IX_BangladeshAddresses_Upazila",
                table: "BangladeshAddresses",
                column: "Upazila");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangladeshAddresses");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "35449e5d-d913-49e2-a34a-9b642e9aaa23");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "423a1ad8-837f-4b81-b855-c4bead6f4c9d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5c0d8263-124a-455f-9ccc-34a2a0baaa8d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c92dc3a2-246a-40de-ab87-28fbac473902");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cb63951f-32e7-43f6-a3c8-9c2f3ff9a159");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f6d16203-2ddd-401b-93d3-27ebf879dbbd");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "13362f20-884b-4932-a7e7-25192482acb4", "83783cbf-91df-4032-a00b-e166ac045d39", new DateTime(2025, 10, 4, 4, 1, 38, 467, DateTimeKind.Utc).AddTicks(3879), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "7e5cc77d-aacf-4ef5-ac4c-b70c74bbeed5", "e7d859be-5487-42e4-a76c-f946f60f626d", new DateTime(2025, 10, 4, 4, 1, 38, 467, DateTimeKind.Utc).AddTicks(3856), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "8a8ce163-a629-429d-a4b3-505c32838e99", "cecec77a-e5c3-44c2-90e4-547793f6c53d", new DateTime(2025, 10, 4, 4, 1, 38, 467, DateTimeKind.Utc).AddTicks(3872), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "a2629ae0-b03c-41ee-897f-a24bea1606d1", "6c0d1159-1e11-41b9-a8c4-ca3eacc3b7f0", new DateTime(2025, 10, 4, 4, 1, 38, 467, DateTimeKind.Utc).AddTicks(3862), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "dbdfa7dd-ffb6-43d2-a2fb-461f47331df2", "e3a13a13-186b-4d82-b329-f41802409ca2", new DateTime(2025, 10, 4, 4, 1, 38, 467, DateTimeKind.Utc).AddTicks(3841), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "dd1b538c-1253-4bb0-ad50-2cebe4784127", "06228992-6296-4428-8347-1af21b10e45d", new DateTime(2025, 10, 4, 4, 1, 38, 467, DateTimeKind.Utc).AddTicks(3849), "Manager with limited administrative access", true, "Manager", "MANAGER", null }
                });
        }
    }
}
