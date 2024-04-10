using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HAR.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rents_Payments_PaymentId",
                table: "Rents");

            migrationBuilder.DropIndex(
                name: "IX_Rents_PaymentId",
                table: "Rents");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Rents");

            migrationBuilder.AlterColumn<Guid>(
                name: "RentId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RentId",
                table: "Payments",
                column: "RentId",
                unique: true,
                filter: "[RentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Rents_RentId",
                table: "Payments",
                column: "RentId",
                principalTable: "Rents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Rents_RentId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_RentId",
                table: "Payments");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "Rents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "RentId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rents_PaymentId",
                table: "Rents",
                column: "PaymentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rents_Payments_PaymentId",
                table: "Rents",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
