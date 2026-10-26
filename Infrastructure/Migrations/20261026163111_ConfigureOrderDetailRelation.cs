using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureOrderDetailRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_orderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_productId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_orderId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_productId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "orderId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "productId",
                table: "OrderDetails");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_Order_id",
                table: "OrderDetails",
                column: "Order_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_Product_id",
                table: "OrderDetails",
                column: "Product_id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_Order_id",
                table: "OrderDetails",
                column: "Order_id",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_Product_id",
                table: "OrderDetails",
                column: "Product_id",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_Order_id",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_Product_id",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_Order_id",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_Product_id",
                table: "OrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "orderId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "productId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_orderId",
                table: "OrderDetails",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_productId",
                table: "OrderDetails",
                column: "productId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_orderId",
                table: "OrderDetails",
                column: "orderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_productId",
                table: "OrderDetails",
                column: "productId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
