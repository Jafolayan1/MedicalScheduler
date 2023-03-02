using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalScheduler.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LatsName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "7408cc4b-4c92-463c-ac7b-4d2c3c10bf50", 0, "2b639529-2b4e-4a94-a7d6-8282873ab59c", "admin@gmail.com", true, " Super", "Admin", false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEMTvXfXS/8qP6UFngIVcWenLXvUzjVKnVPW5NRIIoMVBReTsds1TtdNV/IA+mTHD2g==", "1234567890", false, "0b2d87f2-17e5-43dd-82bf-149e635da6de", false, "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7408cc4b-4c92-463c-ac7b-4d2c3c10bf50");
        }
    }
}
