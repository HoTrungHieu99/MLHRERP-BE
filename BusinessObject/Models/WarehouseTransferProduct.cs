using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class WarehouseTransferProduct
    {
        [Key]
        public long Id { get; set; }

        // 🔗 FK đến yêu cầu điều phối
        [Required]
        public long WarehouseTransferRequestId { get; set; }

        [ForeignKey("WarehouseTransferRequestId")]
        public WarehouseTransferRequest TransferRequest { get; set; }

        // 🔗 FK đến sản phẩm
        [Required]
        public long ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [MaxLength(255)]
        public string? Notes { get; set; }
    }
}
