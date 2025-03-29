using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Order
{
    public class OrderDetailDto
    {
        public Guid OrderDetailId { get; set; }
        public Guid OrderId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }  // ✅ Lấy từ navigation Product
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string Unit { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
