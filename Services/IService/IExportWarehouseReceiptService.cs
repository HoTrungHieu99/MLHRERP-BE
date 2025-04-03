using BusinessObject.DTO.RequestExport;
using BusinessObject.DTO.Warehouse;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IExportWarehouseReceiptService
    {
        Task<ExportWarehouseReceipt> CreateReceiptAsync(ExportWarehouseReceiptDTO dto);
        Task ApproveReceiptAsync(long id, Guid approvedBy);

        Task<List<ExportWarehouseReceipt>> GetAllReceiptsByWarehouseIdAsync(long warehouseId, string? sortBy = null);

        Task<ExportWarehouseReceipt> CreateFromRequestAsync(int requestExportId, long warehouseId, Guid approvedBy);
        Task<bool> UpdateExportReceiptAsync(UpdateExportWarehouseReceiptFullDto dto);

        Task<ExportWarehouseReceipt> CreateReceiptFromTransferAsync(ExportWarehouseTransferDTO dto, Guid userId);

    }

}
