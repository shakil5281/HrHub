using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDegreeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "135e37f0-766f-4153-ae34-be0aa3f0fea7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "40a88446-7473-46f6-b768-f50f85b5bda1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f966ca0-c597-4ace-a499-af4c7bbad78a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "93c1d14e-e7c9-4746-a4e3-c7e96527f69f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c2d93b27-2066-41e3-8d33-e674bc5b57a7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cde51735-bf0e-4880-a4bb-b60b9fb6d101");

            migrationBuilder.CreateTable(
                name: "Degrees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameBangla = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LevelBangla = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InstitutionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstitutionTypeBangla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degrees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Degrees_Companies_CompanyId",
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
                    { "1ede0da2-cdca-4d76-b4e9-f86125d6bff5", "99d0f693-b30a-42cd-af4b-886432b62620", new DateTime(2025, 10, 4, 3, 34, 14, 748, DateTimeKind.Utc).AddTicks(7739), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "341522a3-09cb-4304-b567-d139013c30a9", "21f0169e-8686-4d72-adc3-d0abc9445184", new DateTime(2025, 10, 4, 3, 34, 14, 748, DateTimeKind.Utc).AddTicks(7732), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "8e1f0e8f-0662-4fd0-8eea-f96f6e92ec52", "de3ec4db-4986-46d5-bb7d-be118f841f3e", new DateTime(2025, 10, 4, 3, 34, 14, 748, DateTimeKind.Utc).AddTicks(7718), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "9d1e134a-6fcd-41b5-88db-f23e2a92fbba", "728c5d39-dc83-4692-a548-eab2f6d162f1", new DateTime(2025, 10, 4, 3, 34, 14, 748, DateTimeKind.Utc).AddTicks(7711), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "a73adc80-8ec8-4942-97e7-a713dff10f47", "945af56d-1186-421e-a1ad-e7a0d71bb979", new DateTime(2025, 10, 4, 3, 34, 14, 748, DateTimeKind.Utc).AddTicks(7745), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "a8d91ec2-d43f-4a09-a261-16b1dfe5ba74", "c775e8e5-b1c3-4d1f-b2ad-1822483cda56", new DateTime(2025, 10, 4, 3, 34, 14, 748, DateTimeKind.Utc).AddTicks(7724), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Degree_CompanyId_Name",
                table: "Degrees",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Degrees_CompanyId",
                table: "Degrees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Degrees_InstitutionType",
                table: "Degrees",
                column: "InstitutionType");

            migrationBuilder.CreateIndex(
                name: "IX_Degrees_IsActive",
                table: "Degrees",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Degrees_Level",
                table: "Degrees",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Degrees_Name",
                table: "Degrees",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Degrees");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ede0da2-cdca-4d76-b4e9-f86125d6bff5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "341522a3-09cb-4304-b567-d139013c30a9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8e1f0e8f-0662-4fd0-8eea-f96f6e92ec52");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9d1e134a-6fcd-41b5-88db-f23e2a92fbba");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a73adc80-8ec8-4942-97e7-a713dff10f47");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a8d91ec2-d43f-4a09-a261-16b1dfe5ba74");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "135e37f0-766f-4153-ae34-be0aa3f0fea7", "578001d2-031d-49be-9cd0-5bbcb59b2772", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3826), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "40a88446-7473-46f6-b768-f50f85b5bda1", "d0907b9a-e5cf-4fbb-8af0-36590295d888", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3790), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "8f966ca0-c597-4ace-a499-af4c7bbad78a", "4f655222-2f7f-49cb-a30c-d4508813ef17", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3820), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "93c1d14e-e7c9-4746-a4e3-c7e96527f69f", "3ac9ba16-30c7-4a86-8399-38a7d004f650", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3814), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "c2d93b27-2066-41e3-8d33-e674bc5b57a7", "415a55ee-7ca0-45b3-9c36-f814b2383c12", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3805), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "cde51735-bf0e-4880-a4bb-b60b9fb6d101", "09ad672f-d858-402e-bd96-7c9721ace83a", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3798), "Manager with limited administrative access", true, "Manager", "MANAGER", null }
                });
        }
    }
}
