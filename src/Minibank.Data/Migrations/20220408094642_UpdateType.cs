using Microsoft.EntityFrameworkCore.Migrations;

namespace Minibank.Data.Migrations
{
    public partial class UpdateType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bank_account_user_user_id",
                table: "bank_account");

            migrationBuilder.DropForeignKey(
                name: "FK_money_transfer_bank_account_from_bank_account_id",
                table: "money_transfer");

            migrationBuilder.DropForeignKey(
                name: "FK_money_transfer_bank_account_to_bank_account_id",
                table: "money_transfer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_money_transfer",
                table: "money_transfer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bank_account",
                table: "bank_account");

            migrationBuilder.RenameIndex(
                name: "IX_money_transfer_to_bank_account_id",
                table: "money_transfer",
                newName: "ix_money_transfer_to_bank_account_id");

            migrationBuilder.RenameIndex(
                name: "IX_money_transfer_from_bank_account_id",
                table: "money_transfer",
                newName: "ix_money_transfer_from_bank_account_id");

            migrationBuilder.RenameIndex(
                name: "IX_bank_account_user_id",
                table: "bank_account",
                newName: "ix_bank_account_user_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "money_transfer",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<decimal>(
                name: "balance",
                table: "bank_account",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 2,
                oldDefaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "pk_user",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_money_transfer",
                table: "money_transfer",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bank_account",
                table: "bank_account",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bank_account_user_owner_user_id",
                table: "bank_account",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_money_transfer_bank_accounts_from_bank_account_id",
                table: "money_transfer",
                column: "from_bank_account_id",
                principalTable: "bank_account",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_money_transfer_bank_accounts_to_bank_account_id",
                table: "money_transfer",
                column: "to_bank_account_id",
                principalTable: "bank_account",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bank_account_user_owner_user_id",
                table: "bank_account");

            migrationBuilder.DropForeignKey(
                name: "fk_money_transfer_bank_accounts_from_bank_account_id",
                table: "money_transfer");

            migrationBuilder.DropForeignKey(
                name: "fk_money_transfer_bank_accounts_to_bank_account_id",
                table: "money_transfer");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_money_transfer",
                table: "money_transfer");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bank_account",
                table: "bank_account");

            migrationBuilder.RenameIndex(
                name: "ix_money_transfer_to_bank_account_id",
                table: "money_transfer",
                newName: "IX_money_transfer_to_bank_account_id");

            migrationBuilder.RenameIndex(
                name: "ix_money_transfer_from_bank_account_id",
                table: "money_transfer",
                newName: "IX_money_transfer_from_bank_account_id");

            migrationBuilder.RenameIndex(
                name: "ix_bank_account_user_id",
                table: "bank_account",
                newName: "IX_bank_account_user_id");

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "money_transfer",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<double>(
                name: "balance",
                table: "bank_account",
                type: "double precision",
                precision: 2,
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldDefaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_money_transfer",
                table: "money_transfer",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_bank_account",
                table: "bank_account",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_bank_account_user_user_id",
                table: "bank_account",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_money_transfer_bank_account_from_bank_account_id",
                table: "money_transfer",
                column: "from_bank_account_id",
                principalTable: "bank_account",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_money_transfer_bank_account_to_bank_account_id",
                table: "money_transfer",
                column: "to_bank_account_id",
                principalTable: "bank_account",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
