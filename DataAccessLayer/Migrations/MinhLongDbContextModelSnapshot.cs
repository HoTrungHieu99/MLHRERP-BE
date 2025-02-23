﻿// <auto-generated />
using System;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(MinhLongDbContext))]
    partial class MinhLongDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BusinessObject.Models.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AddressId"));

                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.Property<int>("ProvinceId")
                        .HasColumnType("int");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WardId")
                        .HasColumnType("int");

                    b.HasKey("AddressId");

                    b.HasIndex("DistrictId");

                    b.HasIndex("ProvinceId");

                    b.HasIndex("WardId");

                    b.ToTable("Address", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.AgencyAccount", b =>
                {
                    b.Property<long>("AgencyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AgencyId"));

                    b.Property<int>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("AgencyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("AgencyId");

                    b.HasIndex("AddressId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("AgencyAccount", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.AgencyAccountLevel", b =>
                {
                    b.Property<long>("AgencyLevelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AgencyLevelId"));

                    b.Property<long>("AgencyId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ChangeDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("LevelId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("MonthlyRevenue")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("OrderDiscount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("OrderRevenue")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("TotalDebtValue")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("AgencyLevelId");

                    b.HasIndex("AgencyId");

                    b.HasIndex("LevelId");

                    b.ToTable("AgencyAccountLevel", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.AgencyLevel", b =>
                {
                    b.Property<long>("LevelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("LevelId"));

                    b.Property<decimal?>("CreditLimit")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal?>("DiscountPercentage")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("LevelName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PaymentTerm")
                        .HasColumnType("int");

                    b.HasKey("LevelId");

                    b.ToTable("AgencyLevel", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.District", b =>
                {
                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.Property<string>("CodeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DistrictName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DistrictType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProvinceId")
                        .HasColumnType("int");

                    b.HasKey("DistrictId");

                    b.HasIndex("ProvinceId");

                    b.ToTable("District", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.Employee", b =>
                {
                    b.Property<long>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("EmployeeId"));

                    b.Property<int>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("EmployeeId");

                    b.HasIndex("AddressId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Employee", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.Permission", b =>
                {
                    b.Property<long>("PermissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("PermissionId"));

                    b.Property<string>("PermissionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PermissionId");

                    b.ToTable("Permission", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.Province", b =>
                {
                    b.Property<int>("ProvinceId")
                        .HasColumnType("int");

                    b.Property<string>("CodeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PhoneCode")
                        .HasColumnType("int");

                    b.Property<string>("ProvinceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProvinceType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProvinceId");

                    b.ToTable("Province", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.RegisterAccount", b =>
                {
                    b.Property<int>("RegisterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RegisterId"));

                    b.Property<string>("AgencyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Department")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DistrictName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProvinceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WardName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RegisterId");

                    b.ToTable("RegisterAccounts");
                });

            modelBuilder.Entity("BusinessObject.Models.Role", b =>
                {
                    b.Property<long>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RoleId"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("Role", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.RolePermission", b =>
                {
                    b.Property<long>("RolePermissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RolePermissionId"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<long>("PermissionId")
                        .HasColumnType("bigint");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.HasKey("RolePermissionId");

                    b.HasIndex("PermissionId");

                    b.HasIndex("RoleId");

                    b.ToTable("RolePermission", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("UserType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("UserId");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.UserRole", b =>
                {
                    b.Property<long>("UserRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("UserRoleId"));

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserRoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRole", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.Ward", b =>
                {
                    b.Property<int>("WardId")
                        .HasColumnType("int");

                    b.Property<string>("CodeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.Property<string>("WardName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WardType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("WardId");

                    b.HasIndex("DistrictId");

                    b.ToTable("Ward", (string)null);
                });

            modelBuilder.Entity("BusinessObject.Models.Address", b =>
                {
                    b.HasOne("BusinessObject.Models.District", "District")
                        .WithMany("Addresses")
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Province", "Province")
                        .WithMany("Addresses")
                        .HasForeignKey("ProvinceId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Ward", "Ward")
                        .WithMany("Addresses")
                        .HasForeignKey("WardId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("District");

                    b.Navigation("Province");

                    b.Navigation("Ward");
                });

            modelBuilder.Entity("BusinessObject.Models.AgencyAccount", b =>
                {
                    b.HasOne("BusinessObject.Models.Address", "Address")
                        .WithMany("AgencyAccounts")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.User", "User")
                        .WithOne("AgencyAccount")
                        .HasForeignKey("BusinessObject.Models.AgencyAccount", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Models.AgencyAccountLevel", b =>
                {
                    b.HasOne("BusinessObject.Models.AgencyAccount", "Agency")
                        .WithMany()
                        .HasForeignKey("AgencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.AgencyLevel", "Level")
                        .WithMany("AgencyAccountLevels")
                        .HasForeignKey("LevelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agency");

                    b.Navigation("Level");
                });

            modelBuilder.Entity("BusinessObject.Models.District", b =>
                {
                    b.HasOne("BusinessObject.Models.Province", "Province")
                        .WithMany("Districts")
                        .HasForeignKey("ProvinceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Province");
                });

            modelBuilder.Entity("BusinessObject.Models.Employee", b =>
                {
                    b.HasOne("BusinessObject.Models.Address", "Address")
                        .WithMany("Employees")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.User", "User")
                        .WithOne("Employee")
                        .HasForeignKey("BusinessObject.Models.Employee", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Models.RolePermission", b =>
                {
                    b.HasOne("BusinessObject.Models.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BusinessObject.Models.UserRole", b =>
                {
                    b.HasOne("BusinessObject.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Models.Ward", b =>
                {
                    b.HasOne("BusinessObject.Models.District", "District")
                        .WithMany("Wards")
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("District");
                });

            modelBuilder.Entity("BusinessObject.Models.Address", b =>
                {
                    b.Navigation("AgencyAccounts");

                    b.Navigation("Employees");
                });

            modelBuilder.Entity("BusinessObject.Models.AgencyLevel", b =>
                {
                    b.Navigation("AgencyAccountLevels");
                });

            modelBuilder.Entity("BusinessObject.Models.District", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Wards");
                });

            modelBuilder.Entity("BusinessObject.Models.Permission", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("BusinessObject.Models.Province", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Districts");
                });

            modelBuilder.Entity("BusinessObject.Models.Role", b =>
                {
                    b.Navigation("RolePermissions");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("BusinessObject.Models.User", b =>
                {
                    b.Navigation("AgencyAccount")
                        .IsRequired();

                    b.Navigation("Employee")
                        .IsRequired();

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("BusinessObject.Models.Ward", b =>
                {
                    b.Navigation("Addresses");
                });
#pragma warning restore 612, 618
        }
    }
}
