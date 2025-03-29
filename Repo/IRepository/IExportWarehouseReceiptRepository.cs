using BusinessObject.DTO;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IExportWarehouseReceiptRepository
    {
        Task<ExportWarehouseReceipt> CreateReceiptAsync(ExportWarehouseReceipt receipt);
        Task<ExportWarehouseReceipt> GetReceiptByIdAsync(long id);
        Task ApproveReceiptAsync(long id);
        Task<List<WarehouseProduct>> GetWarehouseProductsByIdsAsync(List<long> warehouseProductIds); // ✅ Thêm phương thức mới
        Task<ExportWarehouseReceipt> CreateFromRequestAsync(int requestExportId, long warehouseId);

        Task<bool> UpdateFullAsync(UpdateExportWarehouseReceiptFullDto dto);
    }

}
