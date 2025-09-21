using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdToCustomerIdAndAddStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "vehicles",
                newName: "CustomerId");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "vehicles");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "vehicles",
                newName: "UserId");
        }
    }
}
