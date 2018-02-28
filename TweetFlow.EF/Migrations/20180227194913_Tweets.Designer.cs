﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using TweetFlow.EF;

namespace TweetFlow.EF.Migrations
{
    [DbContext(typeof(TweetFlowContext))]
    [Migration("20180227194913_Tweets")]
    partial class Tweets
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TweetFlow.DatabaseModel.TWAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.HasKey("Id");

                    b.ToTable("TWAccount");
                });

            modelBuilder.Entity("TweetFlow.DatabaseModel.TWStreamInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EventType");

                    b.Property<string>("ExceptionMessage");

                    b.Property<DateTime>("OccuredAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Reason");

                    b.HasKey("Id");

                    b.ToTable("TWStreamInfo");
                });

            modelBuilder.Entity("TweetFlow.DatabaseModel.TWTweet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CelebrityHighlighted");

                    b.Property<bool>("ConvertedToOriginal");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("datetime2");

                    b.Property<int>("FavoriteCount");

                    b.Property<bool>("Favorited");

                    b.Property<string>("FullText");

                    b.Property<bool>("IsRetweet");

                    b.Property<int>("QuoteCount");

                    b.Property<int>("ReplyCount");

                    b.Property<int>("RetweetCount");

                    b.Property<bool>("RetweetHighlighted");

                    b.Property<string>("StrId");

                    b.Property<string>("Type");

                    b.Property<DateTime>("UserCreatedAt")
                        .HasColumnName("datetime2");

                    b.Property<int>("UserFavouritesCount");

                    b.Property<int>("UserFollowersCount");

                    b.Property<int>("UserFriendsCount");

                    b.Property<long>("UserId");

                    b.Property<int>("UserListedCount");

                    b.Property<string>("UserName");

                    b.Property<string>("UserProfileImageUrl400x400");

                    b.Property<string>("UserScreenName");

                    b.Property<int>("UserStatusesCount");

                    b.Property<string>("UserStrId");

                    b.Property<bool>("UserVerified");

                    b.HasKey("Id");

                    b.ToTable("TWTWeet");
                });

            modelBuilder.Entity("TweetFlow.DatabaseModel.TWUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("HashtagBannCount");

                    b.Property<long>("TwitterId");

                    b.Property<int>("WordBannCount");

                    b.HasKey("Id");

                    b.ToTable("TWUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
