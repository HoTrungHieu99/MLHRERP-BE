using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Inventory
    {
        [Key]
        public long InventoryId { get; set; }
        public long ProductId { get; set; }
        public long WarehouseId { get; set; }
        public long BatchId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }

        [ForeignKey("BatchId")]
        public Batch Batch { get; set; }
    }
}
