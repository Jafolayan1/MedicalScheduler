using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalScheduler.Migrations
{
    public partial class updateTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7408cc4b-4c92-463c-ac7b-4d2c3c10bf50");

            migrationBuilder.RenameColumn(
                name: "Ailment",
                table: "Patients",
                newName: "SchoolFee");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LatsName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "31d52c20-3a45-4e0f-9a41-0e32fbffcef3", 0, "80627af9-97b6-46a1-b7f9-3e3186e81986", "admin@gmail.com", true, " Super", "Admin", false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEP0YsoKTPV/7j7DAnpWrnIAULRf5Vkzc9OrQ7GBgRBLAIT4oX5mWqicuI2Fh2lTBpg==", "1234567890", false, "86b68c7c-0330-458a-997e-87cc562a6524", false, "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "31d52c20-3a45-4e0f-9a41-0e32fbffcef3");

            migrationBuilder.RenameColumn(
                name: "SchoolFee",
                table: "Patients",
                newName: "Ailment");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LatsName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "7408cc4b-4c92-463c-ac7b-4d2c3c10bf50", 0, "2b639529-2b4e-4a94-a7d6-8282873ab59c", "admin@gmail.com", true, " Super", "Admin", false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEMTvXfXS/8qP6UFngIVcWenLXvUzjVKnVPW5NRIIoMVBReTsds1TtdNV/IA+mTHD2g==", "1234567890", false, "0b2d87f2-17e5-43dd-82bf-149e635da6de", false, "Admin" });
        }
    }
}
