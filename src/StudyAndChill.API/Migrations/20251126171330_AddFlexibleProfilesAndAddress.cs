using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudyAndChill.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFlexibleProfilesAndAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "StudentProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Complement",
                table: "StudentProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                table: "StudentProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Neighborhood",
                table: "StudentProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Number",
                table: "StudentProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                table: "StudentProfiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_State",
                table: "StudentProfiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "StudentProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AsaasCustomerId",
                table: "StudentProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentNumber",
                table: "StudentProfiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "StudentProfiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TeacherProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentType = table.Column<int>(type: "integer", nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Address_Country = table.Column<string>(type: "text", nullable: false),
                    Address_PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Address_Street = table.Column<string>(type: "text", nullable: false),
                    Address_Number = table.Column<string>(type: "text", nullable: false),
                    Address_Complement = table.Column<string>(type: "text", nullable: true),
                    Address_Neighborhood = table.Column<string>(type: "text", nullable: false),
                    Address_City = table.Column<string>(type: "text", nullable: false),
                    Address_State = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BankCode = table.Column<string>(type: "text", nullable: true),
                    Agency = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    PixKey = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherProfiles_UserId",
                table: "TeacherProfiles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Address_Complement",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Address_Neighborhood",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Address_Number",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Address_State",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "AsaasCustomerId",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "DocumentNumber",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "StudentProfiles");
        }
    }
}
