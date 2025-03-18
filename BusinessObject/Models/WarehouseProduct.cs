using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class WarehouseProduct
    {
        [Key]
        public long WarehouseProductId { get; set; }

        [Required]
        public long ProductId { get; set; }

        [Required]
        public long WarehouseId { get; set; }

        [Required]
        public long BatchId { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int Quantity { get; set; }

        public string Status { get; set; }

        // 🔹 Foreign Keys
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }

        [ForeignKey("BatchId")]
        public Batch Batch { get; set; }

        // 🔹 Mối quan hệ với ExportTransactionDetail
        public ICollection<ExportTransactionDetail> ExportTransactionDetails { get; set; }

        // 🔹 Mối quan hệ với ExportWarehouseReceiptDetail (ĐÃ SỬA)
        public ICollection<ExportWarehouseReceiptDetail> ExportWarehouseReceiptDetails { get; set; }
    }

}
