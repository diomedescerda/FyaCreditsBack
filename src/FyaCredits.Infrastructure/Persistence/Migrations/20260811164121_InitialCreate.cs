using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FyaCredits.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "credits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ClientId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TermMonths = table.Column<int>(type: "integer", nullable: false),
                    CommercialName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    RegisteredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_credits_Amount",
                table: "credits",
                column: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_credits_ClientId",
                table: "credits",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_credits_CommercialName",
                table: "credits",
                column: "CommercialName");

            migrationBuilder.CreateIndex(
                name: "IX_credits_RegisteredAtUtc",
                table: "credits",
                column: "RegisteredAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credits");
        }
    }
}
