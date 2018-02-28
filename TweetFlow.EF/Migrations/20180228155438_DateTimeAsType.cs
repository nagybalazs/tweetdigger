using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TweetFlow.EF.Migrations
{
    public partial class DateTimeAsType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "datetime2",
                table: "TWTWeet",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "UserCreatedAt",
                table: "TWTWeet",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserCreatedAt",
                table: "TWTWeet");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "TWTWeet",
                newName: "datetime2");
        }
    }
}
