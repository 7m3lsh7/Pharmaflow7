using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmaflow7.Migrations
{
    /// <inheritdoc />
    public partial class sfss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_AspNetUsers_DistributorId1",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_DistributorId1",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DistributorId1",
                table: "Shipments");

            migrationBuilder.AlterColumn<string>(
                name: "DistributorId",
                table: "Shipments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DistributorId",
                table: "Shipments",
                column: "DistributorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_AspNetUsers_DistributorId",
                table: "Shipments",
                column: "DistributorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_AspNetUsers_DistributorId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_DistributorId",
                table: "Shipments");

            migrationBuilder.AlterColumn<int>(
                name: "DistributorId",
                table: "Shipments",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Shipments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DistributorId1",
                table: "Shipments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DistributorId1",
                table: "Shipments",
                column: "DistributorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_AspNetUsers_DistributorId1",
                table: "Shipments",
                column: "DistributorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
