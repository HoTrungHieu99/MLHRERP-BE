using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ExportWarehouseReceiptDetail
    {
        [Key]
        public long ExportWarehouseReceiptDetailId { get; set; }

        [Required]
        public long ExportWarehouseReceiptId { get; set; } // FK đến phiếu xuất kho

        [Required]
        public long WarehouseProductId { get; set; } // FK đến sản phẩm theo lô trong kho

        // 🔹 Thông tin chi tiết sản phẩm
        // 🔹 `ProductId` lấy từ `WarehouseProduct`
        [Required]
        public long ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [Required]
        public string ProductName { get; set; } // Tên sản phẩm

        [Required]
        public string BatchNumber { get; set; } // Mã lô hàng

        [Required]
        public int Quantity { get; set; } // Số lượng xuất

        [Required]
        public decimal UnitPrice { get; set; } // Đơn giá sản phẩm

        [Required]
        public decimal TotalProductAmount { get; set; } // Thành tiền (Số lượng * Đơn giá)

        [Required]
        public DateTime ExpiryDate { get; set; } // Ngày hết hạn của lô hàng

        // 🔹 Khóa ngoại đến ExportWarehouseReceipt
        [ForeignKey("ExportWarehouseReceiptId")]
        public ExportWarehouseReceipt ExportWarehouseReceipt { get; set; }

        // 🔹 Khóa ngoại đến WarehouseProduct
        [ForeignKey("WarehouseProductId")]
        public WarehouseProduct WarehouseProduct { get; set; }
    }

}
