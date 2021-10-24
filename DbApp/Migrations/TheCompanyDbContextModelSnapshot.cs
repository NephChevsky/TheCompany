﻿// <auto-generated />
using System;
using DbApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DbApp.Migrations
{
    [DbContext(typeof(TheCompanyDbContext))]
    partial class TheCompanyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DbApp.Models.Individual", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Deleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MobilePhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Owner")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Customers_Individual");
                });

            modelBuilder.Entity("DbApp.Models.Invoice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Deleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("ExtractDateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ExtractId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("InvoiceNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsExtracted")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("LastModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("LockedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Owner")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("ShouldBeExtracted")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.HasKey("Id");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("DbApp.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(512)")
                        .HasMaxLength(512);

                    b.Property<DateTime>("LastLoginDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(512)")
                        .HasMaxLength(512);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(512)")
                        .HasMaxLength(512);

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DbApp.Models.Individual", b =>
                {
                    b.OwnsOne("DbApp.Models.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("IndividualId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("City")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Country")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Number")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Street")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ZipCode")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("IndividualId");

                            b1.ToTable("Customers_Individual");

                            b1.WithOwner()
                                .HasForeignKey("IndividualId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
