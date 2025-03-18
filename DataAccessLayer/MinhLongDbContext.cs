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

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        var configuration = new ConfigurationBuilder()
        //            .SetBasePath(Directory.GetCurrentDirectory())
        //            .AddJsonFile("appsettings.json")
        //            .Build();

        //        var connectionString = configuration.GetConnectionString("ServerConnection");
        //        optionsBuilder.UseSqlServer(connectionString);
        //    }
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ServerConnection"));
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
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<TaxConfig> TaxConfigs { get; set; }
        public DbSet<ImportTransaction> ImportTransactions { get; set; }
        public DbSet<ImportTransactionDetail> ImportTransactionDetails { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<WarehouseProduct> WarehouseProduct { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<RequestProduct> RequestProducts { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<WarehouseReceipt> WarehouseReceipts { get; set; }
        public DbSet<ExportWarehouseReceipt> ExportWarehouseReceipts { get; set; }
        public DbSet<ExportTransaction> ExportTransactions { get; set; }
        public DbSet<ExportTransactionDetail> ExportTransactionDetails { get; set; }
        public DbSet<WarehouseLedger> WarehouseLedgers { get; set; }

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
            modelBuilder.Entity<RegisterAccount>().ToTable("RegisterAccount");
            modelBuilder.Entity<Province>().ToTable("Province");
            modelBuilder.Entity<District>().ToTable("District");
            modelBuilder.Entity<Ward>().ToTable("Ward");
            modelBuilder.Entity<Address>().ToTable("Address");
            modelBuilder.Entity<Warehouse>().ToTable("Warehouse");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory");
            modelBuilder.Entity<TaxConfig>().ToTable("TaxConfig");
            modelBuilder.Entity<ImportTransaction>().ToTable("ImportTransaction");
            modelBuilder.Entity<ImportTransactionDetail>().ToTable("ImportTransactionDetail");
            modelBuilder.Entity<Batch>().ToTable("Batch");
            modelBuilder.Entity<WarehouseProduct>().ToTable("WarehouseProduct");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<RequestProduct>().ToTable("RequestProduct");
            modelBuilder.Entity<Image>().ToTable("Image");
            modelBuilder.Entity<WarehouseReceipt>().ToTable("WarehouseReceipt");
            modelBuilder.Entity<ExportWarehouseReceipt>().ToTable("ExportWarehouseReceipt");
            modelBuilder.Entity<ExportTransaction>().ToTable("ExportTransaction");
            modelBuilder.Entity<ExportTransactionDetail>().ToTable("ExportTransactionDetail");
            modelBuilder.Entity<WarehouseLedger>().ToTable("WarehouseLedger");

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

            // 🔥 Cấu hình khóa chính tự động tăng
            modelBuilder.Entity<User>().Property(u => u.UserId).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Employee>().Property(e => e.EmployeeId).ValueGeneratedOnAdd();
            modelBuilder.Entity<AgencyAccount>().Property(a => a.AgencyId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Warehouse>().Property(w => w.WarehouseId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Product>().Property(p => p.ProductId).ValueGeneratedOnAdd();
            modelBuilder.Entity<ProductCategory>().Property(pc => pc.CategoryId).ValueGeneratedOnAdd();
            modelBuilder.Entity<TaxConfig>().Property(tc => tc.TaxId).ValueGeneratedOnAdd();
            modelBuilder.Entity<ImportTransaction>().Property(it => it.ImportTransactionId).ValueGeneratedOnAdd();
            modelBuilder.Entity<ImportTransactionDetail>().Property(itd => itd.ImportTransactionDetailId).ValueGeneratedOnAdd();
            modelBuilder.Entity<WarehouseProduct>().Property(i => i.WarehouseProductId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Order>().Property(o => o.OrderId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Batch>().Property(b => b.BatchId).ValueGeneratedOnAdd();
            modelBuilder.Entity<RequestProduct>().Property(r => r.RequestProductId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Image>().Property(i => i.ImageId).ValueGeneratedOnAdd();
            modelBuilder.Entity<WarehouseReceipt>().Property(wr => wr.WarehouseReceiptId).ValueGeneratedOnAdd();
            modelBuilder.Entity<WarehouseLedger>().Property(wr => wr.WarehouseLedgerId).ValueGeneratedOnAdd();
            modelBuilder.Entity<ExportTransaction>().Property(wr => wr.ExportTransactionId).ValueGeneratedOnAdd();
            modelBuilder.Entity<ExportTransactionDetail>().Property(wr => wr.ExportTransactionDetailId).ValueGeneratedOnAdd();
            modelBuilder.Entity<ExportWarehouseReceipt>().Property(wr => wr.ExportWarehouseReceiptId).ValueGeneratedOnAdd();


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
            // Đảm bảo mỗi User chỉ có 1 Warehouse
            modelBuilder.Entity<Warehouse>()
                .HasIndex(w => w.UserId)
                .IsUnique();
            // Liên kết Warehouse với Address (1 Warehouse - 1 Address)
            modelBuilder.Entity<Warehouse>()
                .HasOne(w => w.Address)
                .WithOne(a => a.Warehouse) // 🔥 1-1 Mapping
                .HasForeignKey<Warehouse>(w => w.AddressId)
                .OnDelete(DeleteBehavior.Cascade);
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

            // Configurations if needed
            modelBuilder.Entity<ImportTransaction>()
                .HasMany(it => it.ImportTransactionDetails)
                .WithOne(itd => itd.ImportTransaction)
                .HasForeignKey(itd => itd.ImportTransactionId);

            modelBuilder.Entity<ImportTransactionDetail>()
                .HasMany(itd => itd.Batches)
                .WithOne(b => b.ImportTransactionDetail)
                .HasForeignKey(b => b.ImportTransactionDetailId);

            modelBuilder.Entity<WarehouseProduct>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.NoAction); // Fixing cascade issue

            modelBuilder.Entity<WarehouseProduct>()
                .HasOne(i => i.Warehouse)
                .WithMany()
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<WarehouseProduct>()
                .HasOne(i => i.Batch)
                .WithMany()
                .HasForeignKey(i => i.BatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Request)
                .WithMany()
                .HasForeignKey(o => o.RequestId);

            modelBuilder.Entity<RequestProduct>()
                .HasOne(r => r.AgencyAccount)
                .WithMany()
                .HasForeignKey(r => r.AgencyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RequestProduct>()
                .HasOne(r => r.ApprovedByEmployee)
                .WithMany()
                .HasForeignKey(r => r.ApprovedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RequestProduct>()
                .HasOne(r => r.AgencyAccount)
                .WithMany()
                .HasForeignKey(r => r.AgencyId)
                .OnDelete(DeleteBehavior.NoAction); // Assuming Request is linked to an AgencyAccount

            modelBuilder.Entity<Batch>()
                .HasOne(b => b.ImportTransactionDetail)
                .WithMany(d => d.Batches)
                .HasForeignKey(b => b.ImportTransactionDetailId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Chỉ dùng cascade khi thực sự cần

            // Explicitly define precision and scale for decimal fields to avoid truncation issues
            modelBuilder.Entity<Batch>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Batch>()
                .Property(b => b.UnitCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ImportTransactionDetail>()
                .Property(itd => itd.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Discount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.FinalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Image>()
            .HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId);

            // Định dạng cột decimal (18,2) cho TotalPrice
            modelBuilder.Entity<WarehouseReceipt>()
                .Property(w => w.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Batch>()
                .Property(b => b.UnitCost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Batch>()
                .Property(b => b.TotalAmount)
                .HasColumnType("decimal(18,2)");

            //Moi Quan He Warehouse, WarehouseProduct, ExportTransaction, ExportTransactionDetail
            // 🔹 1. ExportTransaction ↔ Warehouse (1-Nhiều)
            modelBuilder.Entity<ExportTransaction>()
                .HasOne(et => et.Warehouse)
                .WithMany(w => w.ExportTransactions)
                .HasForeignKey(et => et.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 2. ExportTransaction ↔ ExportTransactionDetail (1-Nhiều)
            modelBuilder.Entity<ExportTransactionDetail>()
                .HasOne(etd => etd.ExportTransaction)
                .WithMany(et => et.ExportTransactionDetail)
                .HasForeignKey(etd => etd.ExportTransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 3. ExportTransactionDetail ↔ WarehouseProduct (1-Nhiều)
            modelBuilder.Entity<ExportTransactionDetail>()
                .HasOne(etd => etd.WarehouseProduct)
                .WithMany(wp => wp.ExportTransactionDetails) // ✅ Đúng với WarehouseProduct
                .HasForeignKey(etd => etd.WarehouseProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 4. ExportWarehouseReceipt ↔ Warehouse (1-Nhiều)
            modelBuilder.Entity<ExportWarehouseReceipt>()
                .HasOne(ewr => ewr.Warehouse)
                .WithMany(w => w.ExportWarehouseReceipts)
                .HasForeignKey(ewr => ewr.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 ExportWarehouseReceipt ↔ ExportWarehouseReceiptDetail (1-Nhiều)
            modelBuilder.Entity<ExportWarehouseReceiptDetail>()
                .HasOne(ewrd => ewrd.ExportWarehouseReceipt)
                .WithMany(ewr => ewr.ExportWarehouseReceiptDetails)
                .HasForeignKey(ewrd => ewrd.ExportWarehouseReceiptId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 ExportWarehouseReceiptDetail ↔ WarehouseProduct (1-Nhiều)
            modelBuilder.Entity<ExportWarehouseReceiptDetail>()
                .HasOne(ewrd => ewrd.WarehouseProduct)
                .WithMany()
                .HasForeignKey(ewrd => ewrd.WarehouseProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 7. WarehouseLedger ↔ Warehouse (1-Nhiều)
            modelBuilder.Entity<WarehouseLedger>()
                .HasOne(wl => wl.Warehouse)
                .WithMany(w => w.WarehouseLedgers)
                .HasForeignKey(wl => wl.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 8. WarehouseLedger ↔ ExportTransaction (1-1 hoặc 1-Nhiều, nullable)
            modelBuilder.Entity<WarehouseLedger>()
                .HasOne(wl => wl.ExportTransaction)
                .WithMany()
                .HasForeignKey(wl => wl.ExportTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 9. WarehouseLedger ↔ ImportTransaction (1-1 hoặc 1-Nhiều, nullable)
            modelBuilder.Entity<WarehouseLedger>()
                .HasOne(wl => wl.ImportTransaction)
                .WithMany()
                .HasForeignKey(wl => wl.ImportTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 WarehouseProduct ↔ ExportWarehouseReceiptDetail (1-Nhiều)
            modelBuilder.Entity<ExportWarehouseReceiptDetail>()
                .HasOne(ewrd => ewrd.WarehouseProduct)
                .WithMany(wp => wp.ExportWarehouseReceiptDetails)
                .HasForeignKey(ewrd => ewrd.WarehouseProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 WarehouseProduct ↔ Warehouse (1-Nhiều)
            modelBuilder.Entity<WarehouseProduct>()
                .HasOne(wp => wp.Warehouse)
                .WithMany(w => w.WarehouseProducts) // ✅ Đảm bảo Warehouse có `ICollection<WarehouseProduct>`
                .HasForeignKey(wp => wp.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExportTransactionDetail>()
                .Property(e => e.UnitPrice)
                .HasPrecision(18, 4); // Đảm bảo chính xác đến 4 số lẻ

            modelBuilder.Entity<ExportTransactionDetail>()
                .Property(e => e.TotalProductAmount)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ExportWarehouseReceipt>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ExportWarehouseReceiptDetail>()
                .Property(e => e.UnitPrice)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ExportWarehouseReceiptDetail>()
                .Property(e => e.TotalProductAmount)
                .HasPrecision(18, 4);

            modelBuilder.Entity<WarehouseLedger>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 4);

        }
    }

}
