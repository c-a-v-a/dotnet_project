using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoParts.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixServiceOrderSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Customers_CustomerId",
                table: "ServiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Vehicles_VehicleId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_CustomerId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_VehicleId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "ServiceOrders");
        }
    }
}
