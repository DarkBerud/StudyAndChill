using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudyAndChill.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClassSessionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassSessionId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "classSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SatartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_classSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_classSessions_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClassSessionId",
                table: "Users",
                column: "ClassSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_classSessions_TeacherId",
                table: "classSessions",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_classSessions_ClassSessionId",
                table: "Users",
                column: "ClassSessionId",
                principalTable: "classSessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_classSessions_ClassSessionId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "classSessions");

            migrationBuilder.DropIndex(
                name: "IX_Users_ClassSessionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClassSessionId",
                table: "Users");
        }
    }
}
