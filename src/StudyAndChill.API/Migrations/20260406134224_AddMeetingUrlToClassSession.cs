using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyAndChill.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingUrlToClassSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MeetingURL",
                table: "ClassSessions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingURL",
                table: "ClassSessions");
        }
    }
}
