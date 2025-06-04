using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoParts.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "UsedParts");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "ServiceTasks");

            migrationBuilder.AddColumn<int>(
                name: "ServiceOrderId",
                table: "ServiceTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceOrderId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ServiceOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MechanicId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Users_MechanicId",
                        column: x => x.MechanicId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_ServiceOrderId",
                table: "ServiceTasks",
                column: "ServiceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ServiceOrderId",
                table: "Comments",
                column: "ServiceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_MechanicId",
                table: "ServiceOrders",
                column: "MechanicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_ServiceOrders_ServiceOrderId",
                table: "Comments",
                column: "ServiceOrderId",
                principalTable: "ServiceOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTasks_ServiceOrders_ServiceOrderId",
                table: "ServiceTasks",
                column: "ServiceOrderId",
                principalTable: "ServiceOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_ServiceOrders_ServiceOrderId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTasks_ServiceOrders_ServiceOrderId",
                table: "ServiceTasks");

            migrationBuilder.DropTable(
                name: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTasks_ServiceOrderId",
                table: "ServiceTasks");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ServiceOrderId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ServiceOrderId",
                table: "ServiceTasks");

            migrationBuilder.DropColumn(
                name: "ServiceOrderId",
                table: "Comments");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "UsedParts",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "ServiceTasks",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
