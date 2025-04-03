using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ExportWarehouseReceipt
    {
        [Key]
        public long ExportWarehouseReceiptId { get; set; }  // Mã phiếu xuất kho
        public string DocumentNumber { get; set; }           // Số chứng từ
        public DateTime DocumentDate { get; set; }           // Ngày chứng từ
        public DateTime ExportDate { get; set; }             // Ngày xuất
        public string ExportType { get; set; }               // Loại xuất (xuất bán, xuất chuyển, xuất hủy)

        // 🔹 Thông tin tổng hợp
        public int TotalQuantity { get; set; }               // Tổng số lượng xuất
        public decimal TotalAmount { get; set; }             // Tổng tiền

        [ForeignKey("RequestExport")]
        public int RequestExportId { get; set; }
        public RequestExport RequestExport { get; set; }

        public string? AgencyName { get; set; }
        public string? OrderCode { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Trạng thái của phiếu xuất kho (Pending, Approved, Rejected)

        // 🔹 Khóa ngoại đến Warehouse
        [Required]
        public long WarehouseId { get; set; }
        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }

        // 🔹 Danh sách chi tiết sản phẩm (Liên kết với `ExportWarehouseReceiptDetail`)
        [ForeignKey("WarehouseTransferRequest")]
        public long? WarehouseTransferRequestId { get; set; }
        public WarehouseTransferRequest? WarehouseTransferRequest { get; set; }



        public ICollection<ExportWarehouseReceiptDetail> ExportWarehouseReceiptDetails { get; set; }
    }

}
