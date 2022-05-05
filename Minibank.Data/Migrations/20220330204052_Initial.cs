using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Minibank.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bank_account",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    balance = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    currency = table.Column<string>(type: "text", nullable: false),
                    is_open = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    date_open = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_close = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bank_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_bank_account_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "money_transfer",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    currency = table.Column<string>(type: "text", nullable: false),
                    from_bank_account_id = table.Column<string>(type: "text", nullable: false),
                    to_bank_account_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_money_transfer", x => x.id);
                    table.ForeignKey(
                        name: "FK_money_transfer_bank_account_from_bank_account_id",
                        column: x => x.from_bank_account_id,
                        principalTable: "bank_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_money_transfer_bank_account_to_bank_account_id",
                        column: x => x.to_bank_account_id,
                        principalTable: "bank_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bank_account_user_id",
                table: "bank_account",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_money_transfer_from_bank_account_id",
                table: "money_transfer",
                column: "from_bank_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_money_transfer_to_bank_account_id",
                table: "money_transfer",
                column: "to_bank_account_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "money_transfer");

            migrationBuilder.DropTable(
                name: "bank_account");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
