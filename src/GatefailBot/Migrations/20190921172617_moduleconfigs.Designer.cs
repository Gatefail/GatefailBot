﻿// <auto-generated />
using System;
using GatefailBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GatefailBot.Migrations
{
    [DbContext(typeof(GatefailContext))]
    [Migration("20190921172617_moduleconfigs")]
    partial class moduleconfigs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("GatefailBot.Database.BatchQueries.BatchQueryItem", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("IdToQuery");

                    b.Property<Guid>("UniqueId");

                    b.HasKey("Id");

                    b.ToTable("BatchQueries");
                });

            modelBuilder.Entity("GatefailBot.Database.Models.CommandChannelRestriction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Command")
                        .IsRequired();

                    b.Property<ulong>("DiscordChannelId");

                    b.Property<string>("NormalizedCommand")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("CommandChannelRestrictions");
                });

            modelBuilder.Entity("GatefailBot.Database.Models.GatefailUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("DiscordId");

                    b.Property<string>("GuildId");

                    b.HasKey("Id");

                    b.HasIndex("DiscordId");

                    b.HasIndex("GuildId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GatefailBot.Database.Models.Guild", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("DiscordId");

                    b.HasKey("Id");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("GatefailBot.Database.Models.ModuleConfiguration", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activated");

                    b.Property<string>("GuildId");

                    b.Property<string>("ModuleName");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("ModuleConfigurations");
                });

            modelBuilder.Entity("GatefailBot.Database.Models.GatefailUser", b =>
                {
                    b.HasOne("GatefailBot.Database.Models.Guild", "Guild")
                        .WithMany("Users")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GatefailBot.Database.Models.ModuleConfiguration", b =>
                {
                    b.HasOne("GatefailBot.Database.Models.Guild")
                        .WithMany("ModuleConfigurations")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}