using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IWarehouseReceiptRepository
    {
        Task<bool> AddAsync(WarehouseReceipt warehouseReceipt);
        Task<WarehouseReceipt> GetByIdAsync(long id);
        Task<List<WarehouseReceipt>> GetAllAsync();
        Task<bool> ApproveAsync(long id, Guid currentUserId);

        Task<List<WarehouseReceipt>> GetWarehouseReceiptDTOIdAsync(long Id);
    }
}
