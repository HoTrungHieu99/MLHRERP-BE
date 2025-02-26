using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IWarehouseRepository
    {
        List<Warehouse> GetAllWarehouses();
        Warehouse GetWarehouseByUserId(Guid userId);
        Warehouse GetWarehouseById(int warehouseId);
        void AddWarehouse(Warehouse warehouse);
        void UpdateWarehouse(Warehouse warehouse);
        void DeleteWarehouse(int warehouseId);
    }
}
