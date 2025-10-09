using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyAndChill.API.Migrations
{
    /// <inheritdoc />
    public partial class MakeIconUrlNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_userInvitations",
                table: "userInvitations");

            migrationBuilder.RenameTable(
                name: "userInvitations",
                newName: "UserInvitations");

            migrationBuilder.AlterColumn<string>(
                name: "IconUrl",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserInvitations",
                table: "UserInvitations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserInvitations",
                table: "UserInvitations");

            migrationBuilder.RenameTable(
                name: "UserInvitations",
                newName: "userInvitations");

            migrationBuilder.AlterColumn<string>(
                name: "IconUrl",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_userInvitations",
                table: "userInvitations",
                column: "Id");
        }
    }
}
