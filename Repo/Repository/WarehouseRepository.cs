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

        public List<Warehouse> GetAllWarehouses() => _context.Warehouses.ToList();

        public Warehouse GetWarehouseByUserId(Guid userId)
            => _context.Warehouses.FirstOrDefault(w => w.UserId == userId);

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

    }
}
