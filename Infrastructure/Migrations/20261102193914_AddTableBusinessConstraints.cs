using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableBusinessConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tables_Number",
                table: "Tables",
                column: "Number",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Tables_Capacity_Minimum",
                table: "Tables",
                sql: "[Capacity] >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Tables_Number_Minimum",
                table: "Tables",
                sql: "[Number] >= 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tables_Number",
                table: "Tables");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Tables_Capacity_Minimum",
                table: "Tables");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Tables_Number_Minimum",
                table: "Tables");
        }
    }
}
