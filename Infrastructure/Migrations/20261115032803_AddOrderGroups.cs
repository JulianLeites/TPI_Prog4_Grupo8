using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_Order_id",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "Order_id",
                table: "OrderDetails",
                newName: "OrderGroup_id");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_Order_id",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderGroup_id");

            migrationBuilder.CreateTable(
                name: "OrderGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderGroups_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderGroups_OrderId",
                table: "OrderGroups",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_OrderGroups_OrderGroup_id",
                table: "OrderDetails",
                column: "OrderGroup_id",
                principalTable: "OrderGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_OrderGroups_OrderGroup_id",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderGroups");

            migrationBuilder.RenameColumn(
                name: "OrderGroup_id",
                table: "OrderDetails",
                newName: "Order_id");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderGroup_id",
                table: "OrderDetails",
                newName: "IX_OrderDetails_Order_id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_Order_id",
                table: "OrderDetails",
                column: "Order_id",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
