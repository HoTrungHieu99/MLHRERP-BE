using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UpdateExportWarehouseReceiptFullDto
    {
        public long ExportWarehouseReceiptId { get; set; }
        public DateTime ExportDate { get; set; }
        public string ExportType { get; set; }

        public List<ExportWarehouseReceiptDetailEditDto> AddDetails { get; set; } = new();
        public List<ExportWarehouseReceiptDetailEditDto> UpdateDetails { get; set; } = new();
        public List<long> DeleteDetailIds { get; set; } = new();
    }

    public class ExportWarehouseReceiptDetailEditDto
    {
        public long? ExportWarehouseReceiptDetailId { get; set; } // null nếu là thêm mới

        public long WarehouseProductId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string BatchNumber { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime ExpiryDate { get; set; }

        public decimal TotalProductAmount => Quantity * UnitPrice;
    }
}
