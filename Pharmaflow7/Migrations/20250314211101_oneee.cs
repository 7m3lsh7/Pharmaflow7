using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmaflow7.Migrations
{
    /// <inheritdoc />
    public partial class oneee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dashboardViewModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegisteredMedicines = table.Column<int>(type: "int", nullable: false),
                    ActiveShipments = table.Column<int>(type: "int", nullable: false),
                    LowStockMedicines = table.Column<int>(type: "int", nullable: false),
                    DailyShipmentRate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dashboardViewModels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dashboardViewModels");
        }
    }
}
