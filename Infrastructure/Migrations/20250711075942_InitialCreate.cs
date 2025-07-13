using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessUnits",
                columns: table => new
                {
                    BusinessUnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Npi = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnits", x => x.BusinessUnitId);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Npi = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Credential = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MemberType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberId);
                });

            migrationBuilder.CreateTable(
                name: "BusinessUnitMembers",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    BusinessUnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnitMembers", x => new { x.MemberId, x.BusinessUnitId });
                    table.ForeignKey(
                        name: "FK_BusinessUnitMembers_BusinessUnits_BusinessUnitId",
                        column: x => x.BusinessUnitId,
                        principalTable: "BusinessUnits",
                        principalColumn: "BusinessUnitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessUnitMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberContactDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    AddressPurpose = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberContactDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberContactDetails_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnitMembers_BusinessUnitId",
                table: "BusinessUnitMembers",
                column: "BusinessUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberContactDetails_MemberId",
                table: "MemberContactDetails",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Npi",
                table: "Members",
                column: "Npi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessUnitMembers");

            migrationBuilder.DropTable(
                name: "MemberContactDetails");

            migrationBuilder.DropTable(
                name: "BusinessUnits");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
