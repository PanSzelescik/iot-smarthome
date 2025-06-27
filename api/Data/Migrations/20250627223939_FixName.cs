using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IotSmartHome.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Automations_UserSwitch_UserSwitchId",
                table: "Automations");

            migrationBuilder.DropIndex(
                name: "IX_Automations_UserSwitchId",
                table: "Automations");

            migrationBuilder.DropColumn(
                name: "UserSwitchId",
                table: "Automations");

            migrationBuilder.CreateIndex(
                name: "IX_Automations_ThenSwitchId",
                table: "Automations",
                column: "ThenSwitchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Automations_UserSwitch_ThenSwitchId",
                table: "Automations",
                column: "ThenSwitchId",
                principalTable: "UserSwitch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Automations_UserSwitch_ThenSwitchId",
                table: "Automations");

            migrationBuilder.DropIndex(
                name: "IX_Automations_ThenSwitchId",
                table: "Automations");

            migrationBuilder.AddColumn<int>(
                name: "UserSwitchId",
                table: "Automations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Automations_UserSwitchId",
                table: "Automations",
                column: "UserSwitchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Automations_UserSwitch_UserSwitchId",
                table: "Automations",
                column: "UserSwitchId",
                principalTable: "UserSwitch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
