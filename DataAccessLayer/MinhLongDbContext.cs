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
        public MinhLongDbContext(DbContextOptions<MinhLongDbContext> options) : base(options) { }
        public MinhLongDbContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Đọc chuỗi kết nối từ appsettings.json
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        // Khai báo DbSet cho các bảng
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<AgencyAccount> AgencyAccounts { get; set; }
        public DbSet<AgencyLevel> AgencyLevels { get; set; }
        public DbSet<AgencyAccountLevel> AgencyAccountLevels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Đặt tên bảng để nhất quán với database
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
            // Cấu hình decimal(18, 2) cho các thuộc tính kiểu decimal
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

            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasDefaultValue(false);

            // Cấu hình tự động tăng cho tất cả các ID trong cơ sở dữ liệu
            // Cấu hình tự động tăng cho AgencyAccountId
            modelBuilder.Entity<AgencyAccount>()
                .Property(rp => rp.AgencyId)
                .ValueGeneratedOnAdd();

            // Cấu hình tự động tăng cho AgencyLevelId
            modelBuilder.Entity<AgencyAccountLevel>()
                .Property(rp => rp.AgencyLevelId)
                .ValueGeneratedOnAdd();

            // Cấu hình tự động tăng cho LevelId
            modelBuilder.Entity<AgencyLevel>()
                .Property(rp => rp.LevelId)
                .ValueGeneratedOnAdd();

            // Cấu hình tự động tăng cho EmployeeId
            modelBuilder.Entity<Employee>()
                .Property(rp => rp.EmployeeId)
                .ValueGeneratedOnAdd();

            // Cấu hình tự động tăng cho PermissionId
            modelBuilder.Entity<Permission>()
                .Property(rp => rp.PermissionId)
                .ValueGeneratedOnAdd();

            // Cấu hình tự động tăng cho RoleId
            modelBuilder.Entity<Role>()
                .Property(rp => rp.RoleId)
                .ValueGeneratedOnAdd();

            // Cấu hình tự động tăng cho RolePermissionId
            modelBuilder.Entity<RolePermission>()
                .Property(rp => rp.RolePermissionId)
                .ValueGeneratedOnAdd();

            // Cấu hình tự động tăng cho UserId
            modelBuilder.Entity<User>()
                .Property(rp => rp.UserId)
                .HasDefaultValueSql("NEWID()");

            // Cấu hình tự động tăng cho UserRoleId
            modelBuilder.Entity<UserRole>()
                .Property(rp => rp.UserRoleId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<RegisterAccount>()
                .Property(rp => rp.RegisterId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Address>()
                .Property(rp => rp.AddressId)
                .ValueGeneratedOnAdd();

            // Cấu hình quan hệ User-Role (N-N)
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .HasPrincipalKey(u => u.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Cấu hình quan hệ Role-Permission (N-N)
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            // Cấu hình quan hệ Employee-User (1-1)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId);

            // Cấu hình quan hệ Employee-Address (N-1)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Address)
                .WithMany(l => l.Employees)
                .HasForeignKey(e => e.AddressId);

            // Cấu hình quan hệ AgencyAccount-User (1-1)
            modelBuilder.Entity<AgencyAccount>()
                .HasOne(a => a.User)
                .WithOne(u => u.AgencyAccount)
                .HasForeignKey<AgencyAccount>(a => a.UserId);

            // Cấu hình quan hệ AgencyAccount-Location (N-1)
            modelBuilder.Entity<AgencyAccount>()
                .HasOne(a => a.Address)
                .WithMany(l => l.AgencyAccounts)
                .HasForeignKey(a => a.AddressId);

            // Cấu hình quan hệ AgencyAccountLevel-AgencyAccount (N-1)
            modelBuilder.Entity<AgencyAccountLevel>()
                .HasOne(aal => aal.Agency)
                .WithMany()
                .HasForeignKey(aal => aal.AgencyId);

            // Cấu hình quan hệ AgencyAccountLevel-AgencyLevel (N-1)
            modelBuilder.Entity<AgencyAccountLevel>()
                .HasOne(aal => aal.Level)
                .WithMany(al => al.AgencyAccountLevels)
                .HasForeignKey(aal => aal.LevelId);

            //Quan hệ Location
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

            modelBuilder.Entity<Address>()
                .HasOne(a => a.Ward)
                .WithMany(w => w.Addresses)
                .HasForeignKey(a => a.WardId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Vẫn để Cascade

            modelBuilder.Entity<Address>()
                .HasOne(a => a.District)
                .WithMany(d => d.Addresses)
                .HasForeignKey(a => a.DistrictId)
                .OnDelete(DeleteBehavior.NoAction); // ❌ Đổi từ CASCADE ➝ NO ACTION

            modelBuilder.Entity<Address>()
                .HasOne(a => a.Province)
                .WithMany(p => p.Addresses)
                .HasForeignKey(a => a.ProvinceId)
                .OnDelete(DeleteBehavior.NoAction); // ❌ Đổi từ CASCADE ➝ NO ACTION

        }
    }
}
