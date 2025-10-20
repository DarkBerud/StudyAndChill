using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyAndChill.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClassSessionWithManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_ClassSessions_ClassSessionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ClassSessionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClassSessionId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "ClassSessionUser",
                columns: table => new
                {
                    ClassSessionsId = table.Column<int>(type: "integer", nullable: false),
                    StudentsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSessionUser", x => new { x.ClassSessionsId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_ClassSessionUser_ClassSessions_ClassSessionsId",
                        column: x => x.ClassSessionsId,
                        principalTable: "ClassSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassSessionUser_Users_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessionUser_StudentsId",
                table: "ClassSessionUser",
                column: "StudentsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassSessionUser");

            migrationBuilder.AddColumn<int>(
                name: "ClassSessionId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClassSessionId",
                table: "Users",
                column: "ClassSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ClassSessions_ClassSessionId",
                table: "Users",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id");
        }
    }
}
