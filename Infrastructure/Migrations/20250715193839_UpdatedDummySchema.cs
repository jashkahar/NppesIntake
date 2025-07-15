using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDummySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_Npi",
                table: "Members");

            migrationBuilder.AlterColumn<long>(
                name: "Npi",
                table: "Members",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Npi",
                table: "Members",
                column: "Npi",
                unique: true,
                filter: "[Npi] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_Npi",
                table: "Members");

            migrationBuilder.AlterColumn<long>(
                name: "Npi",
                table: "Members",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_Npi",
                table: "Members",
                column: "Npi",
                unique: true);
        }
    }
}
