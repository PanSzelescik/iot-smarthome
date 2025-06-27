using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IotSmartHome.Data.Migrations
{
    /// <inheritdoc />
    public partial class Automations_And_Switches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Switches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Switches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSwitch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<string>(type: "text", nullable: false),
                    FriendlyName = table.Column<string>(type: "text", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSwitch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSwitch_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Automations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserThermometerId = table.Column<int>(type: "integer", nullable: false),
                    WhenState = table.Column<double>(type: "double precision", nullable: false),
                    WhenCondition = table.Column<int>(type: "integer", nullable: false),
                    ThenSwitchId = table.Column<int>(type: "integer", nullable: false),
                    UserSwitchId = table.Column<int>(type: "integer", nullable: false),
                    ThenState = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Automations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Automations_UserSwitch_UserSwitchId",
                        column: x => x.UserSwitchId,
                        principalTable: "UserSwitch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Automations_UserThermometers_UserThermometerId",
                        column: x => x.UserThermometerId,
                        principalTable: "UserThermometers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Automations_UserSwitchId",
                table: "Automations",
                column: "UserSwitchId");

            migrationBuilder.CreateIndex(
                name: "IX_Automations_UserThermometerId",
                table: "Automations",
                column: "UserThermometerId");

            migrationBuilder.CreateIndex(
                name: "IX_Switches_DeviceId_CreatedDate",
                table: "Switches",
                columns: new[] { "DeviceId", "CreatedDate" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_UserSwitch_UserId_DeviceId",
                table: "UserSwitch",
                columns: new[] { "UserId", "DeviceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Automations");

            migrationBuilder.DropTable(
                name: "Switches");

            migrationBuilder.DropTable(
                name: "UserSwitch");
        }
    }
}
