﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProvisionData.Specifications.Internal;

namespace ProvisionData.Specifications.Migrations
{
    [DbContext(typeof(EntityFrameworkContext))]
    partial class EntityFrameworkContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ProvisionData.Specifications.Internal.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id")
                        .HasName("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("b7200d4a-ffb2-43b3-b996-ae8bc4a86553"),
                            DateOfBirth = new DateTime(2007, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Gender = 2,
                            Name = "Shyloh"
                        },
                        new
                        {
                            Id = new Guid("3acb3238-196b-487a-91f0-0cf0481fb272"),
                            DateOfBirth = new DateTime(1975, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Gender = 1,
                            Name = "Charmaine"
                        },
                        new
                        {
                            Id = new Guid("ef636071-8288-497e-a890-3cb2fe14dfb2"),
                            DateOfBirth = new DateTime(1974, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Gender = 2,
                            Name = "Doug"
                        },
                        new
                        {
                            Id = new Guid("d08413b2-b072-458f-9e38-29ea89fc37b2"),
                            DateOfBirth = new DateTime(2011, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Gender = 2,
                            Name = "Geordi"
                        },
                        new
                        {
                            Id = new Guid("140e7335-0299-468d-a821-89709a99511f"),
                            DateOfBirth = new DateTime(2008, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Gender = 1,
                            Name = "Piper"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
