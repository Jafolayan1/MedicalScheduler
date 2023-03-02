using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalScheduler.Migrations
{
    public partial class updateAppointmentTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "31d52c20-3a45-4e0f-9a41-0e32fbffcef3");

            migrationBuilder.RenameColumn(
                name: "Ailment",
                table: "Appointments",
                newName: "Matric");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LatsName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "680ee2ed-f94c-4c7f-970c-2ac5b7fc3a84", 0, "c1896514-876a-4c68-b48c-a9cd60a9f2fe", "admin@gmail.com", true, " Super", "Admin", false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEKIDefZ+STdneHN4OxXAAaHlxmG1yxqS+5NygWcH/4m3ZS3SMpzsIamd7obUa82rrw==", "1234567890", false, "e00fb6f8-3cc3-45b7-8365-ebb384e64293", false, "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "680ee2ed-f94c-4c7f-970c-2ac5b7fc3a84");

            migrationBuilder.RenameColumn(
                name: "Matric",
                table: "Appointments",
                newName: "Ailment");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LatsName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "31d52c20-3a45-4e0f-9a41-0e32fbffcef3", 0, "80627af9-97b6-46a1-b7f9-3e3186e81986", "admin@gmail.com", true, " Super", "Admin", false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEP0YsoKTPV/7j7DAnpWrnIAULRf5Vkzc9OrQ7GBgRBLAIT4oX5mWqicuI2Fh2lTBpg==", "1234567890", false, "86b68c7c-0330-458a-997e-87cc562a6524", false, "Admin" });
        }
    }
}
