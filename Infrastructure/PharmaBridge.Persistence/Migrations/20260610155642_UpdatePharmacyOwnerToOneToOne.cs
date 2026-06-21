using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaBridge.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePharmacyOwnerToOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pharmacies_PharmaOwnerId",
                table: "Pharmacies");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_PharmaOwnerId",
                table: "Pharmacies",
                column: "PharmaOwnerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pharmacies_PharmaOwnerId",
                table: "Pharmacies");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_PharmaOwnerId",
                table: "Pharmacies",
                column: "PharmaOwnerId");
        }
    }
}
