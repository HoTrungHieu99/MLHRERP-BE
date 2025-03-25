using BusinessObject.DTO;
using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repository
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly MinhLongDbContext _context;

        public WarehouseRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public List<Warehouse> GetAllWarehouses() => _context.Warehouses.Include(w => w.Address).ToList();

        public Warehouse GetWarehouseByUserId(Guid userId)
                => _context.Warehouses
               .Include(w => w.Address) // Load thông tin Address theo AddressId
               .FirstOrDefault(w => w.UserId == userId);


        public Warehouse GetWarehouseById(int warehouseId)
            => _context.Warehouses.Find(warehouseId);

        public async Task<Warehouse> GetWarehouseByIdAsync(int warehouseId)
        {
            return await _context.Warehouses
                .Include(w => w.Address) // Load Address để đảm bảo xóa Cascade
                .FirstOrDefaultAsync(w => w.WarehouseId == warehouseId);
        }

        public void AddWarehouse(Warehouse warehouse)
        {

            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();
        }

        public void UpdateWarehouse(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            _context.SaveChanges();
        }

        public async Task DeleteWarehouseAsync(Warehouse warehouse)
        {
            // Kiểm tra xem Warehouse có Address không
            if (warehouse.AddressId != null)
            {
                var address = await _context.Addresses.FindAsync(warehouse.AddressId);
                if (address != null)
                {
                    _context.Addresses.Remove(address); // 🔥 XÓA ADDRESS TRƯỚC
                }
            }

            _context.Warehouses.Remove(warehouse); // 🔥 Sau đó xóa Warehouse
            await _context.SaveChangesAsync(); // 🔥 Lưu thay đổi vào DB
        }

        public async Task<IEnumerable<WarehouseProductDto>> GetProductsByWarehouseIdAsync(long warehouseId)
        {
            return await _context.WarehouseProduct
                .Where(wp => wp.WarehouseId == warehouseId)
                .Select(wp => new WarehouseProductDto
                {
                    WarehouseProductId = wp.WarehouseProductId,
                    ProductId = wp.ProductId,
                    ProductName = wp.Product.ProductName,
                    WarehouseId = wp.WarehouseId,
                    BatchId = wp.BatchId,
                    BatchCode = wp.Batch.BatchCode,
                    ExpirationDate = wp.ExpirationDate,
                    Quantity = wp.Quantity,
                    Status = wp.Status,
                    Price = wp.Batch.SellingPrice
                })
                .ToListAsync();
        }

        public async Task<WarehouseProductDto> GetProductByIdAsync(long warehouseProductId)
        {
            return await _context.WarehouseProduct
                .Where(wp => wp.WarehouseProductId == warehouseProductId)
                .Select(wp => new WarehouseProductDto
                {
                    WarehouseProductId = wp.WarehouseProductId,
                    ProductId = wp.ProductId,
                    ProductName = wp.Product.ProductName,
                    WarehouseId = wp.WarehouseId,
                    BatchId = wp.BatchId,
                    BatchCode = wp.Batch.BatchCode,
                    ExpirationDate = wp.ExpirationDate,
                    Quantity = wp.Quantity,
                    Status = wp.Status,
                    Price = wp.Batch.SellingPrice
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> GetUserIdByWarehouseIdAsync(long warehouseId)
        {
            var userId = await _context.Warehouses
                .Where(w => w.WarehouseId == warehouseId)
                .Select(w => w.UserId)
                .FirstAsync(); // dùng FirstAsync vì bạn đảm bảo tồn tại

            return userId;
        }
    }
}
