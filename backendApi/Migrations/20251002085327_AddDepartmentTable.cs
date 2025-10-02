using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameBangla = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "024c6577-5de2-4b99-804b-ef1262152f08", "a9a51418-7dfd-4721-b8b2-1e8d50ffa422", new DateTime(2025, 10, 2, 8, 53, 26, 441, DateTimeKind.Utc).AddTicks(3545), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "3ba20610-5478-4cb3-8b6e-9907d572d9e2", "cfc65713-c246-4dab-a946-bdfafe94bed5", new DateTime(2025, 10, 2, 8, 53, 26, 441, DateTimeKind.Utc).AddTicks(3650), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "4ba89663-eaf3-4291-9cb4-888f4ec65efd", "2dff9e89-1a39-4275-9d9c-0b054dbd7c4f", new DateTime(2025, 10, 2, 8, 53, 26, 441, DateTimeKind.Utc).AddTicks(3529), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "5e349878-47a5-4e21-9dbc-cd224d566d54", "e92af005-b52c-4c78-acd1-2cd04a26879d", new DateTime(2025, 10, 2, 8, 53, 26, 441, DateTimeKind.Utc).AddTicks(3539), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "773bd333-537f-4185-a5dd-f3d828dabf73", "a05548b6-1efc-4364-bade-e38e5ab73692", new DateTime(2025, 10, 2, 8, 53, 26, 441, DateTimeKind.Utc).AddTicks(3642), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "7b772762-145a-43b1-b16d-f4ed31202147", "5323db40-e380-459a-9158-e372c9468b08", new DateTime(2025, 10, 2, 8, 53, 26, 441, DateTimeKind.Utc).AddTicks(3636), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_IsActive",
                table: "Departments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "024c6577-5de2-4b99-804b-ef1262152f08");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3ba20610-5478-4cb3-8b6e-9907d572d9e2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4ba89663-eaf3-4291-9cb4-888f4ec65efd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e349878-47a5-4e21-9dbc-cd224d566d54");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "773bd333-537f-4185-a5dd-f3d828dabf73");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7b772762-145a-43b1-b16d-f4ed31202147");

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
        }
    }
}
