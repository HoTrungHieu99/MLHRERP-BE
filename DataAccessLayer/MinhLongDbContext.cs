using System;
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

        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<RequestExport> RequestExports { get; set; }
        public DbSet<RequestExportDetail> RequestExportDetails { get; set; }
        public DbSet<WarehouseRequestExport> WarehouseRequestExports { get; set; }

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
            modelBuilder.Entity<PaymentHistory>().ToTable("PaymentHistory");
            modelBuilder.Entity<PaymentTransaction>().ToTable("PaymentTransaction");
            modelBuilder.Entity<RequestExport>().ToTable("RequestExport");
            modelBuilder.Entity<RequestExportDetail>().ToTable("RequestExportDetail");
            modelBuilder.Entity<WarehouseRequestExport>().ToTable("WarehouseRequestExport");


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
            modelBuilder.Entity<Order>().Property(o => o.OrderId).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Batch>().Property(b => b.BatchId).ValueGeneratedOnAdd();
            modelBuilder.Entity<RequestProduct>().Property(r => r.RequestProductId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Image>().Property(i => i.ImageId).ValueGeneratedOnAdd();
            modelBuilder.Entity<WarehouseReceipt>().Property(wr => wr.WarehouseReceiptId).ValueGeneratedOnAdd();
            modelBuilder.Entity<PaymentHistory>().Property(ph => ph.PaymentHistoryId).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<PaymentTransaction>().Property(pt => pt.TransactionId).HasDefaultValueSql("NEWID()");


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

            // 🏷️ Cấu hình bảng PaymentHistory
            modelBuilder.Entity<PaymentHistory>()
                .ToTable("PaymentHistory")
                .HasOne(ph => ph.Order)
                .WithMany(o => o.PaymentHistories)
                .HasForeignKey(ph => ph.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Khi xóa Order, xóa tất cả PaymentHistory liên quan

            modelBuilder.Entity<PaymentHistory>()
                .HasOne(ph => ph.PrePayment)
                .WithMany()
                .HasForeignKey(ph => ph.PrePaymentId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa PaymentHistory nếu có liên kết với PrePayment

            modelBuilder.Entity<PaymentHistory>()
                .Property(ph => ph.RemainingDebtAmount)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<PaymentHistory>()
                .Property(ph => ph.Amount)
                .HasColumnType("decimal(10,2)");

            // 🏷️ Cấu hình bảng PaymentTransaction
            modelBuilder.Entity<PaymentTransaction>()
                .ToTable("PaymentTransaction")
                .HasOne(pt => pt.PaymentHistory)
                .WithMany(ph => ph.PaymentTransactions)
                .HasForeignKey(pt => pt.PaymentHistoryId)
                .OnDelete(DeleteBehavior.Cascade); // Khi xóa PaymentHistory, xóa tất cả PaymentTransaction liên quan

            modelBuilder.Entity<PaymentTransaction>()
                .Property(pt => pt.Amount)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<PaymentTransaction>()
                .Property(pt => pt.TransactionReference)
                .HasMaxLength(100);

            // 🔥 Cấu hình quan hệ Order
            modelBuilder.Entity<Order>()
                .ToTable("Order")
                .HasMany(o => o.PaymentHistories)
                .WithOne(ph => ph.Order)
                .HasForeignKey(ph => ph.OrderId);

            modelBuilder.Entity<RequestExport>()
        .ToTable("RequestExport")
        .HasOne(re => re.RequestedByEmployee)
        .WithMany()
        .HasForeignKey(re => re.RequestedBy)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestExport>()
                .HasOne(re => re.ApprovedByEmployee)
                .WithMany()
                .HasForeignKey(re => re.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestExport>()
                .HasOne(re => re.Order)
                .WithMany()
                .HasForeignKey(re => re.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestExportDetail>()
                .ToTable("RequestExportDetail")
                .HasOne(red => red.RequestExport)
                .WithMany(re => re.RequestExportDetails)
                .HasForeignKey(red => red.RequestExportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RequestExportDetail>()
                .HasOne(red => red.Product)
                .WithMany()
                .HasForeignKey(red => red.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarehouseRequestExport>()
                .ToTable("WarehouseRequestExport")
                .HasOne(wre => wre.RequestExport)
                .WithMany(re => re.WarehouseRequestExports)
                .HasForeignKey(wre => wre.RequestExportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WarehouseRequestExport>()
                .HasOne(wre => wre.Warehouse)
                .WithMany()
                .HasForeignKey(wre => wre.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestExport>()
                .HasOne(re => re.Order)
                .WithMany(o => o.RequestExports)
                .HasForeignKey(re => re.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }

}
