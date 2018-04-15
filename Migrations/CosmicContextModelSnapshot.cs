﻿// <auto-generated />
using System;
using CosmicBox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CosmicBox.Migrations
{
    [DbContext(typeof(CosmicContext))]
    partial class CosmicContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.0-preview2-30571");

            modelBuilder.Entity("CosmicBox.Models.Box", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.HasKey("Id");

                    b.ToTable("boxes");
                });

            modelBuilder.Entity("CosmicBox.Models.Grant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("BoxId")
                        .HasColumnName("box_id");

                    b.Property<string>("Sub")
                        .IsRequired()
                        .HasColumnName("sub");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnName("type");

                    b.HasKey("Id");

                    b.HasIndex("BoxId");

                    b.ToTable("grants");
                });

            modelBuilder.Entity("CosmicBox.Models.Run", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("BoxId")
                        .HasColumnName("box_id");

                    b.Property<DateTime>("End")
                        .HasColumnName("end");

                    b.Property<DateTime>("Start")
                        .HasColumnName("start");

                    b.HasKey("Id");

                    b.HasIndex("BoxId");

                    b.ToTable("runs");
                });

            modelBuilder.Entity("CosmicBox.Models.Trace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<float>("Pressure")
                        .HasColumnName("pressure");

                    b.Property<int>("RunId")
                        .HasColumnName("run_id");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnName("timestamp");

                    b.HasKey("Id");

                    b.HasIndex("RunId");

                    b.ToTable("traces");
                });

            modelBuilder.Entity("CosmicBox.Models.Grant", b =>
                {
                    b.HasOne("CosmicBox.Models.Box", "Box")
                        .WithMany("Grants")
                        .HasForeignKey("BoxId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CosmicBox.Models.Run", b =>
                {
                    b.HasOne("CosmicBox.Models.Box", "Box")
                        .WithMany("Runs")
                        .HasForeignKey("BoxId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CosmicBox.Models.Trace", b =>
                {
                    b.HasOne("CosmicBox.Models.Run", "Run")
                        .WithMany("Traces")
                        .HasForeignKey("RunId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
