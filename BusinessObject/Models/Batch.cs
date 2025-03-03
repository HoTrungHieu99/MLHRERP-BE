using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Batch
    {
        [Key]
        public long BatchId { get; set; }
        public long ImportTransactionDetailId { get; set; }
        public long ProductId { get; set; }
        public string BatchCode { get; set; }
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Available, Expired

        [ForeignKey("ImportTransactionDetailId")]
        public ImportTransactionDetail ImportTransactionDetail { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
