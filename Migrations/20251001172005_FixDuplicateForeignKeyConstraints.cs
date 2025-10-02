using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixDuplicateForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13d43666-4561-488c-b783-7693d4ae0836");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c6d7375d-d923-4e99-8028-6ab2025a6fd4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c9abac5b-906f-4877-bca4-f91251cb878d");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstablishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeCount = table.Column<int>(type: "int", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "2ba2c5d7-6965-4d57-9aca-9e8e91c08ec8", "d3f2cd8d-fdfb-4083-a725-d4a61c048d69", new DateTime(2025, 10, 1, 17, 20, 4, 449, DateTimeKind.Utc).AddTicks(9927), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "89d416fc-1607-4413-903d-2f6d9b9dac28", "a240692c-85f1-41aa-9aee-ea1d5e8d093e", new DateTime(2025, 10, 1, 17, 20, 4, 449, DateTimeKind.Utc).AddTicks(9911), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "b4f9f22e-8047-41f5-9fb0-1ded3ac2ee3d", "e3001dc8-7906-44e7-9426-8f9a30209a92", new DateTime(2025, 10, 1, 17, 20, 4, 449, DateTimeKind.Utc).AddTicks(9902), "System Administrator with full access", true, "Admin", "ADMIN", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_IsActive",
                table: "Companies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers");

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

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "13d43666-4561-488c-b783-7693d4ae0836", "98919653-b8d6-4ddf-8673-522571f745aa", new DateTime(2025, 10, 1, 15, 0, 58, 999, DateTimeKind.Utc).AddTicks(9276), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "c6d7375d-d923-4e99-8028-6ab2025a6fd4", "a851fbc0-35fc-4961-9879-a118d68a1772", new DateTime(2025, 10, 1, 15, 0, 58, 999, DateTimeKind.Utc).AddTicks(9268), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "c9abac5b-906f-4877-bca4-f91251cb878d", "020a489b-c8de-4935-9f5a-bf77d83aad1e", new DateTime(2025, 10, 1, 15, 0, 58, 999, DateTimeKind.Utc).AddTicks(9292), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null }
                });
        }
    }
}
