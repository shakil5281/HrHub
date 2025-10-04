using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRosterScheduleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "RosterSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StatusBangla = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NotesBangla = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OvertimeHours = table.Column<TimeSpan>(type: "time", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RosterSchedules_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RosterSchedules_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RosterSchedules_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedule_CompanyId_ScheduleDate",
                table: "RosterSchedules",
                columns: new[] { "CompanyId", "ScheduleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedule_EmployeeId_ScheduleDate",
                table: "RosterSchedules",
                columns: new[] { "EmployeeId", "ScheduleDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedule_ShiftId_ScheduleDate",
                table: "RosterSchedules",
                columns: new[] { "ShiftId", "ScheduleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedules_CheckInTime",
                table: "RosterSchedules",
                column: "CheckInTime");

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedules_CheckOutTime",
                table: "RosterSchedules",
                column: "CheckOutTime");

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedules_CompanyId",
                table: "RosterSchedules",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedules_EmployeeId",
                table: "RosterSchedules",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedules_IsActive",
                table: "RosterSchedules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedules_ScheduleDate",
                table: "RosterSchedules",
                column: "ScheduleDate");

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedules_ShiftId",
                table: "RosterSchedules",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterSchedules_Status",
                table: "RosterSchedules",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RosterSchedules");

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
        }
    }
}
