﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer
{
    public class MinhLongDbContext : DbContext
    {
        public MinhLongDbContext(DbContextOptions options) : base(options) { }
        public MinhLongDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }


        // 🔥 Định nghĩa DbSet cho các bảng
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<AgencyAccount> AgencyAccounts { get; set; }
        public DbSet<AgencyLevel> AgencyLevels { get; set; }
        public DbSet<AgencyAccountLevel> AgencyAccountLevels { get; set; }
        public DbSet<RegisterAccount> RegisterAccounts { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<TaxConfig> TaxConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🏷️ **Định danh bảng**
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<Permission>().ToTable("Permission");
            modelBuilder.Entity<RolePermission>().ToTable("RolePermission");
            modelBuilder.Entity<AgencyAccount>().ToTable("AgencyAccount");
            modelBuilder.Entity<AgencyLevel>().ToTable("AgencyLevel");
            modelBuilder.Entity<AgencyAccountLevel>().ToTable("AgencyAccountLevel");
            modelBuilder.Entity<Province>().ToTable("Province");
            modelBuilder.Entity<District>().ToTable("District");
            modelBuilder.Entity<Ward>().ToTable("Ward");
            modelBuilder.Entity<Address>().ToTable("Address");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory");
            modelBuilder.Entity<TaxConfig>().ToTable("TaxConfig");



            // 🔥 **Cấu hình quan hệ**
            modelBuilder.Entity<Ward>()
                .HasOne(w => w.District)
                .WithMany(d => d.Wards)
                .HasForeignKey(w => w.DistrictId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<District>()
                .HasOne(d => d.Province)
                .WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.Cascade);

            // ❌ **Fix lỗi "Multiple Cascade Paths"**
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Ward)
                .WithMany(w => w.Addresses)
                .HasForeignKey(a => a.WardId)
                .OnDelete(DeleteBehavior.NoAction); // 🔥 Fix lỗi

            modelBuilder.Entity<Address>()
                .HasOne(a => a.District)
                .WithMany(d => d.Addresses)
                .HasForeignKey(a => a.DistrictId)
                .OnDelete(DeleteBehavior.NoAction); // 🔥 Fix lỗi

            modelBuilder.Entity<Address>()
                .HasOne(a => a.Province)
                .WithMany(p => p.Addresses)
                .HasForeignKey(a => a.ProvinceId)
                .OnDelete(DeleteBehavior.NoAction); // 🔥 Fix lỗi

            // ✅ **Cấu hình khóa chính tự động tăng**
            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Employee>()
                .Property(e => e.EmployeeId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AgencyAccount>()
                .Property(a => a.AgencyId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Address>()
                .Property(a => a.AddressId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Role>()
                .Property(r => r.RoleId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Permission>()
                .Property(p => p.PermissionId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<RolePermission>()
                .Property(rp => rp.RolePermissionId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserRole>()
                .Property(ur => ur.UserRoleId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<RegisterAccount>()
                .Property(ra => ra.RegisterId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Product>()
                .Property(p => p.ProductId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ProductCategory>()
                .Property(pc => pc.CategoryId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TaxConfig>()
                .Property(tc => tc.TaxId)
                .ValueGeneratedOnAdd();

            // 🔥 **Cấu hình quan hệ nhiều - nhiều**
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            // 🔥 **Quan hệ User - Employee (1-1)**
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId);

            // 🔥 **Quan hệ User - AgencyAccount (1-1)**
            modelBuilder.Entity<AgencyAccount>()
                .HasOne(a => a.User)
                .WithOne(u => u.AgencyAccount)
                .HasForeignKey<AgencyAccount>(a => a.UserId);

            // Cấu hình quan hệ ProductCategory
            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.ParentCategory)
                .WithMany()
                .HasForeignKey(pc => pc.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(c => c.Creator)
                .WithMany()
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(c => c.Updater)
                .WithMany()
                .HasForeignKey(c => c.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Cấu hình quan hệ Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.TaxConfig)
                .WithMany()
                .HasForeignKey(p => p.TaxId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Updater)
                .WithMany()
                .HasForeignKey(p => p.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // 🔥 **Cấu hình giá trị decimal**
            modelBuilder.Entity<AgencyAccountLevel>()
                .Property(aal => aal.MonthlyRevenue)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AgencyAccountLevel>()
                .Property(aal => aal.OrderDiscount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AgencyAccountLevel>()
                .Property(aal => aal.OrderRevenue)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AgencyAccountLevel>()
                .Property(aal => aal.TotalDebtValue)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AgencyLevel>()
                .Property(al => al.CreditLimit)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AgencyLevel>()
                .Property(al => al.DiscountPercentage)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<TaxConfig>()
                .Property(tc => tc.TaxRate)
                .HasColumnType("decimal(18, 4)");

            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasDefaultValue(false);
        }
    }

}
