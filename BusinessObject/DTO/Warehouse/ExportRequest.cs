using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Warehouse
{
    public class ExportRequest
    {
        // Số chứng từ
        public string DocumentNumber { get; set; }

        // Ngày chứng từ
        public DateTime DocumentDate { get; set; }

        // Ngày xuất
        public DateTime ExportDate { get; set; }

        // Loại xuất (xuất bán, xuất chuyển, xuất hủy)
        public string ExportType { get; set; }

        // Tên sản phẩm
        public string ProductName { get; set; }

        // Tên kho
        public string WarehouseName { get; set; }

        // Tổng số lượng
        public int TotalQuantity { get; set; }

        // Đơn giá
        public decimal UnitPrice { get; set; }

        // Số lượng
        public int Quantity { get; set; }

        // Thành tiền sản phẩm (Số lượng * Đơn giá)
        public decimal TotalProductAmount => Quantity * UnitPrice;

        // Ngày hết hạn của sản phẩm
        public DateTime ExpiryDate { get; set; }

        // Tổng thành tiền (Tính từ số lượng và đơn giá)
        public decimal TotalAmount => Quantity * UnitPrice;

        // Thông tin về đơn hàng (liên kết với bảng ExportTransaction)
        public long ExportTransactionId { get; set; }
    }

}
