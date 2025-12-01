using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyAndChill.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMakeUpExpiryDateToClassSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MakeUpExpiryDate",
                table: "ClassSessions",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MakeUpExpiryDate",
                table: "ClassSessions");
        }
    }
}
