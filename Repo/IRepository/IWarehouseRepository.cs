using BusinessObject.DTO;
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
        //Warehouse GetWarehouseByUserId(Guid userId);
        Warehouse GetWarehouseById(int warehouseId);
        Task<Warehouse> GetWarehouseByIdAsync(int warehouseId);
        void AddWarehouse(Warehouse warehouse);
        void UpdateWarehouse(Warehouse warehouse);
        Task DeleteWarehouseAsync(Warehouse warehouse);

        Task<IEnumerable<WarehouseProductDto>> GetProductsByWarehouseIdAsync(long warehouseId);
        Task<WarehouseProductDto> GetProductByIdAsync(long warehouseProductId);
    }
}
