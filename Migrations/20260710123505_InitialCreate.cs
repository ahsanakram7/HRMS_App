using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee_Self_Service.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    emp_no = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    part_full_time_flag = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    religion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    emp_sex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    blood_group = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    marital_status = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    marital_dated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    country_of_birth = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    city_of_birth = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    can_travel_local = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    can_travel_abroad = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    mobile_no = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    official_email_add = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    personal_email_add = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.emp_no);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
