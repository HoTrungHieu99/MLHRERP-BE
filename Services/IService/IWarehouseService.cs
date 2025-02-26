using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IWarehouseService
    {
        List<Warehouse> GetAllWarehouses();
        Warehouse GetWarehouseByUserId(Guid userId);
        void CreateWarehouse(Guid userId, string warehousName, string street, string province, string district, string ward);
        void UpdateWarehouse(Guid userId, int warehouseId, string warehousName, string street, string province, string district, string ward);
        void DeleteWarehouse(int warehouseId);

    }
}
