using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegendaryCruises.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByUserIdToCruises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Cruises",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Cruises");
        }
    }
}
