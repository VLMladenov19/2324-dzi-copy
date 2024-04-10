using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HAR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthsToCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Carts",
                newName: "MonthPrice");

            migrationBuilder.AddColumn<int>(
                name: "RentMonths",
                table: "RentProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RentMonths",
                table: "CartProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentMonths",
                table: "RentProducts");

            migrationBuilder.DropColumn(
                name: "RentMonths",
                table: "CartProducts");

            migrationBuilder.RenameColumn(
                name: "MonthPrice",
                table: "Carts",
                newName: "TotalPrice");
        }
    }
}
