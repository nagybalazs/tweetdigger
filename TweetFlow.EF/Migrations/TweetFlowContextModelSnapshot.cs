﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using TweetFlow.EF;

namespace TweetFlow.EF.Migrations
{
    [DbContext(typeof(TweetFlowContext))]
    partial class TweetFlowContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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