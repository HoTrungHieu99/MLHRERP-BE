using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class WarehouseProductDto
    {
        public long WarehouseProductId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public long WarehouseId { get; set; }
        public long BatchId { get; set; }
        public string BatchCode { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}
