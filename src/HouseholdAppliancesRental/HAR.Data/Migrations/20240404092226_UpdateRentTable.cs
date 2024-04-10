using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HAR.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rents_AspNetUsers_UserId",
                table: "Rents");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Rents",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Rents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Rents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserFullName",
                table: "Rents",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Rents_AddressId",
                table: "Rents",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rents_Addresses_AddressId",
                table: "Rents",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rents_AspNetUsers_UserId",
                table: "Rents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rents_Addresses_AddressId",
                table: "Rents");

            migrationBuilder.DropForeignKey(
                name: "FK_Rents_AspNetUsers_UserId",
                table: "Rents");

            migrationBuilder.DropIndex(
                name: "IX_Rents_AddressId",
                table: "Rents");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Rents");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Rents");

            migrationBuilder.DropColumn(
                name: "UserFullName",
                table: "Rents");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Rents",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rents_AspNetUsers_UserId",
                table: "Rents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
