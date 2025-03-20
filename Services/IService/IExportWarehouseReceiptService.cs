using BusinessObject.DTO;
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
        Task ApproveReceiptAsync(long id);
        Task<List<ExportWarehouseReceipt>> GetAllReceiptsByWarehouseIdAsync(long warehouseId);
    }

}
