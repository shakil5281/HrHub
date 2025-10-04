using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIdToDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "303e2161-163f-4b21-9e20-cb5fe5306e10");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5579a48f-c19d-44a8-bd0c-3fcbbf77529e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "93148183-7e5d-4794-84c4-6881ac4313d3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad4b8c38-2d75-49ad-97ab-07d83864fd68");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba3fc9ee-cde2-470b-a950-acb08cd3bf15");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c667b719-ca6f-455c-8f19-9739c2bd83fd");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Department_CompanyId_Name",
                table: "Departments",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId",
                table: "Departments",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Companies_CompanyId",
                table: "Departments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Companies_CompanyId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Department_CompanyId_Name",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_CompanyId",
                table: "Departments");

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

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Departments");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "303e2161-163f-4b21-9e20-cb5fe5306e10", "cce0d963-6359-4e2f-9cdb-86c6639d91eb", new DateTime(2025, 10, 2, 14, 53, 29, 740, DateTimeKind.Utc).AddTicks(9016), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "5579a48f-c19d-44a8-bd0c-3fcbbf77529e", "263930f9-42e4-420b-af45-b476a3a8567a", new DateTime(2025, 10, 2, 14, 53, 29, 740, DateTimeKind.Utc).AddTicks(8964), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "93148183-7e5d-4794-84c4-6881ac4313d3", "5148b20a-3d37-4f81-bfc3-dbce0a8a4db3", new DateTime(2025, 10, 2, 14, 53, 29, 740, DateTimeKind.Utc).AddTicks(8956), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "ad4b8c38-2d75-49ad-97ab-07d83864fd68", "ff4a2fd5-447f-4b75-b2cc-cd9bca79ad87", new DateTime(2025, 10, 2, 14, 53, 29, 740, DateTimeKind.Utc).AddTicks(8987), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "ba3fc9ee-cde2-470b-a950-acb08cd3bf15", "0f598794-4086-4118-b5a5-bf232d27f5e0", new DateTime(2025, 10, 2, 14, 53, 29, 740, DateTimeKind.Utc).AddTicks(8978), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "c667b719-ca6f-455c-8f19-9739c2bd83fd", "a0ad9758-f06a-4cf7-b470-b2234ba86425", new DateTime(2025, 10, 2, 14, 53, 29, 740, DateTimeKind.Utc).AddTicks(8971), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null }
                });
        }
    }
}
