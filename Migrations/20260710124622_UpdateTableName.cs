using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee_Self_Service.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "emp_info");

            migrationBuilder.AddPrimaryKey(
                name: "PK_emp_info",
                table: "emp_info",
                column: "emp_no");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_emp_info",
                table: "emp_info");

            migrationBuilder.RenameTable(
                name: "emp_info",
                newName: "Products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "emp_no");
        }
    }
}
