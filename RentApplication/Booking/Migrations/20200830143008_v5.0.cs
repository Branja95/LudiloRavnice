using Microsoft.EntityFrameworkCore.Migrations;

namespace Booking.Migrations
{
    public partial class v50 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Value",
                table: "UserFeedbacks",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "UserFeedbacks",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
