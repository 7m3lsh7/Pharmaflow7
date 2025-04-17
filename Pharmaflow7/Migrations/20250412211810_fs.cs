using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmaflow7.Migrations
{
    /// <inheritdoc />
    public partial class fs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Shipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Shipments",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Shipments");
        }
    }
}
