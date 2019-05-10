using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SagaDemo.LoyaltyPointsAPI.DataAccess.Migrations
{
    public partial class InitialModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointsConsumedEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    PointChange = table.Column<int>(nullable: false),
                    UtcDateTimeRecorded = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(maxLength: 256, nullable: false),
                    TransactionId = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsConsumedEvents", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "PointsEarnedEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    PointChange = table.Column<int>(nullable: false),
                    UtcDateTimeRecorded = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(maxLength: 256, nullable: false),
                    TransactionId = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsEarnedEvents", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "PointsRefundedEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    PointChange = table.Column<int>(nullable: false),
                    UtcDateTimeRecorded = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(maxLength: 256, nullable: false),
                    TransactionId = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsRefundedEvents", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointsConsumedEvents_TransactionId",
                table: "PointsConsumedEvents",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsConsumedEvents_UserId",
                table: "PointsConsumedEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsEarnedEvents_TransactionId",
                table: "PointsEarnedEvents",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsEarnedEvents_UserId",
                table: "PointsEarnedEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsRefundedEvents_TransactionId",
                table: "PointsRefundedEvents",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PointsRefundedEvents_UserId",
                table: "PointsRefundedEvents",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointsConsumedEvents");

            migrationBuilder.DropTable(
                name: "PointsEarnedEvents");

            migrationBuilder.DropTable(
                name: "PointsRefundedEvents");
        }
    }
}
