using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class WarehouseLedger
    {
        [Key]
        public long WarehouseLedgerId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } // IMPORT, EXPORT
        public int QuantityChange { get; set; }
        public int BalanceQuantity { get; set; }
        public decimal TotalAmount { get; set; } // Tổng giá trị nhập/xuất kho
        public string Note { get; set; }

        [Required]
        public long WarehouseId { get; set; } // FK đến kho

        public long? ExportTransactionId { get; set; } // FK đến phiếu xuất kho (nullable)
        public long? ImportTransactionId { get; set; } // FK đến phiếu nhập kho (nullable)

        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }

        [ForeignKey("ExportTransactionId")]
        public ExportTransaction ExportTransaction { get; set; }

        [ForeignKey("ImportTransactionId")]
        public ImportTransaction ImportTransaction{ get; set; }

    }
}
