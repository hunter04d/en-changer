using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace EnChanger.Migrations
{
    public partial class AddedValidUntilTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    NumberOfAccesses = table.Column<long>(nullable: true),
                    ValidUntil = table.Column<Instant>(nullable: true),
                    CreatedAt = table.Column<Instant>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");
        }
    }
}
