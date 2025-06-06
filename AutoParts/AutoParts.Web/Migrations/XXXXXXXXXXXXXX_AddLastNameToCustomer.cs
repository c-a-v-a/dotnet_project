using Microsoft.EntityFrameworkCore.Migrations;

public partial class AddLastNameToCustomer : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "LastName",
            table: "Customers",
            maxLength: 50,
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "LastName",
            table: "Customers");
    }
}
