﻿// <auto-generated />
using System;
using BookShelf.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
#pragma warning disable CS1591
namespace BookShelf.Migrations
{
    [DbContext(typeof(BooksDbContext))]
    [Migration("20190317203523_dateIsNullable")]
    partial class dateIsNullable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BookShelf.Models.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasMaxLength(55);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateRead");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(7000);

                    b.Property<string>("ImagePath");

                    b.Property<int?>("Rating");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(55);

                    b.HasKey("Id");

                    b.ToTable("Books");
                });
#pragma warning restore 612, 618
        }
    }
}