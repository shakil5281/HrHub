using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "416f5d81-1cdf-4f14-bb39-2541cdc7ff44");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "44311961-1b8a-4260-8d7e-f52ca9d408b7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "59aa3a43-9a6d-459b-9f89-870617bba693");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "85709f65-a95a-4208-9638-f17a71c5c373");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8d17de3f-c0b9-4553-902e-e413a4524293");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9580912e-a57f-4661-8d1e-2782ea83e6e5");

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameBangla = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    BreakStartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    BreakEndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shifts_Companies_CompanyId",
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
                    { "135e37f0-766f-4153-ae34-be0aa3f0fea7", "578001d2-031d-49be-9cd0-5bbcb59b2772", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3826), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "40a88446-7473-46f6-b768-f50f85b5bda1", "d0907b9a-e5cf-4fbb-8af0-36590295d888", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3790), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "8f966ca0-c597-4ace-a499-af4c7bbad78a", "4f655222-2f7f-49cb-a30c-d4508813ef17", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3820), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "93c1d14e-e7c9-4746-a4e3-c7e96527f69f", "3ac9ba16-30c7-4a86-8399-38a7d004f650", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3814), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "c2d93b27-2066-41e3-8d33-e674bc5b57a7", "415a55ee-7ca0-45b3-9c36-f814b2383c12", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3805), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null },
                    { "cde51735-bf0e-4880-a4bb-b60b9fb6d101", "09ad672f-d858-402e-bd96-7c9721ace83a", new DateTime(2025, 10, 4, 3, 9, 17, 801, DateTimeKind.Utc).AddTicks(3798), "Manager with limited administrative access", true, "Manager", "MANAGER", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shift_CompanyId_Name",
                table: "Shifts",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_CompanyId",
                table: "Shifts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_EndTime",
                table: "Shifts",
                column: "EndTime");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_IsActive",
                table: "Shifts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_Name",
                table: "Shifts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_StartTime",
                table: "Shifts",
                column: "StartTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shifts");

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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "IsActive", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { "416f5d81-1cdf-4f14-bb39-2541cdc7ff44", "0d51302e-a363-4b66-97b5-6cb82a6e7707", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9705), "System Administrator with full access", true, "Admin", "ADMIN", null },
                    { "44311961-1b8a-4260-8d7e-f52ca9d408b7", "08a338b3-dcf9-4b8e-9882-34ab9e4dee0f", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9727), "IT personnel with technical system access and user management capabilities", true, "IT", "IT", null },
                    { "59aa3a43-9a6d-459b-9f89-870617bba693", "0dc411f9-b029-4d75-bc44-c67890a969a7", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9733), "HR personnel with employee management and company-wide HR operations access", true, "HR", "HR", null },
                    { "85709f65-a95a-4208-9638-f17a71c5c373", "230c1489-dfa3-43eb-a17d-7d36f8112d28", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9715), "Manager with limited administrative access", true, "Manager", "MANAGER", null },
                    { "8d17de3f-c0b9-4553-902e-e413a4524293", "4338b148-f6ee-4891-8177-acc87f6fee7b", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9741), "Senior HR personnel with advanced HR management capabilities and strategic oversight", true, "HR Manager", "HR MANAGER", null },
                    { "9580912e-a57f-4661-8d1e-2782ea83e6e5", "0d0452b1-6be3-4b00-80e5-5b39777b83ac", new DateTime(2025, 10, 4, 3, 3, 28, 249, DateTimeKind.Utc).AddTicks(9721), "Regular employee with basic access", true, "Employee", "EMPLOYEE", null }
                });
        }
    }
}
