﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmallPosters.Data;

namespace SmallPosters.Data.Migrations
{
    [DbContext(typeof(SmallPostersContext))]
    partial class SmallPostersContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SmallPosters.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<byte[]>("Salt")
                        .IsRequired();

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("SmallPosters.Models.Ad", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdTimeframe");

                    b.Property<int>("AdminApprovalState");

                    b.Property<Guid>("CategoryId");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<Guid>("CreatorId");

                    b.Property<DateTime>("DateOfCreation");

                    b.Property<string>("ImageURL");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CreatorId");

                    b.ToTable("Ads");
                });

            modelBuilder.Entity("SmallPosters.Models.AuthToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<DateTime>("DateOfCreation");

                    b.Property<string>("HashedValue")
                        .IsRequired();

                    b.Property<bool>("IsValid");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("AuthTokens");
                });

            modelBuilder.Entity("SmallPosters.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(18);

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("SmallPosters.Models.Ad", b =>
                {
                    b.HasOne("SmallPosters.Models.Category", "Category")
                        .WithMany("Ads")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmallPosters.Models.Account", "Creator")
                        .WithMany("Ads")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SmallPosters.Models.AuthToken", b =>
                {
                    b.HasOne("SmallPosters.Models.Account", "Account")
                        .WithMany("AuthTokens")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
