using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaBridge.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MedcinAndImageValidateCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_PrescriptionRequest_Content",
                table: "PrescriptionRequests",
                sql: "[ImageUrl] IS NOT NULL OR [MedicineName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_PrescriptionRequest_Content",
                table: "PrescriptionRequests");
        }
    }
}
