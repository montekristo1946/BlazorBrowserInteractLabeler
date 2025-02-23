﻿// <auto-generated />
using BrowserInteractLabeler.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BrowserInteractLabeler.Repository.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230718150026_PositionInGroup")]
    partial class PositionInGroup
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.8");

            modelBuilder.Entity("BrowserInteractLabeler.Web.Common.DTO.Annotation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ImageFrameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LabelId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LabelPattern")
                        .HasColumnType("INTEGER");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ImageFrameId");

                    b.ToTable("Annotations");
                });

            modelBuilder.Entity("BrowserInteractLabeler.Web.Common.DTO.ImageFrame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Images")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("NameImages")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SizeImageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SizeImageId");

                    b.ToTable("ImageFrames");
                });

            modelBuilder.Entity("BrowserInteractLabeler.Web.Common.DTO.Label", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("NameLabel")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Labels");
                });

            modelBuilder.Entity("BrowserInteractLabeler.Web.Common.PointF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AnnotationId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PositionInGroup")
                        .HasColumnType("INTEGER");

                    b.Property<float>("X")
                        .HasColumnType("REAL");

                    b.Property<float>("Y")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("AnnotationId");

                    b.ToTable("Points");
                });

            modelBuilder.Entity("BrowserInteractLabeler.Web.Common.SizeF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("Height")
                        .HasColumnType("REAL");

                    b.Property<float>("Width")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Sizes");
                });

            modelBuilder.Entity("BrowserInteractLabeler.Web.Common.DTO.Annotation", b =>
                {
                    b.HasOne("BrowserInteractLabeler.Web.Common.DTO.ImageFrame", "Images")
                        .WithMany("Annotations")
                        .HasForeignKey("ImageFrameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Images");
                });

            modelBuilder.Entity("BrowserInteractLabeler.Web.Common.DTO.ImageFrame", b =>
                {
                    b.HasOne("BrowserInteractLabeler.Web.Common.SizeF", "SizeImage")
                        .WithMany()
                        .HasForeignKey("SizeImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SizeImage");
                });

            modelBuilder.Entity("BrowserInteractLabeler.Web.Common.PointF", b =>
                {
                    b.HasOne("BrowserInteractLabeler.Web.Common.DTO.Annotation", "Annot")
                        .WithMany("Points")
                        .HasForeignKey("AnnotationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Annot");
                });

            modelBuilder.Entity("BrowserInteractLabeler.Web.Common.DTO.Annotation", b =>
                {
                    b.Navigation("Points");
                });

            modelBuilder.Entity("BrowserInteractLabeler.Web.Common.DTO.ImageFrame", b =>
                {
                    b.Navigation("Annotations");
                });
#pragma warning restore 612, 618
        }
    }
}
