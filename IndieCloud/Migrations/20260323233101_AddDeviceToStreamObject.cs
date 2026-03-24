using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndieCloud.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceToStreamObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Device",
                table: "StreamObjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Device",
                table: "StreamObjects");
        }
    }
}
