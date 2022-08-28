using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "433b717d-8cae-4589-906b-036058831b76", "d8fec200-96f2-4450-8f6e-0db5da1bf28a", "Administrator", "ADMINISTRATOR" },
                    { "c998efad-b5c4-4ce5-96f0-45be5f10cd5f", "4b7bcf7f-7b54-4144-b9db-e55a0da2a6a5", "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "433b717d-8cae-4589-906b-036058831b76");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c998efad-b5c4-4ce5-96f0-45be5f10cd5f");
        }
    }
}
