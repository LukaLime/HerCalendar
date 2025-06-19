using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerCalendar.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToCycleTracker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CycleTracker",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CycleTracker_UserId",
                table: "CycleTracker",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CycleTracker_AspNetUsers_UserId",
                table: "CycleTracker",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CycleTracker_AspNetUsers_UserId",
                table: "CycleTracker");

            migrationBuilder.DropIndex(
                name: "IX_CycleTracker_UserId",
                table: "CycleTracker");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CycleTracker");
        }
    }
}
