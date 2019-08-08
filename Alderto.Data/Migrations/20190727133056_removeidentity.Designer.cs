﻿// <auto-generated />
using System;
using Alderto.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Alderto.Data.Migrations
{
    [DbContext(typeof(AldertoDbContext))]
    [Migration("20190727133056_removeidentity")]
    partial class removeidentity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Alderto.Data.Models.CustomCommand", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("TriggerKeyword")
                        .HasMaxLength(20);

                    b.Property<string>("LuaCode")
                        .HasMaxLength(2000);

                    b.HasKey("GuildId", "TriggerKeyword");

                    b.ToTable("CustomCommands");
                });

            modelBuilder.Entity("Alderto.Data.Models.Guild", b =>
                {
                    b.Property<decimal>("Id")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<DateTimeOffset?>("PremiumUntil");

                    b.HasKey("Id");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("Alderto.Data.Models.GuildConfiguration", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal>("AcceptedMemberRoleId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("CurrencySymbol")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("TimelyCooldown");

                    b.Property<int>("TimelyRewardQuantity");

                    b.HasKey("GuildId");

                    b.ToTable("GuildPreferences");
                });

            modelBuilder.Entity("Alderto.Data.Models.GuildMember", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal>("MemberId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<int>("CurrencyCount");

                    b.Property<DateTimeOffset>("CurrencyLastClaimed");

                    b.Property<DateTimeOffset?>("JoinedAt");

                    b.Property<string>("Nickname")
                        .HasMaxLength(32);

                    b.Property<decimal?>("RecruiterMemberId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.HasKey("GuildId", "MemberId");

                    b.HasIndex("MemberId");

                    b.ToTable("GuildMembers");
                });

            modelBuilder.Entity("Alderto.Data.Models.GuildMemberDonation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Donation")
                        .HasMaxLength(100);

                    b.Property<DateTimeOffset>("DonationDate");

                    b.Property<decimal>("GuildId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal>("MemberId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.HasKey("Id");

                    b.HasIndex("GuildId", "MemberId");

                    b.ToTable("GuildMemberDonations");
                });

            modelBuilder.Entity("Alderto.Data.Models.Member", b =>
                {
                    b.Property<decimal>("Id")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Discriminator")
                        .HasMaxLength(4);

                    b.Property<string>("Username")
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("Alderto.Data.Models.CustomCommand", b =>
                {
                    b.HasOne("Alderto.Data.Models.Guild", "Guild")
                        .WithMany("CustomCommands")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Alderto.Data.Models.GuildConfiguration", b =>
                {
                    b.HasOne("Alderto.Data.Models.Guild", "Guild")
                        .WithOne("Configuration")
                        .HasForeignKey("Alderto.Data.Models.GuildConfiguration", "GuildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Alderto.Data.Models.GuildMember", b =>
                {
                    b.HasOne("Alderto.Data.Models.Guild", "Guild")
                        .WithMany("GuildMembers")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Alderto.Data.Models.Member", "Member")
                        .WithMany("GuildMembers")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Alderto.Data.Models.GuildMemberDonation", b =>
                {
                    b.HasOne("Alderto.Data.Models.GuildMember", "GuildMember")
                        .WithMany("Donations")
                        .HasForeignKey("GuildId", "MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}