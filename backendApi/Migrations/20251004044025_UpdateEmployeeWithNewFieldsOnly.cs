using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeWithNewFieldsOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "Employees",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Conveyance",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DegreeId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Employees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpId",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmpType",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Floor",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Food",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "House",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LineId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MobileBill",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NightBill",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAllowance",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Religion",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RentMedical",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalaryType",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShiftId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Transport",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Bank",
                table: "Employees",
                column: "Bank");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BloodGroup",
                table: "Employees",
                column: "BloodGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DegreeId",
                table: "Employees",
                column: "DegreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmpId",
                table: "Employees",
                column: "EmpId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmpType",
                table: "Employees",
                column: "EmpType");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Gender",
                table: "Employees",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Group",
                table: "Employees",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LineId",
                table: "Employees",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_MaritalStatus",
                table: "Employees",
                column: "MaritalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Religion",
                table: "Employees",
                column: "Religion");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SalaryType",
                table: "Employees",
                column: "SalaryType");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ShiftId",
                table: "Employees",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                table: "Employees",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Degrees_DegreeId",
                table: "Employees",
                column: "DegreeId",
                principalTable: "Degrees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Lines_LineId",
                table: "Employees",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Shifts_ShiftId",
                table: "Employees",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Degrees_DegreeId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Lines_LineId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Shifts_ShiftId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Bank",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_BloodGroup",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DegreeId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_EmpId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_EmpType",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Gender",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Group",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_LineId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_MaritalStatus",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Religion",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_SalaryType",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ShiftId",
                table: "Employees");

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

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Conveyance",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DegreeId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmpType",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Floor",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Food",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "House",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LineId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "MobileBill",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "NightBill",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "OtherAllowance",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Religion",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RentMedical",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SalaryType",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Transport",
                table: "Employees");

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
        }
    }
}
