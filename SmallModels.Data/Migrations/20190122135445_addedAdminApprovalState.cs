using Microsoft.EntityFrameworkCore.Migrations;

namespace SmallPosters.Data.Migrations
{
    public partial class addedAdminApprovalState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminApprovalState",
                table: "Ads",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminApprovalState",
                table: "Ads");
        }
    }
}
