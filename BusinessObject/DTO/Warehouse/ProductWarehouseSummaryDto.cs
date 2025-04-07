using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Warehouse
{
    public class ProductWarehouseSummaryDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }

        public long WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public int TotalQuantity { get; set; }
    }
}
