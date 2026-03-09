using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Medicares.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubscriptionPlanSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SubscriptionPlans");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreLimit",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "StoreLimit",
                table: "SubscriptionPlans");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
