using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMealApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderPackageCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderPackages_Orders_OrderID",
                table: "OrderPackages");

            // 1. DROP THE INDEX FIRST
            migrationBuilder.DropIndex(
                name: "IX_Users_BusinessId",
                table: "Users");

            // 2. ALTER THE COLUMN
            migrationBuilder.AlterColumn<Guid>(
                name: "BusinessId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // 3. RECREATE THE INDEX
            migrationBuilder.CreateIndex(
                name: "IX_Users_BusinessId",
                table: "Users",
                column: "BusinessId",
                unique: true,
                filter: "[BusinessId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderPackages_Orders_OrderID",
                table: "OrderPackages",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderPackages_Orders_OrderID",
                table: "OrderPackages");

            // 1. DROP THE INDEX FIRST
            migrationBuilder.DropIndex(
                name: "IX_Users_BusinessId",
                table: "Users");

            // 2. REVERT THE COLUMN
            migrationBuilder.AlterColumn<Guid>(
                name: "BusinessId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // 3. RECREATE THE INDEX (Without the IS NOT NULL filter since it's required here)
            migrationBuilder.CreateIndex(
                name: "IX_Users_BusinessId",
                table: "Users",
                column: "BusinessId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderPackages_Orders_OrderID",
                table: "OrderPackages",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}