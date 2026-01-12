using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modique.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordSaltToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductOptions_ProductOptionId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "ProductOptionId",
                table: "CartItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_ProductOptionId",
                table: "CartItems",
                newName: "IX_CartItems_ProductId");

            migrationBuilder.AddColumn<string>(
                name: "PasswordSalt",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "CartItems",
                newName: "ProductOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                newName: "IX_CartItems_ProductOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductOptions_ProductOptionId",
                table: "CartItems",
                column: "ProductOptionId",
                principalTable: "ProductOptions",
                principalColumn: "ProductOptionId");
        }
    }
}
