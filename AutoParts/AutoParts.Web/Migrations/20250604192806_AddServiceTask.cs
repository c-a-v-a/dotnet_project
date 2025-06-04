using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoParts.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceTaskId",
                table: "UsedParts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ServiceTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaborCost = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTasks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsedParts_ServiceTaskId",
                table: "UsedParts",
                column: "ServiceTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedParts_ServiceTasks_ServiceTaskId",
                table: "UsedParts",
                column: "ServiceTaskId",
                principalTable: "ServiceTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedParts_ServiceTasks_ServiceTaskId",
                table: "UsedParts");

            migrationBuilder.DropTable(
                name: "ServiceTasks");

            migrationBuilder.DropIndex(
                name: "IX_UsedParts_ServiceTaskId",
                table: "UsedParts");

            migrationBuilder.DropColumn(
                name: "ServiceTaskId",
                table: "UsedParts");
        }
    }
}
