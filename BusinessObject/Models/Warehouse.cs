using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Warehouse
    {
        [Key]
        public int WarehouseId { get; set; }

        [Required]
        public string WarehousName { get; set; }

        [Required]
        public Guid UserId { get; set; } // Người tạo Warehouse

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int AddressId { get; set; }  // Liên kết với bảng Address

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
    }
}
