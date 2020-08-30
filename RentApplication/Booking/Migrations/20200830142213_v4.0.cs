using Microsoft.EntityFrameworkCore.Migrations;

namespace Booking.Migrations
{
    public partial class v40 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasUserCommented",
                table: "UserFeedbacks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasUserRated",
                table: "UserFeedbacks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasUserCommented",
                table: "UserFeedbacks");

            migrationBuilder.DropColumn(
                name: "HasUserRated",
                table: "UserFeedbacks");
        }
    }
}
