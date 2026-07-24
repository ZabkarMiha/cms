using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class CarNavigationsAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_CarBody_BodyType",
                table: "Car");

            migrationBuilder.DropForeignKey(
                name: "FK_Car_CarBrand_Brand",
                table: "Car");

            migrationBuilder.DropForeignKey(
                name: "FK_Car_CarEngine_EngineType",
                table: "Car");

            migrationBuilder.RenameColumn(
                name: "EngineType",
                table: "Car",
                newName: "EngineTypeId");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "Car",
                newName: "BrandId");

            migrationBuilder.RenameColumn(
                name: "BodyType",
                table: "Car",
                newName: "BodyTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Car_EngineType",
                table: "Car",
                newName: "IX_Car_EngineTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Car_Brand",
                table: "Car",
                newName: "IX_Car_BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_Car_BodyType",
                table: "Car",
                newName: "IX_Car_BodyTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Car_CarBody_BodyTypeId",
                table: "Car",
                column: "BodyTypeId",
                principalTable: "CarBody",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Car_CarBrand_BrandId",
                table: "Car",
                column: "BrandId",
                principalTable: "CarBrand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Car_CarEngine_EngineTypeId",
                table: "Car",
                column: "EngineTypeId",
                principalTable: "CarEngine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_CarBody_BodyTypeId",
                table: "Car");

            migrationBuilder.DropForeignKey(
                name: "FK_Car_CarBrand_BrandId",
                table: "Car");

            migrationBuilder.DropForeignKey(
                name: "FK_Car_CarEngine_EngineTypeId",
                table: "Car");

            migrationBuilder.RenameColumn(
                name: "EngineTypeId",
                table: "Car",
                newName: "EngineType");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "Car",
                newName: "Brand");

            migrationBuilder.RenameColumn(
                name: "BodyTypeId",
                table: "Car",
                newName: "BodyType");

            migrationBuilder.RenameIndex(
                name: "IX_Car_EngineTypeId",
                table: "Car",
                newName: "IX_Car_EngineType");

            migrationBuilder.RenameIndex(
                name: "IX_Car_BrandId",
                table: "Car",
                newName: "IX_Car_Brand");

            migrationBuilder.RenameIndex(
                name: "IX_Car_BodyTypeId",
                table: "Car",
                newName: "IX_Car_BodyType");

            migrationBuilder.AddForeignKey(
                name: "FK_Car_CarBody_BodyType",
                table: "Car",
                column: "BodyType",
                principalTable: "CarBody",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Car_CarBrand_Brand",
                table: "Car",
                column: "Brand",
                principalTable: "CarBrand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Car_CarEngine_EngineType",
                table: "Car",
                column: "EngineType",
                principalTable: "CarEngine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
