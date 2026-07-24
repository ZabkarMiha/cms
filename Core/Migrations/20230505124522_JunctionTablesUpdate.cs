using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class JunctionTablesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCars_Profile_UserId",
                table: "UserCars");

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_UserId",
                table: "UserCars",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HandlersUsers_UserId",
                table: "HandlersUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HandlersCars_CarId",
                table: "HandlersCars",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCars_Profile_UserId",
                table: "UserCars",
                column: "UserId",
                principalTable: "Profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCars_Profile_UserId",
                table: "UserCars");

            migrationBuilder.DropIndex(
                name: "IX_UserCars_UserId",
                table: "UserCars");

            migrationBuilder.DropIndex(
                name: "IX_HandlersUsers_UserId",
                table: "HandlersUsers");

            migrationBuilder.DropIndex(
                name: "IX_HandlersCars_CarId",
                table: "HandlersCars");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCars_Profile_UserId",
                table: "UserCars",
                column: "UserId",
                principalTable: "Profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
