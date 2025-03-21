using BusinessObject.DTO;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IWarehouseReceiptService
    {
        Task<bool> CreateReceiptAsync(WarehouseReceiptRequest request, Guid currentUserId);
        Task<bool> ApproveReceiptAsync(long id, Guid currentUserId);
        Task<List<WarehouseReceiptDTO>> GetAllReceiptsByWarehouseIdAsync(long warehouseId);

        Task<WarehouseReceiptDTO?> GetWarehouseReceiptDTOIdAsync(long Id);
    }

}
