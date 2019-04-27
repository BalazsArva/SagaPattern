using Microsoft.EntityFrameworkCore.Migrations;

namespace SagaDemo.LoyaltyPointsAPI.DataAccess.Migrations
{
    public partial class UserIdAndTransactionIdIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "PointsChangedEvents",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PointsChangedEvents_TransactionId",
                table: "PointsChangedEvents",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsChangedEvents_UserId",
                table: "PointsChangedEvents",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PointsChangedEvents_TransactionId",
                table: "PointsChangedEvents");

            migrationBuilder.DropIndex(
                name: "IX_PointsChangedEvents_UserId",
                table: "PointsChangedEvents");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "PointsChangedEvents");
        }
    }
}
