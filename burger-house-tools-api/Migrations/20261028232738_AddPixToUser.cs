using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace burger_house_tools_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPixToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pix",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pix",
                table: "Users");
        }
    }
}
