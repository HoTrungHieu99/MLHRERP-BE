using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ExportTransactionDetail
    {
        [Key]
        public long ExportTransactionDetailId { get; set; } // Mã chi tiết xuất kho

        [Required]
        public long ExportTransactionId { get; set; } // FK đến phiếu xuất kho

        [Required]
        public long WarehouseProductId { get; set; } // FK đến sản phẩm theo lô

        [Required]
        public int Quantity { get; set; } // Số lượng xuất kho

        [Required]
        [Column(TypeName = "decimal(18,4)")] // Đảm bảo không mất dữ liệu
        public decimal UnitPrice { get; set; } // Đơn giá sản phẩm

        [Required]
        [Column(TypeName = "decimal(18,4)")] // Đảm bảo không mất dữ liệu
        public decimal TotalProductAmount { get; set; } // Thành tiền (Số lượng * Đơn giá)
        [Required]
        public DateTime ExpiryDate { get; set; } // Ngày hết hạn của lô hàng

        // 🔹 Khóa ngoại đến ExportTransaction
        [ForeignKey("ExportTransactionId")]
        public ExportTransaction ExportTransaction { get; set; }

        // 🔹 Khóa ngoại đến WarehouseProduct (Thay thế ProductId)
        [ForeignKey("WarehouseProductId")]
        public WarehouseProduct WarehouseProduct { get; set; }
    }

}
