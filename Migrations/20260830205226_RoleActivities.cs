using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee_Self_Service.Migrations
{
    /// <inheritdoc />
    public partial class RoleActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roleActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Screen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    canView = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    canAdd = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    canEdit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    canDelete = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roleActivities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roleActivities");
        }
    }
}
