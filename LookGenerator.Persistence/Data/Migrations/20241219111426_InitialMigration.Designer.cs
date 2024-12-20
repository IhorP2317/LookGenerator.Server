﻿// <auto-generated />
using System;
using LookGenerator.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LookGenerator.Persistence.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241219111426_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LookGenerator.Domain.Entities.AttributeOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("AttributeTypeId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AttributeTypeId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Name");

                    b.ToTable("AttributeOptions");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.AttributeType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Name");

                    b.ToTable("AttributeTypes");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.Look", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatorId");

                    b.ToTable("Looks");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.LookProductVariation", b =>
                {
                    b.Property<Guid>("LookId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductVariationId")
                        .HasColumnType("uuid");

                    b.HasKey("LookId", "ProductVariationId");

                    b.HasIndex("ProductVariationId");

                    b.ToTable("LookProductVariations");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.MasterSizeIdentifier", b =>
                {
                    b.Property<string>("Identifier")
                        .HasColumnType("text");

                    b.Property<Guid>("SizeOptionId")
                        .HasColumnType("uuid");

                    b.HasKey("Identifier");

                    b.HasIndex("SizeOptionId");

                    b.ToTable("MasterSizeIdentifiers");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Name");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductAttributeOption", b =>
                {
                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AttributeOptionId")
                        .HasColumnType("uuid");

                    b.HasKey("ProductId", "AttributeOptionId");

                    b.HasIndex("AttributeOptionId");

                    b.ToTable("ProductAttributeOptions");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentCategoryId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("SizeCategoryId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Name");

                    b.HasIndex("ParentCategoryId");

                    b.HasIndex("SizeCategoryId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductItemId")
                        .HasColumnType("uuid");

                    b.Property<bool>("ProductOnly")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("ImageUrl");

                    b.HasIndex("ProductItemId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<int>("Colour")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductItems");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductVariation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsInStock")
                        .HasColumnType("boolean");

                    b.Property<string>("MasterSizeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric(18,2)");

                    b.Property<Guid>("ProductItemId")
                        .HasColumnType("uuid");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("MasterSizeId");

                    b.HasIndex("ProductItemId");

                    b.ToTable("ProductVariations");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.SizeCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentCategoryId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Name");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("SizeCategories");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.SizeOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<double>("Cm")
                        .HasColumnType("numeric(5,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<double>("Inch")
                        .HasColumnType("numeric(5,2)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("SizeCategoryId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("SizeCategoryId");

                    b.ToTable("SizeOptions");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.AttributeOption", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.AttributeType", "AttributeType")
                        .WithMany("AttributeOptions")
                        .HasForeignKey("AttributeTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("AttributeType");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.AttributeType", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.Look", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.User", "User")
                        .WithMany("Looks")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");

                    b.Navigation("Creator");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.LookProductVariation", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.Look", "Look")
                        .WithMany("LookProductVariations")
                        .HasForeignKey("LookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LookGenerator.Domain.Entities.ProductVariation", "ProductVariation")
                        .WithMany("LookProductVariations")
                        .HasForeignKey("ProductVariationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Look");

                    b.Navigation("ProductVariation");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.MasterSizeIdentifier", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.SizeOption", "SizeOption")
                        .WithMany("MasterSizeIds")
                        .HasForeignKey("SizeOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SizeOption");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.Product", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.ProductCategory", "ProductCategory")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Creator");

                    b.Navigation("ProductCategory");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductAttributeOption", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.AttributeOption", "AttributeOption")
                        .WithMany("ProductAttributeOptions")
                        .HasForeignKey("AttributeOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LookGenerator.Domain.Entities.Product", "Product")
                        .WithMany("ProductAttributeOptions")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AttributeOption");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductCategory", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("LookGenerator.Domain.Entities.ProductCategory", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LookGenerator.Domain.Entities.SizeCategory", "SizeCategory")
                        .WithMany()
                        .HasForeignKey("SizeCategoryId");

                    b.Navigation("Creator");

                    b.Navigation("ParentCategory");

                    b.Navigation("SizeCategory");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductImage", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("LookGenerator.Domain.Entities.ProductItem", "ProductItem")
                        .WithMany("Images")
                        .HasForeignKey("ProductItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("ProductItem");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductItem", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("LookGenerator.Domain.Entities.Product", "Product")
                        .WithMany("Items")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductVariation", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("LookGenerator.Domain.Entities.MasterSizeIdentifier", "MasterSizeIdentifier")
                        .WithMany("ProductVariations")
                        .HasForeignKey("MasterSizeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LookGenerator.Domain.Entities.ProductItem", "ProductItem")
                        .WithMany("Variations")
                        .HasForeignKey("ProductItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("MasterSizeIdentifier");

                    b.Navigation("ProductItem");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.SizeCategory", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("LookGenerator.Domain.Entities.SizeCategory", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Creator");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.SizeOption", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("LookGenerator.Domain.Entities.SizeCategory", "SizeCategory")
                        .WithMany("SizeOptions")
                        .HasForeignKey("SizeCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("SizeCategory");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.User", b =>
                {
                    b.HasOne("LookGenerator.Domain.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.AttributeOption", b =>
                {
                    b.Navigation("ProductAttributeOptions");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.AttributeType", b =>
                {
                    b.Navigation("AttributeOptions");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.Look", b =>
                {
                    b.Navigation("LookProductVariations");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.MasterSizeIdentifier", b =>
                {
                    b.Navigation("ProductVariations");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.Product", b =>
                {
                    b.Navigation("Items");

                    b.Navigation("ProductAttributeOptions");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductCategory", b =>
                {
                    b.Navigation("Products");

                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductItem", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Variations");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.ProductVariation", b =>
                {
                    b.Navigation("LookProductVariations");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.SizeCategory", b =>
                {
                    b.Navigation("SizeOptions");

                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.SizeOption", b =>
                {
                    b.Navigation("MasterSizeIds");
                });

            modelBuilder.Entity("LookGenerator.Domain.Entities.User", b =>
                {
                    b.Navigation("Looks");
                });
#pragma warning restore 612, 618
        }
    }
}