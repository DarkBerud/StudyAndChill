using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudyAndChill.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCompleteFinancialStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreferredPaymentDay",
                table: "TeacherProfiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AsaasSubscriptionId",
                table: "Contracts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DueDay",
                table: "Contracts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyAmount",
                table: "Contracts",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TeacherPaymentShare",
                table: "Contracts",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NetValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AsaasPaymentId = table.Column<string>(type: "text", nullable: true),
                    AsaasInvoiceUrl = table.Column<string>(type: "text", nullable: true),
                    AsaasPixQrCode = table.Column<string>(type: "text", nullable: true),
                    ContractId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherFinancialRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    RelatedContractId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherFinancialRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherFinancialRecords_Contracts_RelatedContractId",
                        column: x => x.RelatedContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeacherFinancialRecords_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ContractId",
                table: "Payments",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherFinancialRecords_RelatedContractId",
                table: "TeacherFinancialRecords",
                column: "RelatedContractId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherFinancialRecords_TeacherId",
                table: "TeacherFinancialRecords",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "TeacherFinancialRecords");

            migrationBuilder.DropColumn(
                name: "PreferredPaymentDay",
                table: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "AsaasSubscriptionId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DueDay",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "MonthlyAmount",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TeacherPaymentShare",
                table: "Contracts");
        }
    }
}
