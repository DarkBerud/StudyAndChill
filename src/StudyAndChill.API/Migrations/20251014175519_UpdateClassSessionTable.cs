using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyAndChill.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassSessionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_classSessions_Users_TeacherId",
                table: "classSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_classSessions_ClassSessionId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_classSessions",
                table: "classSessions");

            migrationBuilder.RenameTable(
                name: "classSessions",
                newName: "ClassSessions");

            migrationBuilder.RenameIndex(
                name: "IX_classSessions_TeacherId",
                table: "ClassSessions",
                newName: "IX_ClassSessions_TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassSessions",
                table: "ClassSessions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSessions_Users_TeacherId",
                table: "ClassSessions",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ClassSessions_ClassSessionId",
                table: "Users",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSessions_Users_TeacherId",
                table: "ClassSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_ClassSessions_ClassSessionId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassSessions",
                table: "ClassSessions");

            migrationBuilder.RenameTable(
                name: "ClassSessions",
                newName: "classSessions");

            migrationBuilder.RenameIndex(
                name: "IX_ClassSessions_TeacherId",
                table: "classSessions",
                newName: "IX_classSessions_TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_classSessions",
                table: "classSessions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_classSessions_Users_TeacherId",
                table: "classSessions",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_classSessions_ClassSessionId",
                table: "Users",
                column: "ClassSessionId",
                principalTable: "classSessions",
                principalColumn: "Id");
        }
    }
}
