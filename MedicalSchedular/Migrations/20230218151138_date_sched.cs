using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalScheduler.Migrations
{
    public partial class date_sched : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Sched",
                table: "Patients",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date_Sched",
                table: "Patients");
        }
    }
}
