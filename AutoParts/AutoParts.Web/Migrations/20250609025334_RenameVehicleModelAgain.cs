using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoParts.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenameVehicleModelAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Model",
                table: "Vehicles",
                newName: "ModelName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModelName",
                table: "Vehicles",
                newName: "Model");
        }
    }
}
