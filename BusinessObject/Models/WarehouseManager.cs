using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class WarehouseManager
    {
        [Key]
        public int WarehouseManagerId { get; set; } // ✅ ID tự tăng

        [ForeignKey("Warehouse")]
        public long WarehouseId { get; set; } // ✅ Kho mà nhân viên quản lý
        public Warehouse Warehouse { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; } // ✅ Nhân viên thủ kho (User)
        public User User { get; set; }
        
    }

}
