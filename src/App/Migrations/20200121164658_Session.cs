using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace EnChanger.Migrations
{
    public partial class Session : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "Entries",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExpiryTime = table.Column<Instant>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entries_SessionId",
                table: "Entries",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_Session_SessionId",
                table: "Entries",
                column: "SessionId",
                principalTable: "Session",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_Session_SessionId",
                table: "Entries");

            migrationBuilder.DropTable(
                name: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Entries_SessionId",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Entries");
        }
    }
}
