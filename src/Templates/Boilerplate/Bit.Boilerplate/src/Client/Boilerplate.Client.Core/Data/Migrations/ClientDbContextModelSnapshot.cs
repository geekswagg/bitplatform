﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Boilerplate.Client.Core.Data.Migrations;

[DbContext(typeof(OfflineDbContext))]
partial class OfflineDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

        modelBuilder.Entity("Boilerplate.Shared.Dtos.Identity.UserDto", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<long?>("BirthDate")
                    .HasColumnType("INTEGER");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<string>("FullName")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<int?>("Gender")
                    .HasColumnType("INTEGER");

                b.Property<string>("Password")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<string>("ProfileImageName")
                    .HasColumnType("TEXT");

                b.Property<string>("UserName")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("Users");

                b.HasData(
                    new
                    {
                        Id = 1,
                        Email = "test@bitplatform.dev",
                        FullName = "Boilerplate test account",
                        Password = "123456",
                        UserName = "test@bitplatform.dev"
                    });
            });
#pragma warning restore 612, 618
    }
}