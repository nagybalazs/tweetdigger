using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TweetFlow.EF.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TWAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TWAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TWTWeet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CelebrityHighlighted = table.Column<bool>(nullable: false),
                    ConvertedToOriginal = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    FavoriteCount = table.Column<int>(nullable: false),
                    Favorited = table.Column<bool>(nullable: false),
                    FullText = table.Column<string>(nullable: true),
                    IsRetweet = table.Column<bool>(nullable: false),
                    QuoteCount = table.Column<int>(nullable: false),
                    ReplyCount = table.Column<int>(nullable: false),
                    RetweetCount = table.Column<int>(nullable: false),
                    RetweetHighlighted = table.Column<bool>(nullable: false),
                    StrId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    UserCreatedAt = table.Column<DateTime>(nullable: false),
                    UserFavouritesCount = table.Column<int>(nullable: false),
                    UserFollowersCount = table.Column<int>(nullable: false),
                    UserFriendsCount = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    UserListedCount = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    UserProfileImageUrl400x400 = table.Column<string>(nullable: true),
                    UserScreenName = table.Column<string>(nullable: true),
                    UserStatusesCount = table.Column<int>(nullable: false),
                    UserStrId = table.Column<string>(nullable: true),
                    UserVerified = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TWTWeet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TWUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HashtagBannCount = table.Column<int>(nullable: false),
                    TwitterId = table.Column<long>(nullable: false),
                    UserMentionBannCount = table.Column<int>(nullable: false),
                    WordBannCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TWUsers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TWAccount");

            migrationBuilder.DropTable(
                name: "TWTWeet");

            migrationBuilder.DropTable(
                name: "TWUsers");
        }
    }
}
