using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyAndChill.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPixKeyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PixKeyType",
                table: "TeacherProfiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentType",
                table: "StudentProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PixKeyType",
                table: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "StudentProfiles");
        }
    }
}
