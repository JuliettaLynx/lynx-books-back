using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lynxbooksbackv2.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubscriptionCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_SubscriberUserId",
                table: "Subscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_SubscriberUserId",
                table: "Subscriptions",
                column: "SubscriberUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_SubscriberUserId",
                table: "Subscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_SubscriberUserId",
                table: "Subscriptions",
                column: "SubscriberUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
