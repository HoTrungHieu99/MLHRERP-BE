using BusinessObject.Models;
using DataAccessLayer;
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

        public void DeleteWarehouse(int warehouseId)
        {
            var warehouse = _context.Warehouses.Find(warehouseId);
            if (warehouse != null)
            {
                _context.Warehouses.Remove(warehouse);
                _context.SaveChanges();
            }
        }
    }
}
